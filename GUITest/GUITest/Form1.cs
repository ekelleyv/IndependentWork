using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;

namespace MotionPlannerGUI
{
    public partial class Form1 : Form
    {
        Button abortButton = new Button();
        Thread t;
        List<Point> point_list = new List<Point>();


        public Form1()
        {
            InitializeComponent();

            Controls.Add(abortButton);
            abortButton.Text = "abort";
            abortButton.Location = new Point(10, 10);
            abortButton.Click += new EventHandler(Abort_Click);

            point_list.Add(new Point(50, 50));

            t = new Thread(new ThreadStart(RunThread));
            t.Start();
            
        }

        public void RunThread()
        {
            while (true)
            {
                Point old_point = point_list[point_list.Count - 1];
                point_list.Add(new Point(old_point.X + 10, old_point.Y + 10));
                Thread.Sleep(1000);
                Invalidate();
            }

        }

        protected void Abort_Click(object sender, EventArgs e)
        {
            t.Abort();
            Close();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen redPen = new Pen(Color.Red, 2);
            for (int i = 0; i < point_list.Count; i++)
            {
                g.DrawRectangle(redPen, point_list[i].X, point_list[i].Y, 2, 2);

            }
            base.OnPaint(e);
        }


    }
}
