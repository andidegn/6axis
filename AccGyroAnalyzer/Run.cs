using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AsyncModel;

namespace ECTunes.AccGyroAnalyzer {
    static class Run {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            //Coworker.SyncBlock(() => {
            //    try {
            //        Form1.port = CustomInputBox.InputBox.Show("Please select COM port", "COM Port").Text;
            //    }
            //    catch (Exception) { return; }
            //});
            //if (Form1.port != null && Form1.port.Length > 0) {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            //}
        }
    }
}
