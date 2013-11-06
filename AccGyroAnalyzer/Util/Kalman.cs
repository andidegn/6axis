/* Copyright (C) 2012 Kristian Lauszus, TKJ Electronics. All rights reserved.
 
 This software may be distributed and modified under the terms of the GNU
 General Public License version 2 (GPL2) as published by the Free Software
 Foundation and appearing in the file GPL2.TXT included in the packaging of
 this file. Please note that GPL2 Section 2[b] requires that all works based
 on this software must also be made publicly available under the terms of
 the GPL2 ("Copyleft").
 
 Contact information
 -------------------
 
 Kristian Lauszus, TKJ Electronics
 Web      :  http://www.tkjelectronics.com
 e-mail   :  kristianl@tkjelectronics.com
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECTunes.Util {
    public class KalmanAngle {
        /* variables */
        double Q_angle; // Process noise variance for the accelerometer
        double Q_bias; // Process noise variance for the gyro bias
        double R_measure; // Measurement noise variance - this is actually the variance of the measurement noise

        double angle; // The angle calculated by the Kalman filter - part of the 2x1 state matrix
        double bias; // The gyro bias calculated by the Kalman filter - part of the 2x1 state matrix
        double rate; // Unbiased rate calculated from the rate and the calculated bias - you have to call getAngle to update the rate

        double[,] P = new double[2, 2]; // Error covariance matrix - This is a 2x2 matrix
        double[] K = new double[2]; // Kalman gain - This is a 2x1 matrix
        double y; // Angle difference - 1x1 matrix
        double S; // Estimate error - 1x1 matrix


        public KalmanAngle() {

            /* We will set the variables like so, these can also be tuned by the user */
            Reset();

            // Reset bias
            // Since we assume the the bias is 0 and we know the starting angle (use setAngle), 
            //the error covariance matrix is set like so - see: http://en.wikipedia.org/wiki/Kalman_filter#Example_application.2C_technical
            SetDefaults();
        }

        // The angle should be in degrees and the rate should be in degrees per second and the delta time in seconds
        public double getAngle(double newAngle, double newRate, double dt) {
            // KasBot V2  -  Kalman filter module - http://www.x-firm.com/?page_id=145
            // Modified by Kristian Lauszus
            // See my blog post for more information: http://blog.tkjelectronics.dk/2012/09/a-practical-approach-to-kalman-filter-and-how-to-implement-it

            // Discrete Kalman filter time update equations - Time Update ("Predict")
            // Update xhat - Project the state ahead
            /* Step 1 */
            rate = newRate - bias;
            angle += dt * rate;

            // Update estimation error covariance - Project the error covariance ahead
            /* Step 2 */
            P[0, 0] += dt * (dt * P[1, 1] - P[0, 1] - P[1, 0] + Q_angle);
            P[0, 1] -= dt * P[1, 1];
            P[1, 0] -= dt * P[1, 1];
            P[1, 1] += Q_bias * dt;

            // Discrete Kalman filter measurement update equations - Measurement Update ("Correct")
            // Calculate Kalman gain - Compute the Kalman gain
            /* Step 4 */
            S = P[0, 0] + R_measure;
            /* Step 5 */
            K[0] = P[0, 0] / S;
            K[1] = P[1, 0] / S;

            // Calculate angle and bias - Update estimate with measurement zk (newAngle)
            /* Step 3 */
            y = newAngle - angle;
            /* Step 6 */
            angle += K[0] * y;
            bias += K[1] * y;

            // Calculate estimation error covariance - Update the error covariance
            /* Step 7 */
            P[0, 0] -= K[0] * P[0, 0];
            P[0, 1] -= K[0] * P[0, 1];
            P[1, 0] -= K[1] * P[0, 0];
            P[1, 1] -= K[1] * P[0, 1];

            return angle;
        }

        public void setAngle(double newAngle) { angle = newAngle; } // Used to set angle, this should be set as the starting angle
        public double getRate() { return rate; } // Return the unbiased rate

        /* These are used to tune the Kalman filter */
        public void setQangle(double newQ_angle) { Q_angle = newQ_angle; }
        public double getQangle() { return Q_angle; }

        public void setQbias(double newQ_bias) { Q_bias = newQ_bias; }
        public double getQbias() { return Q_bias; }

        public void setRmeasure(double newR_measure) { R_measure = newR_measure; }
        public double getRmeasure() { return R_measure; }


        public void Reset() {
            bias = 0; // Reset bias
            P[0, 0] = 0; // Since we assume tha the bias is 0 and we know the starting angle (use setAngle), the error covariance matrix is set like so - see: http://en.wikipedia.org/wiki/Kalman_filter#Example_application.2C_technical
            P[0, 1] = 0;
            P[1, 0] = 0;
            P[1, 1] = 0;
        }

        public void SetDefaults() {
            /* We will set the varibles like so, these can also be tuned by the user */
            Q_angle = 0.001;
            Q_bias = 0.003;
            R_measure = 0.03;
        }

    }//End of class


    /// <summary>
    /// Kalman 
    ///  
    /// Predict:
    ///   X = F*X + H*U
    ///   P = F*P*F^T + Q.
    /// 
    /// Update:
    ///   Y = M – H*X          Called the innovation = measurement – state transformed by H.	
    ///   S = H*P*H^T + R      S= Residual covariance = covariane transformed by H + R
    ///   K = P * H^T *S^-1    K = Kalman gain = variance / residual covariance.
    ///   X = X + K*Y          Update with gain the new measurement
    ///   P = (I – K * H) * P  Update covariance to this time.
    ///
    /// Note: 
    ///   Derived classes need to perhaps hide certain matrixes to simplify the 
    ///   interface. Also perhaps override: SetupF, Predict, Reset or Update.
    /// </summary>
    public class Kalman {
        #region Protected data.
        /// <summary>
        /// State.
        /// </summary>
        protected Matrix m_x = new Matrix();

        /// <summary>
        /// Covariance.
        /// </summary>
        protected Matrix m_p = new Matrix();

        /// <summary>
        /// Minimal covariance.
        /// </summary>
        protected Matrix m_q = new Matrix();

        /// <summary>
        /// Minimal innovative covariance, keeps filter from locking in to a solution.
        /// </summary>
        protected Matrix m_r = new Matrix();

        /// <summary>
        /// Converts m_x forward to a new time interval.
        /// Depending on what the states mean in m_x this could be anything.
        /// Often for 2D it is: 1, dt
        ///                     0,  1  
        /// Because that would convert a 2 dimentional motion, velocity vector
        /// forward by time dt.
        /// 
        /// For exampe, if acceleration was used instead of velocity 
        /// we could instead have dt*dt instead of dt and thus have a 2nd order
        /// Kalman that estimated position with smooth accelerations.
        /// </summary>
        protected Matrix m_f = new Matrix();

        /// <summary>
        /// Converts measurement vector into state vector space.
        /// </summary>
        protected Matrix m_h = new Matrix();

        /// <summary>
        /// Apriori per update information. For example if we knew something 
        /// moved at a specific velocity always we could just use U to add
        /// that in and decouple that from the other statistics addressed 
        /// by the Kalman filter.
        /// </summary>
        protected Matrix m_u = new Matrix();
        #endregion

        /// <summary>
        /// State.
        /// </summary>
        public Matrix X { get { return m_x; } set { m_x.Set(value); } }

        /// <summary>
        /// Covariance.
        /// </summary>
        public Matrix P { get { return m_p; } set { m_p.Set(value); } }

        /// <summary>
        /// Minimal covariance.
        /// </summary>
        public Matrix Q { get { return m_q; } set { m_q.Set(value); } }

        /// <summary>
        /// Minimal innovative covariance, keeps filter from locking in to a solution.
        /// </summary>
        public Matrix R { get { return m_r; } set { m_r.Set(value); } }

        /// <summary>
        /// Converts m_x forward to a new time interval.
        /// Depending on what the states mean in m_x this could be anything.
        /// Often for 2D it is: 1, dt
        ///                     0,  1  
        /// Because that would convert a 2 dimensional motion, velocity vector
        /// forward by time dt.
        /// 
        /// For example, if acceleration was used instead of velocity 
        /// we could instead have dt*dt instead of dt and thus have a 2nd order
        /// Kalman that estimated position with smooth accelerations.
        /// </summary>
        public Matrix F { get { return m_f; } set { m_f.Set(value); } }

        /// <summary>
        /// Converts measurement vector into state vector space.
        /// </summary>
        public Matrix H { get { return m_h; } set { m_h.Set(value); } }

        /// <summary>
        /// Apriori per update information. For example if we knew something 
        /// moved at a specific velocity always we could just use U to add
        /// that in and decouple that from the other statistics addressed 
        /// by the Kalman filter.
        /// </summary>
        public Matrix U { get { return m_u; } set { m_u.Set(value); } }


        /// <summary>
        /// How fast the value is changing.
        /// </summary>
        public double Value(int index) {
            return m_x.Data[index];
        }

        /// <summary>
        /// The last kalman gain determinant used, useful for debug.
        /// </summary>
        public double LastGain { get; protected set; }

        /// <summary>
        /// Last updated value[0] variance.
        /// </summary>
        /// <returns></returns>
        public double Variance() { return m_p.Data[0]; }

        /// <summary>
        /// Get the covariance of item i to itself.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double Variance(int index) { return m_p.Trace(index); }

        /// <summary>
        /// Setup matrix F based on dt, the time between last update and this update. 
        /// Default is for a rectangular time based:
        ///  1, dt, dt^2, dt^3, ...
        ///  0,  1, dt,   dt^2, ...
        ///  0,  0, 1,    dt, ...
        ///  0,  0, 0,     1, ...
        ///  ...
        /// </summary>
        /// <param name="dt"></param>
        public virtual void SetupF(double dt) {
            m_f.Zero();

            for (int i = 0; i < m_f.Rows; i++) {
                double m = 1;
                m_f.Set(i, i, m);
                for (int j = i + 1; j < m_f.Columns; j++) {
                    m *= dt;
                    m_f.Set(i, i, m);
                }
            }
        }

        /// <summary>
        /// Predict the most significant value forward from 
        /// last measurement time by dt.
        /// X = F*X + H*U        
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public virtual double Predict(double dt) {
            SetupF(dt);
            Matrix tmp = Matrix.Multiply(m_f, m_x);
            Matrix tmp2 = Matrix.Multiply(m_h, m_u);
            tmp.Add(tmp2);
            return tmp.Data[0];
        }

        /// <summary>
        /// Get the estimated covariance of position predicted 
        /// forward from last measurement time by dt.
        /// P = F*P*F^T + Q.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public double Variance(double dt) {
            SetupF(dt);
            Matrix tmp = Matrix.MultiplyABAT(m_f, m_p);
            tmp.Add(m_q);
            return tmp.Data[0];
        }

        /// <summary>
        /// Reset the filter.        
        /// </summary>
        /// <param name="q"></param>
        /// <param name="r"></param>
        /// <param name="p"></param>
        /// <param name="x"></param>
        public virtual void Reset(Matrix q, Matrix r, Matrix p, Matrix x) {
            Q = q;
            R = r;
            P = p;
            X = x;
        }

        /// <summary>
        /// Update the state by measurement m at dt time from last measurement.
        /// </summary>
        /// <param name="measurement"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public virtual double Update(Matrix measurement, double dt) {
            // Predict to now, then update.
            // Predict:
            //   X = F*X + H*U
            //   P = F*P*F^T + Q.
            // Update:
            //   Y = M – H*X          Called the innovation = measurement – state transformed by H.	
            //   S = H*P*H^T + R      S= Residual covariance = covariane transformed by H + R
            //   K = P * H^T *S^-1    K = Kalman gain = variance / residual covariance.
            //   X = X + K*Y          Update with gain the new measurement
            //   P = (I – K * H) * P  Update covariance to this time.
            //
            // Same as 1D but mv is used instead of delta m_x[0], and H = [1,1].

            // X = F*X + H*U
            SetupF(dt);
            Matrix t1 = Matrix.Multiply(m_f, m_x);
            Matrix t2 = Matrix.Multiply(m_h, m_u);
            t1.Add(t2);
            m_x.Set(t1);

            // P = F*P*F^T + Q
            m_p = Matrix.MultiplyABAT(m_f, m_p);
            m_p.Add(m_q);

            // Y = M – H*X  
            t1 = Matrix.Multiply(m_h, m_x);
            Matrix y = Matrix.Subtract(measurement, t1);

            // S = H*P*H^T + R 
            Matrix s = Matrix.MultiplyABAT(m_h, m_p);
            s.Add(m_r);

            // K = P * H^T *S^-1 
            Matrix ht = Matrix.Transpose(m_h);
            Matrix tmp = Matrix.Multiply(m_p, ht);
            Matrix sinv = Matrix.Invert(s);
            Matrix k = new Matrix(y.Rows, m_x.Rows);
            if (sinv != null) {
                k = Matrix.Multiply(tmp, sinv);
            }

            LastGain = k.Determinant;

            // X = X + K*Y
            m_x.Add(Matrix.Multiply(k, y));

            // P = (I – K * H) * P
            Matrix kh = Matrix.Multiply(k, m_h);
            Matrix id = new Matrix(kh.Columns, kh.Rows);
            id.SetIdentity();
            id.Subtract(kh);
            id.Multiply(m_p);
            m_p.Set(id);

            // Return latest estimate.
            return m_x.Data[0];
        }
    }
    /// <summary>
    /// Matrix math class, simple very basic.
    /// </summary>
    public class Matrix {
        #region Protected data.
        /// <summary>
        /// Columns
        /// </summary>
        protected int m_c = 0;

        /// <summary>
        /// Rows.
        /// </summary>
        protected int m_r = 0;
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Matrix() {
            Data = null;
        }

        /// <summary>
        /// Constructor with dimensions.
        /// </summary>
        /// <param name="cols"></param>
        /// <param name="rows"></param>
        public Matrix(int cols, int rows) {
            Resize(cols, rows);
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="m"></param>
        public Matrix(Matrix m) {
            Set(m);
        }

        /// <summary>
        /// Set this matrix to m.
        /// </summary>
        /// <param name="m"></param>
        public void Set(Matrix m) {

            Resize(m.Columns, m.Rows);
            for (int i = 0; i < m.Data.Length; i++) {
                Data[i] = m.Data[i];
            }
        }

        /// <summary>
        /// Columns.
        /// </summary>
        public int Columns {
            get { return m_c; }
            set {
                Resize(value, m_r);
            }
        }

        /// <summary>
        /// Rows
        /// </summary>
        public int Rows {
            get { return m_r; }
            set {
                Resize(m_c, value);
            }
        }

        /// <summary>
        /// Resize.
        /// </summary>
        /// <param name="cols"></param>
        /// <param name="rows"></param>
        public void Resize(int cols, int rows) {
            if ((m_c == cols) && (m_r == rows)) return;
            m_c = cols;
            m_r = rows;
            Data = new double[cols * rows];
            Zero();
        }

        /// <summary>
        /// Clone this matrix.
        /// </summary>
        /// <returns></returns>
        public Matrix Clone() {
            Matrix m = new Matrix();
            m.Resize(this.Columns, this.Rows);
            for (int i = 0; i < Data.Length; i++) {
                m.Data[i] = Data[i];
            }
            return m;
        }

        /// <summary>
        /// Get a value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public double Get(int x, int y) {
            return Data[x + y * m_c];
        }


        /// <summary>
        /// Return the trace, same as Get(index, index);
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double Trace(int index) {
            return Get(index, index);
        }

        /// <summary>
        /// Set a value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public void Set(int x, int y, double v) {
            Data[x + (y * m_c)] = v;
        }

        /// <summary>
        /// The raw data.
        /// </summary>
        public double[] Data { get; set; }

        /// <summary>
        /// In place scalar multiplication.
        /// this *= scalar.
        /// </summary>
        /// <param name="scalar"></param>
        public void Multiply(double scalar) {
            for (int i = 0; i < Data.Length; i++) {
                Data[i] *= scalar;
            }
        }

        /// <summary>
        /// Scalar multiplication.
        /// result = Multiply(matrix, scalar);
        /// </summary>
        /// <param name="scalar"></param>
        public static Matrix Multiply(Matrix m, double scalar) {
            Matrix rv = m.Clone();
            rv.Multiply(scalar);
            return rv;
        }

        /// <summary>
        /// Multiply two matrixes.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix Multiply(Matrix a, Matrix b) {
            Matrix rv = new Matrix(b.Columns, a.Rows);
            int min = a.Columns < b.Rows ? a.Columns : b.Rows;
            for (int i = 0; i < a.Rows; i++) {
                for (int j = 0; j < b.Columns; j++) {
                    double s = 0;
                    for (int k = 0; k < min; k++) {
                        double av = a.Get(k, i);
                        double bv = b.Get(j, k);
                        s += av * bv;
                    }
                    rv.Set(j, i, s);
                }
            }
            return rv;
        }

        /// <summary>
        /// Multiply in place this * b.
        /// </summary>
        /// <param name="b"></param>
        public void Multiply(Matrix b) {
            Matrix tmp = Matrix.Multiply(this, b);
            this.Set(tmp);
        }

        /// <summary>
        /// Result = a*b*a^T.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix MultiplyABAT(Matrix a, Matrix b) {
            Matrix rv = Multiply(a, b);
            Matrix t = Matrix.Transpose(a);
            rv.Multiply(t);
            return rv;
        }


        /// <summary>
        /// Add scalar.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Matrix Add(Matrix a, double scalar) {
            Matrix rv = new Matrix(a);
            rv.Add(scalar);
            return rv;
        }

        /// <summary>
        /// Add scalar in place
        /// </summary>
        /// <param name="a"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public void Add(double scalar) {
            for (int i = 0; i < Data.Length; i++) {
                Data[i] += scalar;
            }
        }

        /// <summary>
        /// Add matrix.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Matrix Add(Matrix a, Matrix b) {
            Matrix rv = new Matrix(a);
            rv.Add(b);
            return rv;
        }

        /// <summary>
        /// Add matrix in place
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public void Add(Matrix a) {
            for (int i = 0; i < Data.Length; i++) {
                Data[i] += a.Data[i];
            }
        }

        /// <summary>
        /// Add scalar.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Matrix Subtract(Matrix a, double scalar) {
            Matrix rv = new Matrix(a);
            rv.Subtract(scalar);
            return rv;
        }

        /// <summary>
        /// Add scalar in place
        /// </summary>
        /// <param name="a"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public void Subtract(double scalar) {
            for (int i = 0; i < Data.Length; i++) {
                Data[i] -= scalar;
            }
        }

        /// <summary>
        /// Add matrix.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Matrix Subtract(Matrix a, Matrix b) {
            Matrix rv = new Matrix(a);
            rv.Subtract(b);
            return rv;
        }

        /// <summary>
        /// Add matrix in place
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public void Subtract(Matrix a) {
            for (int i = 0; i < Data.Length; i++) {
                Data[i] -= a.Data[i];
            }
        }


        /// <summary>
        /// Transpose matrix m.
        /// </summary>
        public static Matrix Transpose(Matrix m) {
            Matrix rv = new Matrix(m.m_r, m.m_c);
            for (int i = 0; i < m.m_c; i++) {
                for (int j = 0; j < m.m_r; j++) {
                    rv.Set(j, i, m.Get(i, j));
                }
            }
            return rv;
        }

        /// <summary>
        /// Transpose this matrix in place.
        /// </summary>
        public void Transpose() {
            Matrix rv = new Matrix(this.m_r, this.m_c);
            for (int i = 0; i < m_c; i++) {
                for (int j = 0; j < m_r; j++) {
                    rv.Set(j, i, this.Get(i, j));
                }
            }
            this.Set(rv);
        }

        /// <summary>
        /// Test if this is an identity matrix.
        /// </summary>
        /// <returns></returns>
        public bool IsIdentity() {
            if (m_c != m_r) return false;
            int check = m_c + 1;
            int j = 0;
            for (int i = 0; i < Data.Length; i++) {
                if (j == check) {
                    j = 0;
                    if (Data[i] != 1) return false;
                }
                else {
                    if (Data[i] != 0) return false;
                }
                j++;
            }
            return true;
        }

        /// <summary>
        /// Test if this is an identity matrix.
        /// </summary>
        /// <returns></returns>
        public void SetIdentity() {
            if (m_c != m_r) return;
            int check = m_c + 1;
            int j = 0;
            for (int i = 0; i < Data.Length; i++) {
                Data[i] = (j == check) ? 1 : 0;
                j = j == check ? 1 : j + 1;
            }
        }

        /// <summary>
        /// Zero.
        /// </summary>
        public void Zero() {
            for (int i = 0; i < Data.Length; i++) {
                Data[i] = 0;
            }
        }

        /// <summary>
        /// Determinant.
        /// </summary>
        public double Determinant {
            get {
                if (m_c != m_r) return 0;

                if (m_c == 0) return 0;
                if (m_c == 1) return Data[0];
                if (m_c == 2) return (Data[0] * Data[3]) - (Data[1] * Data[2]);
                if (m_c == 3) return
                    (Data[0] * ((Data[8] * Data[4]) - (Data[7] * Data[5]))) -
                    (Data[3] * ((Data[8] * Data[1]) - (Data[7] * Data[2]))) +
                    (Data[6] * ((Data[5] * Data[1]) - (Data[4] * Data[2])));

                // only supporting 1x1, 2x2 and 3x3
                return 0;
            }
        }

        /// <summary>
        /// Invert.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Matrix Invert(Matrix m) {
            if (m.m_c != m.m_r) return null;
            double det = m.Determinant;
            if (det == 0) return null;

            Matrix rv = new Matrix(m);
            if (m.m_c == 1) rv.Data[0] = 1 / rv.Data[0];
            det = 1 / det;
            if (m.m_c == 2) {
                rv.Data[0] = det * m.Data[3];
                rv.Data[3] = det * m.Data[0];
                rv.Data[1] = -det * m.Data[2];
                rv.Data[2] = -det * m.Data[1];
            }
            if (m.m_c == 3) {
                rv.Data[0] = det * (m.Data[8] * m.Data[4]) - (m.Data[7] * m.Data[5]);
                rv.Data[1] = -det * (m.Data[8] * m.Data[1]) - (m.Data[7] * m.Data[2]);
                rv.Data[2] = det * (m.Data[5] * m.Data[1]) - (m.Data[4] * m.Data[2]);

                rv.Data[3] = -det * (m.Data[8] * m.Data[3]) - (m.Data[6] * m.Data[5]);
                rv.Data[4] = det * (m.Data[8] * m.Data[0]) - (m.Data[6] * m.Data[2]);
                rv.Data[5] = -det * (m.Data[5] * m.Data[0]) - (m.Data[3] * m.Data[2]);

                rv.Data[6] = det * (m.Data[7] * m.Data[3]) - (m.Data[6] * m.Data[4]);
                rv.Data[7] = -det * (m.Data[7] * m.Data[0]) - (m.Data[6] * m.Data[2]);
                rv.Data[8] = det * (m.Data[4] * m.Data[0]) - (m.Data[3] * m.Data[1]);
            }
            return rv;
        }
    }
    /// <summary>
    /// Kalman 1D w/ velocity estimation.
    /// </summary>
    public class Kalman1D {
        #region Protected data.
        /// <summary>
        /// State.
        /// </summary>
        double[] m_x = new double[2];
        /// <summary>
        /// Covariance.
        /// </summary>
        double[] m_p = new double[4];

        /// <summary>
        /// Minimal covariance.
        /// </summary>
        double[] m_q = new double[4];

        /// <summary>
        /// Minimal innovative covariance, keeps filter from locking in to a solution.
        /// </summary>
        double m_r;
        #endregion

        /// <summary>
        /// The last updated value, can also be set if filter gets
        /// sudden absolute measurement data for the latest update.
        /// </summary>
        public double Value {
            get { return m_x[0]; }
            set { m_x[0] = value; }
        }

        /// <summary>
        /// How fast the value is changing.
        /// </summary>
        public double Velocity {
            get { return m_x[1]; }
        }

        /// <summary>
        /// The last kalman gain used, useful for debug.
        /// </summary>
        public double LastGain { get; protected set; }

        /// <summary>
        /// Last updated positional variance.
        /// </summary>
        /// <returns></returns>
        public double Variance() { return m_p[0]; }

        /// <summary>
        /// Predict the value forward from last measurement time by dt.
        /// X = F*X + H*U        
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public double Predicition(double dt) {
            return m_x[0] + (dt * m_x[1]);
        }

        /// <summary>
        /// Get the estimated covariance of position predicted 
        /// forward from last measurement time by dt.
        /// P = F*X*F^T + Q.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public double Variance(double dt) {
            return m_p[0] + dt * (m_p[2] + m_p[1]) + dt * dt * m_p[3] + m_q[0];
            // Not needed.
            // m_p[1] = m_p[1] + dt * m_p[3] + m_q[1];
            // m_p[2] = m_p[2] + dt * m_p[3] + m_q[2];
            // m_p[3] = m_p[3] + m_q[3];
        }

        /// <summary>
        /// Reset the filter.
        /// </summary>
        /// <param name="qx">Measurement to position state minimal variance.</param>
        /// <param name="qv">Measurement to velocity state minimal variance.</param>
        /// <param name="r">Measurement covariance (sets minimal gain).</param>
        /// <param name="pd">Initial variance.</param>
        /// <param name="ix">Initial position.</param>
        public void Reset(double qx, double qv, double r, double pd, double ix) {
            m_q[0] = qx; m_q[1] = qv;
            m_r = r;
            m_p[0] = m_p[3] = pd;
            m_p[1] = m_p[2] = 0;
            m_x[0] = ix;
            m_x[1] = 0;
        }

        /// <summary>
        /// Update the state by measurement m at dt time from last measurement.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public double Update(double m, double dt) {
            // Predict to now, then update.
            // Predict:
            //   X = F*X + H*U
            //   P = F*X*F^T + Q.
            // Update:
            //   Y = M – H*X          Called the innovation = measurement – state transformed by H.	
            //   S = H*P*H^T + R      S= Residual covariance = covariane transformed by H + R
            //   K = P * H^T *S^-1    K = Kalman gain = variance / residual covariance.
            //   X = X + K*Y          Update with gain the new measurement
            //   P = (I – K * H) * P  Update covariance to this time.

            // X = F*X + H*U
            double oldX = m_x[0];
            m_x[0] = m_x[0] + (dt * m_x[1]);

            // P = F*X*F^T + Q
            m_p[0] = m_p[0] + dt * (m_p[2] + m_p[1]) + dt * dt * m_p[3] + m_q[0];
            m_p[1] = m_p[1] + dt * m_p[3] + m_q[1];
            m_p[2] = m_p[2] + dt * m_p[3] + m_q[2];
            m_p[3] = m_p[3] + m_q[3];

            // Y = M – H*X  
            double y0 = m - m_x[0];
            double y1 = ((m - oldX) / dt) - m_x[1];

            // S = H*P*H^T + R 
            // Because H = [1, 0] this is easy, and s is a single value not a matrix to invert.
            double s = m_p[0] + m_r;

            // K = P * H^T *S^-1 
            double k = m_p[0] / s;
            LastGain = k;

            // X = X + K*Y
            m_x[0] += y0 * k;
            m_x[1] += y1 * k;

            // P = (I – K * H) * P
            for (int i = 0; i < 4; i++) m_p[i] = m_p[i] - k * m_p[i];

            // Return latest estimate.
            return m_x[0];
        }
    }
    /// <summary>
    /// Kalman 2D.
    /// </summary>
    public class Kalman2D {
        #region Protected data.
        /// <summary>
        /// State.
        /// </summary>
        Matrix m_x = new Matrix(1, 2);
        /// <summary>
        /// Covariance.
        /// </summary>
        Matrix m_p = new Matrix(2, 2);
        /// <summary>
        /// Minimal covariance.
        /// </summary>
        Matrix m_q = new Matrix(2, 2);

        /// <summary>
        /// Minimal innovative covariance, keeps filter from locking in to a solution.
        /// </summary>
        double m_r;
        #endregion

        /// <summary>
        /// The last updated value, can also be set if filter gets
        /// sudden absolute measurement data for the latest update.
        /// </summary>
        public double Value {
            get { return m_x.Data[0]; }
            set { m_x.Data[0] = value; }
        }

        /// <summary>
        /// How fast the value is changing.
        /// </summary>
        public double Velocity {
            get { return m_x.Data[1]; }
        }

        /// <summary>
        /// The last kalman gain used, useful for debug.
        /// </summary>
        public double LastGain { get; protected set; }

        /// <summary>
        /// Last updated positional variance.
        /// </summary>
        /// <returns></returns>
        public double Variance() { return m_p.Data[0]; }

        /// <summary>
        /// Predict the value forward from last measurement time by dt.
        /// X = F*X + H*U        
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public double Predicition(double dt) {
            return m_x.Data[0] + (dt * m_x.Data[1]);
        }

        /// <summary>
        /// Get the estimated covariance of position predicted 
        /// forward from last measurement time by dt.
        /// P = F*P*F^T + Q.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public double Variance(double dt) {
            return m_p.Data[0] + dt * (m_p.Data[2] + m_p.Data[1]) + dt * dt * m_p.Data[3] + m_q.Data[0];
            // Not needed.
            // m_p[1] = m_p[1] + dt * m_p[3] + m_q[1];
            // m_p[2] = m_p[2] + dt * m_p[3] + m_q[2];
            // m_p[3] = m_p[3] + m_q[3];
        }

        /// <summary>
        /// Reset the filter.
        /// </summary>
        /// <param name="qx">Measurement to position state minimal variance.</param>
        /// <param name="qv">Measurement to velocity state minimal variance.</param>
        /// <param name="r">Measurement covariance (sets minimal gain).</param>
        /// <param name="pd">Initial variance.</param>
        /// <param name="ix">Initial position.</param>
        public void Reset(double qx, double qv, double r, double pd, double ix) {
            m_q.Data[0] = qx * qx;
            m_q.Data[1] = qv * qx;
            m_q.Data[2] = qv * qx;
            m_q.Data[3] = qv * qv;

            m_r = r;
            m_p.Data[0] = m_p.Data[3] = pd;
            m_p.Data[1] = m_p.Data[2] = 0;
            m_x.Data[0] = ix;
            m_x.Data[1] = 0;
        }

        /// <summary>
        /// Update the state by measurement m at dt time from last measurement.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public double Update(double mx, double mv, double dt) {
            // Predict to now, then update.
            // Predict:
            //   X = F*X + H*U
            //   P = F*P*F^T + Q.
            // Update:
            //   Y = M – H*X          Called the innovation = measurement – state transformed by H.	
            //   S = H*P*H^T + R      S= Residual covariance = covariane transformed by H + R
            //   K = P * H^T *S^-1    K = Kalman gain = variance / residual covariance.
            //   X = X + K*Y          Update with gain the new measurement
            //   P = (I – K * H) * P  Update covariance to this time.
            //
            // Same as 1D but mv is used instead of delta m_x[0], and H = [1,1].

            // X = F*X + H*U
            Matrix f = new Matrix(2, 2) { Data = new double[] { 1, dt, 0, 1 } };
            Matrix h = new Matrix(2, 2) { Data = new double[] { 1, 0, 0, 1 } };
            Matrix ht = new Matrix(2, 2) { Data = new double[] { 1, 0, 0, 1 } };
            // U = {0,0};
            m_x = Matrix.Multiply(f, m_x);

            // P = F*P*F^T + Q
            m_p = Matrix.MultiplyABAT(f, m_p);
            m_p.Add(m_q);

            // Y = M – H*X  
            Matrix y = new Matrix(1, 2) { Data = new double[] { mx - m_x.Data[0], mv - m_x.Data[1] } };

            // S = H*P*H^T + R 
            Matrix s = Matrix.MultiplyABAT(h, m_p);
            s.Data[0] += m_r;
            s.Data[3] += m_r * 0.1;

            // K = P * H^T *S^-1 
            Matrix tmp = Matrix.Multiply(m_p, ht);
            Matrix sinv = Matrix.Invert(s);
            Matrix k = new Matrix(2, 2); // inited to zero.

            if (sinv != null) {
                k = Matrix.Multiply(tmp, sinv);
            }

            LastGain = k.Determinant;

            // X = X + K*Y
            m_x.Add(Matrix.Multiply(k, y));

            // P = (I – K * H) * P
            Matrix kh = Matrix.Multiply(k, h);
            Matrix id = new Matrix(2, 2) { Data = new double[] { 1, 0, 0, 1 } };
            kh.Multiply(-1);
            id.Add(kh);
            id.Multiply(m_p);
            m_p.Set(id);


            // Return latest estimate.
            return m_x.Data[0];
        }


    }
}
