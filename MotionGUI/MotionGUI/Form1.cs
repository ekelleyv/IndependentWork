using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MotionGUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Random rdn = new Random();
            for (int i = 0; i < 50; i++)
            {
                chart1.Series["test1"].Points.AddXY
                                (rdn.Next(0, 10), rdn.Next(0, 10));
                chart1.Series["test2"].Points.AddXY
                                (rdn.Next(0, 10), rdn.Next(0, 10));
            }

            chart1.Series["test1"].ChartType =
                                System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            chart1.Series["test1"].Color = Color.Red;

            chart1.Series["test2"].ChartType =
                                System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            chart1.Series["test2"].Color = Color.Blue;

        }
    }
}
