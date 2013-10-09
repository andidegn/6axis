using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace ECTunes.view {
    class ChartControl {


        public static void ChartSetup(Chart chart, String seriesName, int borderWidth, Color color, SeriesChartType chartType, ChartValueType xValueType) {
            Series newSeries = new Series(seriesName);
            newSeries.ChartType = chartType;
            newSeries.BorderWidth = borderWidth;
            newSeries.Color = color;
            newSeries.XValueType = xValueType;
            chart.Series.Add(newSeries);

        }

        //public static void AddNewPoint(Chart chart, String seriesName, double x, double y) {
        //    ChartControl.chart = chart;
        //    ChartControl.seriesName = seriesName;
        //    ChartControl.x = x;
        //    ChartControl.y = y;

        //    Invoke(new EventHandler(AddNewPointEH));

        //    //chart.Series[seriesName].Points.AddXY((double)x, (double)y);
        //    //chart.Series["y_axis"].Points.AddXY((double)x, (double)y2);
        //    //chart.Series["z_axis"].Points.AddXY((double)x, (double)y3);
        //    //chart.Series["Ra"].Points.AddXY((double)x, (double)y4);
        //    //            chart1.Series["Velocity"].Points.AddXY((double)x, (double)V);
        //    //chart1.Series["sign"].Points.AddXY((double)x, (double)sig);

        //}

        //private static void AddNewPointEH() {
        //    chart.Series[seriesName].Points.AddXY((double)x, (double)y);
        //}

    }
}
