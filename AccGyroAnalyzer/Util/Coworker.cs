using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.Reflection;

namespace AsyncModel {

    public class Coworker {
        [ThreadStatic]
        private static TaskRunner currentRunner;

        private class TaskRunner {
            private readonly Object waitLock = new Object();
            private bool block;
            private Action task;
            private Exception exception;
            private SynchronizationContext context;
            private Dispatcher dispatcher;

            public void Run(Action task, bool runAsync) {
                this.block = !runAsync;
                this.context = SynchronizationContext.Current;
                this.dispatcher = Dispatcher.FromThread(Thread.CurrentThread);
                this.task = task;
                if (runAsync) {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(RunTaskAsync));
                }
                else {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(RunTask));
                    Wait();
                }
            }

            private static readonly FieldInfo dispatcherThreadField = typeof(Dispatcher).GetField("_dispatcherThread", BindingFlags.NonPublic | BindingFlags.Instance);

            private object oldDispatcherThread;

            private void EnterSyncState() {
                SynchronizationContext.SetSynchronizationContext(context);

                if (dispatcher != null) {
                    oldDispatcherThread = dispatcherThreadField.GetValue(dispatcher);
                    dispatcherThreadField.SetValue(dispatcher, Thread.CurrentThread);
                }
            }

            private void ExitSyncState() {
                SynchronizationContext.SetSynchronizationContext(context);

                if (dispatcher != null) {
                    dispatcherThreadField.SetValue(dispatcher, oldDispatcherThread);
                }
            }

            private void RunTaskAsync(object dummy) {

                currentRunner = this;
                try {
                    task();
                }
                catch (Exception ex) {
                    exception = ex;
                    if (context != null) {
                        context.Post(new SendOrPostCallback(x => {
                            ThrowUncaughtException();
                        }), null);
                    }
                    else {
                        ThrowUncaughtException();
                    }
                }
                finally {
                    // Just in case we have an unfinished sync block, try to release ui thread
                    Yield();
                    currentRunner = null;
                }
            }

            private void RunTask(object dummy) {
                EnterSyncState();

                currentRunner = this;
                try {
                    task();
                }
                catch (Exception ex) {
                    exception = ex;
                }
                finally {
                    Yield();
                    currentRunner = null;
                }
            }


            /// <summary>
            /// Ends a synchronous block by releasing the waiting UI thread 
            /// </summary>
            public bool Yield() {
                lock (waitLock) {
                    if (!block) return false;

                    ExitSyncState();
                    block = false;
                    Monitor.Pulse(waitLock);
                }
                return true;
            }

            /// <summary>
            /// Resumes synchronous mode by forcing the ui thread in a waiting state.
            /// </summary>
            public bool Resume() {
                if (context != null) {
                    lock (waitLock) {
                        if (block) return false;

                        context.Post(new SendOrPostCallback(o => {
                            lock (waitLock) {
                                block = true;
                                Monitor.Pulse(waitLock);
                            }
                            Wait();
                        }), null);

                        // Wait until block is set to true by above code.
                        while (!block) Monitor.Wait(waitLock);
                    }

                    EnterSyncState();

                    return true;
                }
                return true;
            }

            /// <summary>
            /// Blocks the ui thread during synchronous mode.
            /// </summary>
            public void Wait() {
                // Block calling UI thread until asynchronous block are reached
                lock (waitLock) {
                    while (block) Monitor.Wait(waitLock);
                }
                if (exception != null) {
                    ThrowUncaughtException();
                }
            }

            private void ThrowUncaughtException() {
                throw new UncaughtCoworkerException("Uncaught exception in Coworker: " + exception.Message, exception);
            }


        }

        public class AsyncBlockManager : IDisposable {
            TaskRunner runner;

            public AsyncBlockManager() {
                runner = currentRunner;
                if (runner != null) {
                    if (!runner.Yield()) {
                        // If already in asynchronous mode reset runner so ending/disposing this block
                        // will not have any effect.
                        runner = null;
                    }
                }
            }

            public void EndBlock() {
                Dispose();
            }

            #region IDisposable Members

            public void Dispose() {
                if (runner != null) {
                    runner.Resume();
                }
                runner = null;
            }

            #endregion
        }

        /// <summary>
        /// Starts a block of code asynchronously (use in a using statement).
        /// </summary>
        /// <returns>A IDisposable object to be disposed when to resume the synchronization context.</returns>
        public static AsyncBlockManager AsyncBlock() {
            return new AsyncBlockManager();
        }

        /// <summary>
        /// Runs the given action in asynchronous mode, i.e. releasing any waiting UI thread during its execution.
        /// </summary>
        /// <param name="actionToBeRunAsync">Action to be performed in asynchronous mode</param>
        public static void AsyncBlock(Action actionToBeRunAsync) {
            if (currentRunner != null) {
                throw new InvalidOperationException("The version of Coworker.AsyncBlock taking an Action argument can " +
                                                "only be called from a non-Coworker context. Use the AsyncBlock() " +
                                                "in a 'using' statement instead if you want to run a block of code asynchronously.");

            }
            else {
                new TaskRunner().Run(actionToBeRunAsync, true);
            }
        }


        public class SyncBlockManager : IDisposable {
            TaskRunner runner;

            public SyncBlockManager() {
                runner = currentRunner;
                if (runner != null) {
                    if (!runner.Resume()) {
                        // If already in synchronous mode reset runner so that ending/disposing this block
                        // will not have any effect.
                        runner = null;
                    }
                }
            }

            public void EndBlock() {
                Dispose();
            }

            #region IDisposable Members

            public void Dispose() {
                if (runner != null) {
                    runner.Yield();
                }
                runner = null;
            }

            #endregion
        }

        /// <summary>
        /// Starts a block of code to be run synchronously with UI thread (to be used in a using statement).
        /// </summary>
        /// <returns>A IDisposable object to be disposed when to resume asynchronous mode.</returns>
        public static SyncBlockManager SyncBlock() {
            return new SyncBlockManager();
        }

        /// <summary>
        /// Runs the given action in synchronous mode on a cooperative mode, 
        /// allowing blocks of code to run asynchronous using <see cref="AsyncBlock"/>.
        /// </summary>
        /// <param name="actionToBeRunSync">Action to be performed in synchronous mode</param>
        public static void SyncBlock(Action actionToBeRunSync) {
            if (currentRunner != null) {
                if (currentRunner != null) {
                    throw new InvalidOperationException("The version of Coworker.SyncBlock taking an Action argument can " +
                                                    "only be called from a non-Coworker context. Use the AsyncBlock() " +
                                                    "in a 'using' statement instead.");

                }
            }
            else {
                TaskRunner runner = new TaskRunner();
                runner.Run(actionToBeRunSync, false);
            }
        }

    }

    [Serializable]
    public class UncaughtCoworkerException : Exception {

        public UncaughtCoworkerException() { }
        public UncaughtCoworkerException(string message) : base(message) { }
        public UncaughtCoworkerException(string message, Exception inner) : base(message, inner) { }
        protected UncaughtCoworkerException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
