using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PCGraphic
{
    class Graph
    {
        public int GetLength(double Rx, double Ry, double R, double Rh)
        {
            int L;
            float x1, y1, x2, y2, x3, y3, xr, yr;

            x1 = (float)(Rx + (R + Rh) * Math.Cos(-45 * Math.PI / 180));
            y1 = (float)(Ry + (R + Rh) * Math.Sin(-45 * Math.PI / 180));
            xr = x3 = (float)(x1 + Rh * Math.Cos(-135 * Math.PI / 180));
            yr = y3 = (float)(y1 + Rh * Math.Sin(-135 * Math.PI / 180));
            x1 = (float)(Rx + (R + Rh) * Math.Cos(-135 * Math.PI / 180));
            y1 = (float)(Ry + (R + Rh) * Math.Sin(-135 * Math.PI / 180));
            x2 = (float)(x1 + Rh * Math.Cos(-45 * Math.PI / 180));
            y2 = (float)(y1 + Rh * Math.Sin(-45 * Math.PI / 180));

            L = (int)Math.Sqrt(Math.Pow(y2 - yr, 2) + Math.Pow(x2 - xr, 2));
            return L;
        }

        public void IsBeyondScreen(Panel p, double Rx, ref double Ry, double R, double Rh, double L)
        {
            //if (Ry - 40 - (2 * L / Math.Sqrt(2) + Rh * 2) * (Math.Sqrt(2) / 2) < 0) Ry = p.Height-Ry;
            if (Ry - 40 - (2 * L / Math.Sqrt(2) + Rh * 2) * (Math.Sqrt(2) / 2) < 0)
            {
                p.Height += ((int)Math.Abs(Ry - 40 - (2 * L / Math.Sqrt(2) + Rh * 2) * (Math.Sqrt(2) / 2))) - ((int)Math.Abs(Ry - 40 - (2 * L / Math.Sqrt(2) + Rh * 2) * (Math.Sqrt(2) / 2))) % 20;
                Ry += (Math.Abs(Ry - 40 - (2 * L / Math.Sqrt(2) + Rh * 2) * (Math.Sqrt(2) / 2))) - (Math.Abs(Ry - 40 - (2 * L / Math.Sqrt(2) + Rh * 2) * (Math.Sqrt(2) / 2))) % 20;
            }
            //if ((Rx - 40 - (2 * L / Math.Sqrt(2) + Rh * 2) * (Math.Sqrt(2) / 2)) < 0) Rx = Math.Abs(Rx - L - Rh * 2 - 40);
            if ((Rx + 40 + (2 * L / Math.Sqrt(2) + Rh * 2) * (Math.Sqrt(2) / 2)) > p.Width) p.Width += (int)(Rx + 40 + (2 * L / Math.Sqrt(2) + Rh * 2) * (Math.Sqrt(2) / 2)) - p.Width;
        }

        public void Grid(PictureBox p, Graphics g, List<Point> pts)
        {
            //Axis
            g.DrawString("Y", new Font("Arial", 7, FontStyle.Bold), new SolidBrush(Color.Black), 1, p.Height-35);
            g.DrawString("X", new Font("Arial", 7, FontStyle.Bold), new SolidBrush(Color.Black), p.Width - 15, 15);
            g.DrawString("1", new Font("Arial", 7, FontStyle.Bold), new SolidBrush(Color.Black), 1, 30);
            g.DrawString("1", new Font("Arial", 7, FontStyle.Bold), new SolidBrush(Color.Black), 20, 10);
            g.DrawLine(new Pen(Color.Black, 2), 10, 10, 10, p.Height - 10);
            g.DrawLine(new Pen(Color.Black, 2), 10, p.Height - 10, 5, p.Height - 25);
            g.DrawLine(new Pen(Color.Black, 2), 10, p.Height - 10, 14, p.Height - 25);
            g.DrawLine(new Pen(Color.Black, 2), 10, 10, p.Width - 10, 10);
            g.DrawLine(new Pen(Color.Black, 2), p.Width - 10, 10, p.Width - 20, 5);
            g.DrawLine(new Pen(Color.Black, 2), p.Width - 10, 10, p.Width - 20, 15);

            //Grid
            int pos = 10;
            do
            {
                //g.DrawLine(new Pen(Color.Gray, 1), pos += 20, 0, pos, p.Height - 12);
                pts.Add(new Point((int)(pos += 20), (int)10));
                pts.Add(new Point((int)pos, (int)(p.Height - 12)));
            }
            while (pos < p.Width - 30);
            pos = 10;
            do
            {
                //g.DrawLine(new Pen(Color.Gray, 1), 11, pos += 20, p.Width - 10, pos);
                pts.Add(new Point((int)11, (int)(pos += 20)));
                pts.Add(new Point((int)(p.Width - 10), (int)pos));
            }
            while (pos < p.Height - 30);
        }

        public Point BezierPoint(double t, Point p0, Point p1, Point p2)
        {
            Point p = new Point();
            p.X = (int)(Math.Pow(1 - t, 2) * p0.X);
            p.X += (int)((1 - t) * 2*t * p1.X);
            p.X += (int)(t * t * p2.X);

            p.Y = (int)(Math.Pow(1 - t, 2) * p0.Y);
            p.Y += (int)((1 - t) * 2*t * p1.Y);
            p.Y += (int)(t * t * p2.Y);
            return p;
        }

        public double[,] Multiplication(double[,] a, double[,] b)
        {
            double[,] r = new double[a.GetLength(0), b.GetLength(1)];
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < b.GetLength(1); j++)
                {
                    for (int k = 0; k < b.GetLength(0); k++)
                    {
                        r[i, j] += a[i, k] * b[k, j];
                    }
                }
            }
            return r;
        }

        public void Line(List<Point> p, double x1, double y1, double x2, double y2)
        {
            double L = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
            double xr = x1, yr = y1;
            for (double i = 0; i <= L; i ++)
            {               
                //g.DrawRectangle(new Pen(Color.Red, 2), (float)xr, (float)yr, 1, 1);
                p.Add(new Point((int)xr, (int)yr));
                xr = (x1 + (i / (L - i)) * x2) / (1 + (i / (L - i)));
                yr = (y1 + (i / (L - i)) * y2) / (1 + (i / (L - i)));
            }
        }

        public void Circle(List<Point> p, double Rx, double Ry, double R, int angle)
        {
            float bx=0, by=0;
            float bxo, byo;
            float bxs,bys;
            int sang=(angle==360)?0:angle-180;

            bxs = bxo = (float)(Rx - R * Math.Cos(sang * Math.PI / 180));
            bys = byo = (float)(Ry - R * Math.Sin(sang * Math.PI / 180));

            for (int i=sang+1; i<angle+2; i+=1)
            {
                bx = (float)(Rx - R * Math.Cos(i * Math.PI / 180));
                by = (float)(Ry - R * Math.Sin(i * Math.PI / 180));
                p.Add(new Point((int)bx, (int)by));
                //Line(g, p, bxo, byo, bx, by);
                byo = by;
                bxo = bx;
            }
        }

        public void AxialLine(Graphics g, double x1, double y1, double x2, double y2)
        {
            double L = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2)), L1 = L;
            double xr = x1, yr = y1, xro = xr, yro = yr;
            bool IsTrue = false;
            while (!IsTrue)
            {
                xr = (xro + (40 / (L1 - 40)) * x2) / (1 + (40 / (L1 - 40)));
                yr = (yro + (40 / (L1 - 40)) * y2) / (1 + (40 / (L1 - 40)));
                if (Math.Sqrt(Math.Pow(xr - x1, 2) + Math.Pow(yr - y1, 2)) >= L)
                {
                    xr = x2;
                    yr = y2;
                    IsTrue = true;
                }
                g.DrawLine(new Pen(Color.Red, 2), (float)xro, (float)yro, (float)xr, (float)yr);
                if (!IsTrue)
                {
                    L1 = Math.Sqrt(Math.Pow(x2 - xr, 2) + Math.Pow(y2 - yr, 2));
                    xro = (xr + (12 / (L1 - 12)) * x2) / (1 + (12 / (L1 - 12)));
                    yro = (yr + (12 / (L1 - 12)) * y2) / (1 + (12 / (L1 - 12)));
                    xr = (xr + (10 / (L1 - 10)) * x2) / (1 + (10 / (L1 - 10)));
                    yr = (yr + (10 / (L1 - 10)) * y2) / (1 + (10 / (L1 - 10)));
                    if (Math.Sqrt(Math.Pow(xro - x1, 2) + Math.Pow(yro - y1, 2)) >= L) IsTrue = true;
                    else
                    {
                        g.DrawLine(new Pen(Color.Red,2), (float)xr, (float)yr, (float)xro, (float)yro);
                        L1 = Math.Sqrt(Math.Pow(x2 - xro, 2) + Math.Pow(y2 - yro, 2));
                        xro = (xro + (10 / (L1 - 10)) * x2) / (1 + (10 / (L1 - 10)));
                        yro = (yro + (10 / (L1 - 10)) * y2) / (1 + (10 / (L1 - 10)));
                        L1 = Math.Sqrt(Math.Pow(x2 - xro, 2) + Math.Pow(y2 - yro, 2));
                        if (Math.Sqrt(Math.Pow(xro - x1, 2) + Math.Pow(yro - y1, 2)) >= L) IsTrue = true;
                    }
                }
            }
        }
    }
}