using SharpGL;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Laba3
{
    /// <summary>
    /// The main form class.
    /// </summary>
    public partial class SharpGLForm : Form
    {

        struct Сourse
        {
            public double x;
            public double y;
            public double z;

            public Сourse(double x, double y, double z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public static double DobS(Сourse vector1, Сourse vector2)
            {
                return (vector1.x * vector2.x + vector1.y * vector2.y + vector1.z * vector2.z);
            }
            public static Сourse Enhance(Сourse vector1, Сourse vector2)
            {
                return new Сourse(vector1.x * vector2.x, vector1.y * vector2.y, vector1.z * vector2.z);
            }

            public static Сourse Enhance(Сourse vector1, double sc)
            {
                return new Сourse(vector1.x * sc, vector1.y * sc, vector1.z * sc);
            }
            public static Сourse Cross(Сourse vector1, Сourse vector2)
            {
                return new Сourse(vector1.y * vector2.z - vector1.z * vector2.y, vector1.z * vector2.x - vector1.x * vector2.z, vector1.x * vector2.y - vector1.y * vector2.x);
            }


            public static Сourse Deduct(Сourse vector1, Сourse vector2)
            {
                return new Сourse(vector1.x - vector2.x, vector1.y - vector2.y, vector1.z - vector2.z);
            }

            public static Сourse PlusV(Сourse vector1, Сourse vector2)
            {
                return new Сourse(vector1.x + vector2.x, vector1.y + vector2.y, vector1.z + vector2.z);
            }
            public static Сourse Normalize(Сourse vector1)
            {
                double length = 1.0 / (Math.Sqrt(vector1.x * vector1.x + vector1.y * vector1.y + vector1.z * vector1.z));

                return new Сourse(vector1.x *= length, vector1.y *= length, vector1.z *= length);
            }
        }

        public SharpGLForm()
        {
            InitializeComponent();
        }

        
        double c = 2, teta = 0.2 * Math.PI;
        double oldAngleX, oldAngleY;
        double oldDistX, oldDistY;
        double angleX, angleY, distX, distY;
       // double zoom = -10;
        double[] viewMatrix;

        public double[,] x;
        private double[,] y;
        private double[,] z;
        double a = 0;
        double b = 0;
        double n = 0;
        double vector1 = 0;
        double vector2 = 0;
        private double zoom = 0.9;
        private double angle = 0;
        private double angley = 0;
        private double movemx = 0.1;
        private double movemy = -6.5;
        private double movemx1 = 0;
        private double movemy1 = 0;

        double[][] MirrorMatrixP(double[][] a, double[][] b)
        {
            double[][] res = new double[a.GetLength(0)][];
            for (int i = 0; i < a.GetLength(0); i++)
            {
                res[i] = new double[b.GetLength(0)];
            }

            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < b.GetLength(0); j++)
                {
                    for (int k = 0; k < b.GetLength(0); k++)
                    {
                        res[i][j] += a[i][k] * b[k][j];
                    }
                }
            }

            return res;
        }

        void MirrorMatrix(double[][] m, double[] p, double[] v)
        {
            double dot = p[0] * v[0] + p[1] * v[1] + p[2] * v[2];

            m[0][0] = 1 - 2 * v[0] * v[0];
            m[1][0] = -2 * v[0] * v[1];
            m[2][0] = -2 * v[0] * v[2];
            m[3][0] = 2 * dot * v[0];

            m[0][1] = -2 * v[1] * v[0];
            m[1][1] = 1 - 2 * v[1] * v[1];
            m[2][1] = -2 * v[1] * v[2];
            m[3][1] = 2 * dot * v[1];

            m[0][2] = -2 * v[2] * v[0];
            m[1][2] = -2 * v[2] * v[1];
            m[2][2] = 1 - 2 * v[2] * v[2];
            m[3][2] = 2 * dot * v[2];

            m[0][3] = 0;
            m[1][3] = 0;
            m[2][3] = 0;
            m[3][3] = 1;
        }

        private void openGLControl_Load(object sender, EventArgs e)
        {
            OpenGL gl = openGLControl.OpenGL;

            viewMatrix = new double[16];

            gl.ClearColor(255, 255, 255, 1);
           

            gl.MatrixMode(OpenGL.GL_PROJECTION);

            gl.LoadIdentity();

            gl.Perspective(45, (double)Width / (double)Height, 0.01, 200);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            gl.LoadIdentity();
            a = Convert.ToDouble(textBox1.Text);
            b = Convert.ToDouble(textBox2.Text);
            n = Convert.ToDouble(textBox3.Text);
            vector1 = Convert.ToDouble(textBox4.Text);
            vector2 = Convert.ToDouble(textBox5.Text);
            //ligPos = new Сourse(10, 20, 10);
            //Gl.glGetDoublev(Gl.GL_MODELVIEW_MATRIX, @mtr);
            //viewPos = MatrInitializer(mtr);
            //Drow();
        }
       

       

        // Handles the OpenGLDraw event of the openGLControl control.
        private void openGLControl_OpenGLDraw(object sender, RenderEventArgs e)
        {
            
            int i = 0; int j = 0;
            double h1 = 0.5;//0.05
            double h2 = 0.05;//0.005
            x = new double[Convert.ToInt32((vector2 - vector1) / h1) + 1, Convert.ToInt32((2 * Math.PI) / h2) + 1];
            y = new double[Convert.ToInt32((vector2 - vector1) / h1) + 1, Convert.ToInt32((2 * Math.PI) / h2) + 1];
            z = new double[Convert.ToInt32((vector2 - vector1) / h1) + 1, Convert.ToInt32((2 * Math.PI) / h2) + 1];
            OpenGL gl = openGLControl.OpenGL;

            Сourse lightPosition = new Сourse(0, 20, 0);
            Сourse viewerPosition = MatrInitializer(viewMatrix);
            List<double[]> listpointF = new List<double[]>();
            double[][] reflectionMatrix = new double[4][]
            {
                new double[4],
                new double[4],
                new double[4],
                new double[4]
            };
            double[] masA = new double[] { 0, 0, -4.3 };
            double[] masB = new double[] { 0, 0, 1 };
            List<double[]> mirrorRef = new List<double[]>();
            double[][] matrRef;

            for (double v = vector1; v <= vector2; v += h1)
            {
                j = 0;
                for (double u = 0; u <= 2 * Math.PI; u += h2)
                {
                    double x1 = (a + b * Math.Sin(n * u)) * Math.Cos(u) - v * Math.Sin(u);
                    double y1 = (a + b * Math.Sin(n * u)) * Math.Sin(u) + v * Math.Cos(u);
                    double z1 = b * Math.Cos(n * u);

                    x[i, j] = x1;
                    y[i, j] = y1;
                    z[i, j] = z1;
                    j++;
                }

                i++;
            }
            i = 0;
            for (double v = vector1; v <= vector2; v += h1)
            {
                x[i, j] = x[i, 0];
                y[i, j] = y[i, 0];
                z[i, j] = z[i, 0];
                i++;
            }

           

            //  Clear the color and depth buffer.
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.LoadIdentity();

            gl.Translate(distX * 0.1, distY * 0.1, zoom);
            gl.Rotate((float)angleY, (float)angleX, 0);

            //gl.Translate(movemx / Width * 9, movemy / Height * 9, -4.5);
            //gl.Rotate(angle, 0, 1, 0);
            //gl.Rotate(angley, 1, 0, 0);
            //gl.Scale(zoom, zoom, zoom);

            gl.GetDouble(OpenGL.GL_MODELVIEW_MATRIX, viewMatrix);

            gl.Disable(OpenGL.GL_LIGHTING);

            gl.ShadeModel(SharpGL.Enumerations.ShadeModel.Smooth);

            i = j = 0;
            gl.Begin(OpenGL.GL_TRIANGLES);
            for (double v = vector1; v < vector2 - h1; v += h1)
            {
                for (double u = 0; u < 2 * Math.PI; u += h2)
                {
                    listpointF.Add(new double[] { x[i, j], y[i, j], z[i, j], 1 });
                    mirrorRef.Add(LamP(Normall(v, u),new Сourse(
                        x[i, j] - lightPosition.x, y[i, j] - lightPosition.y, z[i, j] - lightPosition.z),
                                                      viewerPosition));
                    gl.Color(mirrorRef[mirrorRef.Count - 1]);
                    gl.Vertex(x[i, j], y[i, j], z[i, j]);


                    listpointF.Add(new double[] { x[i, j + 1], y[i, j + 1], z[i, j + 1], 1 });
                    mirrorRef.Add(LamP(Normall(v, u),new Сourse(
                        x[i, j + 1] - lightPosition.x, y[i, j + 1] - lightPosition.y, z[i, j + 1] - lightPosition.z),
                                                       viewerPosition));
                    gl.Color(mirrorRef[mirrorRef.Count - 1]);
                    gl.Vertex(x[i, j + 1], y[i, j + 1], z[i, j + 1]);


                    listpointF.Add(new double[] { x[i + 1, j], y[i + 1, j], z[i + 1, j], 1 });
                    mirrorRef.Add(LamP(Normall(v, u),new Сourse(
                        x[i + 1, j] - lightPosition.x, y[i + 1, j] - lightPosition.y, z[i + 1, j] - lightPosition.z),
                                                       viewerPosition));
                    gl.Color(mirrorRef[mirrorRef.Count - 1]);
                    gl.Vertex(x[i + 1, j], y[i + 1, j], z[i + 1, j]);


                    listpointF.Add(new double[] { x[i + 1, j], y[i + 1, j], z[i + 1, j], 1 });
                    mirrorRef.Add(LamP(Normall(v, u),new Сourse(
                        x[i + 1, j] - lightPosition.x, y[i + 1, j] - lightPosition.y, z[i + 1, j] - lightPosition.z),
                                                      viewerPosition));
                    gl.Color(mirrorRef[mirrorRef.Count - 1]);
                    gl.Vertex(x[i + 1, j], y[i + 1, j], z[i + 1, j]);


                    listpointF.Add(new double[] { x[i, j + 1], y[i, j + 1], z[i, j + 1], 1 });
                    mirrorRef.Add(LamP(Normall(v, u),new Сourse
                        (x[i, j + 1] - lightPosition.x, y[i, j + 1] - lightPosition.y, z[i, j + 1] - lightPosition.z),
                                                      viewerPosition));
                    gl.Color(mirrorRef[mirrorRef.Count - 1]);
                    gl.Vertex(x[i, j + 1], y[i, j + 1], z[i, j + 1]);


                    listpointF.Add(new double[] { x[i + 1, j + 1], y[i + 1, j + 1], z[i + 1, j + 1], 1 });
                    mirrorRef.Add(LamP(Normall(v, u),new Сourse(
                        x[i + 1, j + 1] - lightPosition.x, y[i + 1, j + 1] - lightPosition.y, z[i + 1, j + 1] - lightPosition.z),
                                                       viewerPosition));
                    gl.Color(mirrorRef[mirrorRef.Count - 1]);
                    gl.Vertex(x[i + 1, j + 1], y[i + 1, j + 1], z[i + 1, j + 1]);
                    j++;

                }
                j = 0;
                i++;
            }
            gl.End();
            MirrorMatrix(reflectionMatrix, masA, masB);
            matrRef = MirrorMatrixP(listpointF.ToArray(), reflectionMatrix);
            gl.Begin(OpenGL.GL_TRIANGLES);
            {
                for (int k = 0; k < matrRef.Length; k++)
                {
                    gl.Color(mirrorRef[k][0], mirrorRef[k][1], mirrorRef[k][2], 0.5);
                    gl.Vertex(matrRef[k]);
                }
            }
            gl.End();
            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(SharpGL.Enumerations.BlendingSourceFactor.SourceAlpha, SharpGL.Enumerations.BlendingDestinationFactor.OneMinusDestinationAlpha);
            gl.Begin(OpenGL.GL_POLYGON);
            {
                gl.Color(0.5f, 0.5f, 0.5f);
                gl.Vertex(-7, -7, -4.3);
                gl.Vertex(-7, 7, -4.3);
                gl.Vertex(7, 7, -4.3);
                gl.Vertex(7, -7, -4.3);
            }
            gl.End();
            gl.Disable(OpenGL.GL_BLEND);
            gl.BlendFunc(SharpGL.Enumerations.BlendingSourceFactor.DestinationColor, SharpGL.Enumerations.BlendingDestinationFactor.OneMinusDestinationAlpha);
        }

        // Handles the OpenGLInitialized event of the openGLControl control.
        private void openGLControl_OpenGLInitialized(object sender, EventArgs e)
        {
            //OpenGL gl = openGLControl.OpenGL;

            //viewMatrix = new double[16];

            //gl.ClearColor(255, 255, 255, 1);
        }

        // Handles the Resized event of the openGLControl control.
        private void openGLControl_Resized(object sender, EventArgs e)
        {
            //OpenGL gl = openGLControl.OpenGL;

            //gl.MatrixMode(OpenGL.GL_PROJECTION);

            //gl.LoadIdentity();

            //gl.Perspective(45, (double)Width / (double)Height, 0.01,100);

            //gl.MatrixMode(OpenGL.GL_MODELVIEW);

            //gl.LoadIdentity();
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            //Double.TryParse(textBoxA.Text, out a);
            //Double.TryParse(textBoxC.Text, out c);
            //Double.TryParse(textBoxTeta.Text, out teta);
        }

        private double resx = 0;
        private double resy = 0.1;
        private double ww = 0;

        private void button1_Click_1(object sender, EventArgs e)
        {
            a = Convert.ToDouble(textBox1.Text);
            b = Convert.ToDouble(textBox2.Text);
            n = Convert.ToDouble(textBox3.Text);
            vector1 = Convert.ToDouble(textBox4.Text);
            vector2 = Convert.ToDouble(textBox5.Text);
           // Drow();
        }

        private void openGLControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                angleX -= (oldAngleX - e.X) / 3;
                angleY -= (oldAngleY - e.Y) / 3;

                angleY = (angleY < -89.0) ? -89.0 : (angleY > 89.0) ? 89.0 : angleY;
            }
            if (e.Button == MouseButtons.Middle)
            {
                distX -= (oldDistX - e.X) / 3;
                distY += (oldDistY - e.Y) / 3;
            }

            oldAngleX = oldDistX = e.X;
            oldAngleY = oldDistY = e.Y;

            //if (e.Button == MouseButtons.Right)
            //{
            //    if (e.X > resx)
            //        angle += 2;
            //    else angle -= 2;
            //    resx = e.X;

            //    if (e.Y > resy)
            //        angley += 2;
            //    else angley -= 2;
            //    resy = e.Y;
            //   // Drow();
            //}

            //if (e.Button == MouseButtons.Middle)
            //{
            //    movemx = movemx + (e.X - movemx1);
            //    movemx1 = movemx;
            //    movemy = movemy - (e.Y - movemy1);
            //    movemy1 = movemy;
            //   // Drow();
            //}
            //movemx1 = e.X;
            //movemy1 = e.Y;
        }

        private void openGLControl_MouseWheel(object sender, MouseEventArgs e)
        {
            zoom += e.Delta / 120;
            //if (e.Delta > 0)
            //    zoom += 0.05;
            //else
            //    zoom -= 0.05;

            //Drow();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            a = Convert.ToDouble(textBox1.Text);
            b = Convert.ToDouble(textBox2.Text);
            n = Convert.ToDouble(textBox3.Text);
            vector1 = Convert.ToDouble(textBox4.Text);
            vector2 = Convert.ToDouble(textBox5.Text);
          //  Drow();
        }

        double[] LamP(Сourse par1, Сourse par2, Сourse viewPos)
        {
            Сourse lighting = new Сourse();

            Сourse surroundingP = new Сourse(0.6, 0.1, 0.5);//(0.6, 0.7, 0.2);
            Сourse dosparseP = new Сourse(0.5, 0.01, 0.2);//(0.2, 1, 0.2);
            Сourse hyalineP = new Сourse(0.2, 0.1, 0.2);//(0.2, 0.9, 0.2);

            Сourse surroundingS = new Сourse(0.5, 0.5, 0.5);//(0.6, 0.5, 0.1);
            Сourse dosparseS = new Сourse(1, 0.9, 0.1);//(0.5, 0.5, 0.7);
            Сourse hyalineS = new Сourse(1, 0.5, 0.1);//(1, 0.5, 0.5);


            Сourse surrounding = new Сourse();
            Сourse dosparse = new Сourse();
            surrounding = Сourse.Enhance(surroundingP, surroundingS);
            surrounding = Сourse.Normalize(surrounding);
            surrounding = Сourse.Enhance(surrounding, 0.2);
            dosparse.x = Math.Max(Сourse.DobS(par2, par1), 0) * dosparseP.x * dosparseS.x;
            dosparse.y = Math.Max(Сourse.DobS(par2, par1), 0) * dosparseP.y * dosparseS.y;
            dosparse.z = Math.Max(Сourse.DobS(par2, par1), 0) * dosparseP.z * dosparseS.z;
            dosparse = Сourse.Enhance(dosparse, 0.3f);
            Сourse reflection = new Сourse();
            reflection = Сourse.Deduct(par2, Сourse.Enhance(par1, 2 * Сourse.DobS(par1, par2)));
            reflection = Сourse.Normalize(reflection);
            Сourse hyaline = new Сourse();
            hyaline.x = Math.Max(Сourse.DobS(reflection, viewPos), 0) * hyalineP.x * hyalineS.x;
            hyaline.y = Math.Max(Сourse.DobS(reflection, viewPos), 0) * hyalineP.y * hyalineS.y;
            hyaline.z = Math.Max(Сourse.DobS(reflection, viewPos), 0) * hyalineP.z * hyalineS.z;
            hyaline = Сourse.Enhance(hyaline, 0.2f);
            lighting = Сourse.PlusV(surrounding, dosparse);
            lighting = Сourse.PlusV(lighting, hyaline);
            return new double[]
            {
                lighting.x, lighting.y, lighting.z
            };
        }
        Сourse Normall(double v, double u)
        {
            double h3 = 0.000001;
            Сourse vСourse = new Сourse();
            Сourse uСourse = new Сourse();
            vСourse.x = (((a + b * Math.Sin(n * u)) * Math.Cos(u) - (v + h3) * Math.Sin(u)) - ((a + b * Math.Sin(n * u)) * Math.Cos(u) - v * Math.Sin(u))) / h3;
            vСourse.y = (((a + b * Math.Sin(n * u)) * Math.Sin(u) + (v + h3) * Math.Cos(u)) - ((a + b * Math.Sin(n * u)) * Math.Sin(u) + v * Math.Cos(u))) / h3;
            vСourse.z = 0;
            uСourse.x = (((a + b * Math.Sin(n * (u + h3))) * Math.Cos((u + h3)) - v * Math.Sin((u + h3))) - ((a + b * Math.Sin(n * u)) * Math.Cos((u)) - v * Math.Sin(u))) / h3;
            uСourse.y = (((a + b * Math.Sin(n * (u + h3))) * Math.Sin((u + h3)) + v * Math.Cos((u + h3))) - ((a + b * Math.Sin(n * u)) * Math.Sin(u) + v * Math.Cos(u))) / h3;
            uСourse.z = ((b * Math.Cos(n * (u + h3))) - (b * Math.Cos(n * u))) / h3;
            Сourse resault = new Сourse();
            resault = Сourse.Cross(vСourse, uСourse);
            resault = Сourse.Normalize(resault);
            return resault;
        }
        Сourse MatrInitializer(double[] mtr)
        {
            return new Сourse(-(mtr[0] * mtr[12] + mtr[1] * mtr[13] + mtr[2] * mtr[14]),
                              -(mtr[4] * mtr[12] + mtr[5] * mtr[13] + mtr[6] * mtr[14]),
                              -(mtr[8] * mtr[12] + mtr[9] * mtr[13] + mtr[10] * mtr[14]));
        }
    }
}
