using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PCGraphic
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            trackBar1.Value = 180;
            trackBar2.Value = 125;
            trackBar3.Value = 180;
        }

        Graph graph = new Graph();
        PictureBox p;
        List<Point3D> points3D = new List<Point3D>();
        List<Point> points = new List<Point>();
        List<Point> picturepoints = new List<Point>();
        List<Point>[] bezpoints;

        double Alf = 180 * Math.PI / 180;
        double Bet = 125 * Math.PI / 180;
        double Gam = 180 * Math.PI / 180;
        double MovX = 0, MovY = 0;
        double[,] Wmatr;
        int AnimI;
        int PicPosX, PicPosY=-24;
        bool IsTrue;

        private void GraphPanel_Paint(object sender, PaintEventArgs e)
        {
            p = sender as PictureBox;
            Graphics g = e.Graphics;
            double Rx = (textBox1.Text == "") ? 10 : Convert.ToInt32(textBox1.Text);
            double Ry = (textBox2.Text == "") ? 10 : Convert.ToInt32(textBox2.Text);
            points.Clear();
            picturepoints.Clear();
            points3D.Clear();

            for (double j = 0; j < 22; j++)
            {
                for (double i = 0; i < 360; i += 15)
                {
                    points3D.Add(new Point3D((float)(0 + Rx * j * Math.Cos(i * Math.PI / 180)), (float)(0 + Ry * j * Math.Sin(i * Math.PI / 180)), (float)(j * j)));
                }
            }

            #region 3D Matrix
            Wmatr = graph.Multiplication(
                    graph.Multiplication(new double[,]{
                    {1,                0,              0              },
                    {0,                Math.Cos(Bet), -Math.Sin(Bet)  },
                    {0,                Math.Sin(Bet),  Math.Cos(Bet)  }},
                    new double[,]{
                    {Math.Cos(Alf),    0,              Math.Sin(Alf)  },
                    {0,                1,              0              },
                    {-Math.Sin(Alf),   0,              Math.Cos(Alf)  }}),
                new double[,]{
                {Math.Cos(Gam),     -Math.Sin(Gam),  0              },
                {Math.Sin(Gam),      Math.Cos(Gam),  0              },
                {0,                  0,              1              }});

            for (int i = 0; i < points3D.Count; i++)
            {
                double[,] Rmatr = graph.Multiplication(new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 0 } }, graph.Multiplication(Wmatr, new double[,] { { points3D[i].X }, { points3D[i].Y }, { points3D[i].Z } }));
                points3D[i] = new Point3D((float)Rmatr[0, 0] + 350, (float)Rmatr[1, 0] + 70, 0);
            }
            #endregion

            for (int i = 24; i < points3D.Count; i++)
            {
                graph.Line(points, points3D[i - 24].X, points3D[i - 24].Y, points3D[i].X, points3D[i].Y);
            }

            for (int i = 23; i < points3D.Count; i++)
            {
                if ((i - 23) % 24 != 0) graph.Line(points, points3D[i].X, points3D[i].Y, points3D[i + 1].X, points3D[i + 1].Y);
                else graph.Line(points, points3D[i].X, points3D[i].Y, points3D[i - 23].X, points3D[i - 23].Y);
            }

            if (!IsTrue)
            {
                #region Default points of Bezier
                bezpoints = new List<Point>[20]
                {
                    new List<Point>() { new Point((int)points3D[242+PicPosX+PicPosY].X, (int)points3D[242+PicPosX+PicPosY].Y), new Point((int)points3D[218+PicPosX+PicPosY].X, (int)points3D[218+PicPosX+PicPosY].Y), new Point((int)points3D[216+PicPosX+PicPosY].X, (int)points3D[216+PicPosX+PicPosY].Y) },
                    new List<Point>() { new Point((int)points3D[216+PicPosX+PicPosY].X, (int)points3D[216+PicPosX+PicPosY].Y), new Point((int)points3D[261+PicPosX+PicPosY].X, (int)points3D[261+PicPosX+PicPosY].Y), new Point((int)points3D[261+PicPosX+PicPosY].X, (int)points3D[261+PicPosX+PicPosY].Y) },
                    new List<Point>() { new Point((int)points3D[237+PicPosX+PicPosY].X, (int)points3D[237+PicPosX+PicPosY].Y), new Point((int)points3D[260+PicPosX+PicPosY].X, (int)points3D[260+PicPosX+PicPosY].Y), new Point((int)points3D[261+PicPosX+PicPosY].X, (int)points3D[261+PicPosX+PicPosY].Y) },
                    new List<Point>() { new Point((int)points3D[215+PicPosX+PicPosY].X, (int)points3D[215+PicPosX+PicPosY].Y), new Point((int)points3D[237+PicPosX+PicPosY].X, (int)points3D[237+PicPosX+PicPosY].Y), new Point((int)points3D[237+PicPosX+PicPosY].X, (int)points3D[237+PicPosX+PicPosY].Y) },
                    new List<Point>() { new Point((int)points3D[168+PicPosX+PicPosY].X, (int)points3D[168+PicPosX+PicPosY].Y), new Point((int)points3D[167+PicPosX+PicPosY].X, (int)points3D[167+PicPosX+PicPosY].Y), new Point((int)points3D[215+PicPosX+PicPosY].X, (int)points3D[215+PicPosX+PicPosY].Y) },
                    new List<Point>() { new Point((int)points3D[171+PicPosX+PicPosY].X, (int)points3D[171+PicPosX+PicPosY].Y), new Point((int)points3D[146+PicPosX+PicPosY].X, (int)points3D[146+PicPosX+PicPosY].Y), new Point((int)points3D[168+PicPosX+PicPosY].X, (int)points3D[168+PicPosX+PicPosY].Y) },
                    new List<Point>() { new Point((int)points3D[363+PicPosX+PicPosY].X, (int)points3D[363+PicPosX+PicPosY].Y), new Point((int)points3D[245+PicPosX+PicPosY].X, (int)points3D[245+PicPosX+PicPosY].Y), new Point((int)points3D[171+PicPosX+PicPosY].X, (int)points3D[171+PicPosX+PicPosY].Y) },
                    new List<Point>() { new Point((int)points3D[393+PicPosX+PicPosY].X, (int)points3D[393+PicPosX+PicPosY].Y), new Point((int)points3D[343+PicPosX+PicPosY].X, (int)points3D[343+PicPosX+PicPosY].Y), new Point((int)points3D[363+PicPosX+PicPosY].X, (int)points3D[363+PicPosX+PicPosY].Y) },
                    new List<Point>() { new Point((int)points3D[393+PicPosX+PicPosY].X, (int)points3D[393+PicPosX+PicPosY].Y), new Point((int)points3D[418+PicPosX+PicPosY].X, (int)points3D[418+PicPosX+PicPosY].Y), new Point((int)points3D[419+PicPosX+PicPosY].X, (int)points3D[419+PicPosX+PicPosY].Y) },
                    new List<Point>() { new Point((int)points3D[419+PicPosX+PicPosY].X, (int)points3D[419+PicPosX+PicPosY].Y), new Point((int)points3D[420+PicPosX+PicPosY].X, (int)points3D[420+PicPosX+PicPosY].Y), new Point((int)points3D[397+PicPosX+PicPosY].X, (int)points3D[397+PicPosX+PicPosY].Y) },
                    new List<Point>() { new Point((int)points3D[397+PicPosX+PicPosY].X, (int)points3D[397+PicPosX+PicPosY].Y), new Point((int)points3D[422+PicPosX+PicPosY].X, (int)points3D[422+PicPosX+PicPosY].Y), new Point((int)points3D[421+PicPosX+PicPosY].X, (int)points3D[421+PicPosX+PicPosY].Y) },
                    new List<Point>() { new Point((int)points3D[421+PicPosX+PicPosY].X, (int)points3D[421+PicPosX+PicPosY].Y), new Point((int)points3D[445+PicPosX+PicPosY].X, (int)points3D[445+PicPosX+PicPosY].Y), new Point((int)points3D[444+PicPosX+PicPosY].X, (int)points3D[444+PicPosX+PicPosY].Y) },
                    new List<Point>() { new Point((int)points3D[444+PicPosX+PicPosY].X, (int)points3D[444+PicPosX+PicPosY].Y), new Point((int)points3D[468+PicPosX+PicPosY].X, (int)points3D[468+PicPosX+PicPosY].Y), new Point((int)points3D[466+PicPosX+PicPosY].X, (int)points3D[466+PicPosX+PicPosY].Y) },
                    new List<Point>() { new Point((int)points3D[466+PicPosX+PicPosY].X, (int)points3D[466+PicPosX+PicPosY].Y), new Point((int)points3D[489+PicPosX+PicPosY].X, (int)points3D[489+PicPosX+PicPosY].Y), new Point((int)points3D[488+PicPosX+PicPosY].X, (int)points3D[488+PicPosX+PicPosY].Y) },
                    new List<Point>() { new Point((int)points3D[488+PicPosX+PicPosY].X, (int)points3D[488+PicPosX+PicPosY].Y), new Point((int)points3D[511+PicPosX+PicPosY].X, (int)points3D[511+PicPosX+PicPosY].Y), new Point((int)points3D[486+PicPosX+PicPosY].X, (int)points3D[486+PicPosX+PicPosY].Y) },
                    new List<Point>() { new Point((int)points3D[486+PicPosX+PicPosY].X, (int)points3D[486+PicPosX+PicPosY].Y), new Point((int)points3D[510+PicPosX+PicPosY].X, (int)points3D[510+PicPosX+PicPosY].Y), new Point((int)points3D[485+PicPosX+PicPosY].X, (int)points3D[485+PicPosX+PicPosY].Y) },
                    new List<Point>() { new Point((int)points3D[485+PicPosX+PicPosY].X, (int)points3D[485+PicPosX+PicPosY].Y), new Point((int)points3D[508+PicPosX+PicPosY].X, (int)points3D[508+PicPosX+PicPosY].Y), new Point((int)points3D[483+PicPosX+PicPosY].X, (int)points3D[483+PicPosX+PicPosY].Y) },
                    new List<Point>() { new Point((int)points3D[483+PicPosX+PicPosY].X, (int)points3D[483+PicPosX+PicPosY].Y), new Point((int)points3D[506+PicPosX+PicPosY].X, (int)points3D[506+PicPosX+PicPosY].Y), new Point((int)points3D[457+PicPosX+PicPosY].X, (int)points3D[457+PicPosX+PicPosY].Y) },
                    new List<Point>() { new Point((int)points3D[457+PicPosX+PicPosY].X, (int)points3D[457+PicPosX+PicPosY].Y), new Point((int)points3D[455+PicPosX+PicPosY].X, (int)points3D[455+PicPosX+PicPosY].Y), new Point((int)points3D[361+PicPosX+PicPosY].X, (int)points3D[361+PicPosX+PicPosY].Y) },
                    new List<Point>() { new Point((int)points3D[361+PicPosX+PicPosY].X, (int)points3D[361+PicPosX+PicPosY].Y), new Point((int)points3D[338+PicPosX+PicPosY].X, (int)points3D[338+PicPosX+PicPosY].Y), new Point((int)points3D[242+PicPosX+PicPosY].X, (int)points3D[242+PicPosX+PicPosY].Y) }
                };
                #endregion

                for (int i = 0; i < points.Count; i++)
                {
                    g.DrawRectangle(new Pen(Color.Red, 1),
                            (float)(points[i].X + MovX),
                            (float)(points[i].Y + MovY),
                            1, 1);
                }

                if (checkBox1.Checked)
                {
                    for (int i = 0; i < bezpoints.Length; i++)
                        Bezpoints(bezpoints[i][0], bezpoints[i][1], bezpoints[i][2]);

                    for (int i = 0; i < picturepoints.Count; i++)
                    {
                        g.DrawRectangle(new Pen(Color.Blue, 1),
                                (float)(picturepoints[i].X + MovX),
                                (float)(picturepoints[i].Y + MovY),
                                1, 1);
                    }
                }
            }
            else
            {
                for (int i = 0; i < AnimI; i++)
                    g.DrawRectangle(new Pen(Color.Red, 1),
                                (float)(points[i].X + MovX),
                                (float)(points[i].Y + MovY),
                                1, 1);
                AnimI += 150;
                if (AnimI>points.Count)
                {
                    timer1.Stop();
                    IsTrue = false;
                }
            }

        }


        private void Bezpoints(Point p0, Point p1, Point p2)
        {
            Point pt = new Point();
            for (int i = 0; i <= 500; ++i)
            {
                double t = i / 500.0;
                pt = graph.BezierPoint(t, p0, p1, p2);
                picturepoints.Add(pt);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IsTrue = (textBox1.Text == "" && textBox2.Text == "") ? true : false;
            if (IsTrue)
            {
                trackBar1.Value = 180;
                trackBar2.Value = 125;
                trackBar3.Value = 180;
                Alf = 180 * Math.PI / 180;
                Bet = 125 * Math.PI / 180;
                Gam = 180 * Math.PI / 180;
                AnimI = 0;
                checkBox1.Checked = false;
                timer1.Start();
            }
            else GraphPanel.Refresh();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            Alf = trackBar1.Value * Math.PI / 180;
            GraphPanel.Refresh();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            Bet = trackBar2.Value * Math.PI / 180;
            GraphPanel.Refresh();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            Gam = trackBar3.Value * Math.PI / 180;
            GraphPanel.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MovY -= 10;
            GraphPanel.Refresh();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MovX += 10;
            GraphPanel.Refresh();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MovY += 10;
            GraphPanel.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MovX -= 10;
            GraphPanel.Refresh();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            button6.Enabled = button7.Enabled = button8.Enabled = button9.Enabled = (checkBox1.Checked)?true:false;
            GraphPanel.Refresh();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            PicPosX = (PicPosX == 0) ? 23 : --PicPosX;
            GraphPanel.Refresh();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            PicPosY += 24;
            if (PicPosY == 0) button7.Enabled = false;
            if (PicPosY != -144) button9.Enabled = true;
            GraphPanel.Refresh();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            PicPosX = (PicPosX + 1 == 24) ? 0 : ++PicPosX;
            GraphPanel.Refresh();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            PicPosY -= 24;
            if (PicPosY != 0) button7.Enabled = true;
            if (PicPosY == -144) button9.Enabled = false;
            GraphPanel.Refresh();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            GraphPanel.Refresh();
        }
    }
}