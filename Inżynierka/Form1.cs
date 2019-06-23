using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
//using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Inżynierka
{
    public partial class Form1 : Form
    {
        float rotation_X = 0.0f;
        float rotation_Y = 0.0f;
        float boundaries_X = 5.0f;
        float boundaries_Y = 5.0f;
        float boundaries_Z = 5.0f;
        float cameraDistance = -10.0f;
        double camX;
        double camY;
        double camZ;
        Vector3 selector = new Vector3(0.5f, 0.5f, 0.5f);
        Matrix4 projection;

        public Form1()
        {
            InitializeComponent();
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            int w = glControl1.Width;
            int h = glControl1.Height;
            glControl1.MakeCurrent();
            GL.Viewport(0, 0, w, h);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.ClearColor(Color.SkyBlue);
            //GL.Ortho(-w / 2, w / 2, -h / 2, h / 2, -5, 5);
            //Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(60.0f * ((float)Math.PI / 180f), w / (float)h, 0.03f, 100.0f);
            //GL.LoadMatrix(ref projection);
            //GL.Ortho(-10, 10, -10, 10, -100, 100);
            OpenTK.Graphics.Glu.Perspective(60.0f, (float)w / (float)h, 1.0f, 100.0f);
            //GL.End();
            glControl1.SwapBuffers();
        }
        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.MatrixMode(MatrixMode.Modelview);
            //GL.UniformMatrix4(0, false, ref projection);
            //GL.LoadMatrix(ref projection);
            GL.LoadIdentity();
            //camX = selector.X + (cameraDistance * -Math.Sin(rotation_X * (Math.PI / 180)) * Math.Cos((rotation_Y) * (Math.PI / 180)));
            //camY = selector.Y + (cameraDistance * -Math.Sin(rotation_Y * (Math.PI / 180)));
            //camZ = selector.Z + (-cameraDistance * Math.Cos((rotation_X) * (Math.PI / 180)) * Math.Cos((rotation_Y) * (Math.PI / 180)));
            //camX = -cameraDistance * Math.Sin(rotation_X * (Math.PI / 180));
            //camX = -cameraDistance * Math.Cos(rotation_Y * (Math.PI / 180));
            //GL.Translate(-selector.X, -selector.Y, -selector.Z);
            //Matrix4.LookAt(new Vector3(selector.X, selector.Y, selector.Z + cameraDistance), selector, Vector3.UnitY);
            //GL.Translate(0.0f, 0.0f, cameraDistance);
            //GL.Translate(selector.X, selector.Y, selector.Z + cameraDistance);

            //OpenTK.Graphics.Glu.LookAt(0, 0, cameraDistance,0, 0, 0, 0, 1, 0);
            //GL.Rotate(rotation_X, Vector3.UnitX);
            //GL.Rotate(rotation_Y, Vector3.UnitY);
            //GL.Translate(selector.X, selector.Y, selector.Z);
            
            GL.Translate(-selector.X, -selector.Y, -selector.Z+ cameraDistance);
            
            GL.PushMatrix();

            //DrawBox(.2f,Color.Blue);
            //GL.PushMatrix();
            GL.Translate(selector.X, selector.Y, selector.Z);
            GL.Rotate(rotation_X, 1,0,0);
            GL.Rotate(rotation_Y, 0,1,0);
            GL.Translate(-selector.X, -selector.Y, -selector.Z);
            //DrawBox(.2f,Color.Green);
            //DrawSelector(selector.X, selector.Y, selector.Z);
            DrawSelector(selector.X, selector.Y, selector.Z);
            DrawBoundaries();

            GL.PopMatrix();
            //DrawBox(.2f,Color.Red);

            //GL.PushMatrix();
            //GL.Translate(-selector.X, -selector.Y, -selector.Z);



            //GL.Translate(selector.X, selector.Y, selector.Z);// +cameraDistance);
            //GL.PopMatrix();

            //GL.Translate(-0.5f, -0.5f, -0.5f + cameraDistance);
            //DrawBoundaries();
            //GL.Translate(0.5f, 0.5f, 0.5f);
            //DrawBox(1.0f, new Vector3(0.0f, 0.0f, 0.0f));
            //DrawBoundaries();
            //GL.Rotate(0, new Vector3(0.0f, 0.0f, 1.0f));
            //DrawBox(0.5f,selector);
            GL.Flush();
            glControl1.SwapBuffers();
        }

        private void DrawBox(float size,Vector3 position)
        {
            GL.PushMatrix();
            GL.Translate(position.X, position.Y, position.Z);
            GL.Color3(Color.Crimson);
            float[,] n = new float[,]{
            {-1.0f, 0.0f, 0.0f},
            {0.0f, 1.0f, 0.0f},
            {1.0f, 0.0f, 0.0f},
            {0.0f, -1.0f, 0.0f},
            {0.0f, 0.0f, 1.0f},
            {0.0f, 0.0f, -1.0f}
        };
            int[,] faces = new int[,]{
            {0, 1, 2, 3},
            {3, 2, 6, 7},
            {7, 6, 5, 4},
            {4, 5, 1, 0},
            {5, 6, 2, 1},
            {7, 4, 0, 3}
        };
            float[,] v = new float[8, 3];
            int i;

            v[0, 0] = v[1, 0] = v[2, 0] = v[3, 0] = -size / 2;
            v[4, 0] = v[5, 0] = v[6, 0] = v[7, 0] = size / 2;
            v[0, 1] = v[1, 1] = v[4, 1] = v[5, 1] = -size / 2;
            v[2, 1] = v[3, 1] = v[6, 1] = v[7, 1] = size / 2;
            v[0, 2] = v[3, 2] = v[4, 2] = v[7, 2] = -size / 2;
            v[1, 2] = v[2, 2] = v[5, 2] = v[6, 2] = size / 2;

            GL.Begin(PrimitiveType.Quads);
            for (i = 5; i >= 0; i--)
            {
                GL.Normal3(ref n[i, 0]);
                GL.Vertex3(ref v[faces[i, 0], 0]);
                GL.Vertex3(ref v[faces[i, 1], 0]);
                GL.Vertex3(ref v[faces[i, 2], 0]);
                GL.Vertex3(ref v[faces[i, 3], 0]);
            }
            GL.End();
            GL.PopMatrix();
        }

        private void DrawBox(float size, Color color)
        {
            //GL.PushMatrix();
            //GL.Translate(position.X, position.Y, position.Z);
            GL.Color3(color);
            float[,] n = new float[,]{
            {-1.0f, 0.0f, 0.0f},
            {0.0f, 1.0f, 0.0f},
            {1.0f, 0.0f, 0.0f},
            {0.0f, -1.0f, 0.0f},
            {0.0f, 0.0f, 1.0f},
            {0.0f, 0.0f, -1.0f}
        };
            int[,] faces = new int[,]{
            {0, 1, 2, 3},
            {3, 2, 6, 7},
            {7, 6, 5, 4},
            {4, 5, 1, 0},
            {5, 6, 2, 1},
            {7, 4, 0, 3}
        };
            float[,] v = new float[8, 3];
            int i;

            v[0, 0] = v[1, 0] = v[2, 0] = v[3, 0] = -size / 2;
            v[4, 0] = v[5, 0] = v[6, 0] = v[7, 0] = size / 2;
            v[0, 1] = v[1, 1] = v[4, 1] = v[5, 1] = -size / 2;
            v[2, 1] = v[3, 1] = v[6, 1] = v[7, 1] = size / 2;
            v[0, 2] = v[3, 2] = v[4, 2] = v[7, 2] = -size / 2;
            v[1, 2] = v[2, 2] = v[5, 2] = v[6, 2] = size / 2;

            GL.Begin(PrimitiveType.Quads);
            for (i = 5; i >= 0; i--)
            {
                GL.Normal3(ref n[i, 0]);
                GL.Vertex3(ref v[faces[i, 0], 0]);
                GL.Vertex3(ref v[faces[i, 1], 0]);
                GL.Vertex3(ref v[faces[i, 2], 0]);
                GL.Vertex3(ref v[faces[i, 3], 0]);
            }
            GL.End();
            //GL.PopMatrix();
        }

        void DrawBoundaries()
        {
            GL.PushMatrix();
            GL.Color3(Color.Crimson);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(0.0f, 0.0f, 0.0f);
            GL.Vertex3(boundaries_X, 0.0f, 0.0f);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(boundaries_X, 0.0f, 0.0f);
            GL.Vertex3(boundaries_X, boundaries_Y, 0.0f);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(boundaries_X, boundaries_Y, 0.0f);
            GL.Vertex3(0.0f, boundaries_Y, 0.0f);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(0.0f, boundaries_Y, 0.0f);
            GL.Vertex3(0.0f, 0.0f, 0.0f);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(0.0f, 0.0f, boundaries_Z);
            GL.Vertex3(boundaries_X, 0.0f, boundaries_Z);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(boundaries_X, 0.0f, boundaries_Z);
            GL.Vertex3(boundaries_X, boundaries_Y, boundaries_Z);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(boundaries_X, boundaries_Y, boundaries_Z);
            GL.Vertex3(0.0f, boundaries_Y, boundaries_Z);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(0.0f, boundaries_Y, boundaries_Z);
            GL.Vertex3(0.0f, 0.0f, boundaries_Z);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(0.0f, 0.0f, boundaries_Z);
            GL.Vertex3(0.0f, 0.0f, 0.0f);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(boundaries_X, 0.0f, boundaries_Z);
            GL.Vertex3(boundaries_X, 0.0f, 0.0f);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(0.0f, boundaries_Y, boundaries_Z);
            GL.Vertex3(0.0f, boundaries_Y, 0.0f);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(boundaries_X, boundaries_Y, boundaries_Z);
            GL.Vertex3(boundaries_X, boundaries_Y, 0.0f);
            GL.End();
            GL.PopMatrix();
        }

        void DrawSelector(float x,float y,float z)
        {
            GL.PushMatrix();
            GL.Translate(x, y, z);
            GL.Color3(Color.DarkKhaki);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(0.0f, 0.0f, 0.0f);
            GL.Vertex3(- 0.5f, - 0.5f, - 0.5f);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(-0.5f, -0.5f, -0.5f);
            GL.Vertex3( + 0.5f,  - 0.5f,  - 0.5f);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3( + 0.5f, - 0.5f,  - 0.5f);
            GL.Vertex3( + 0.5f, + 0.5f,  - 0.5f);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3( + 0.5f,  + 0.5f,  - 0.5f);
            GL.Vertex3( - 0.5f,  + 0.5f,  - 0.5f);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3( - 0.5f,  + 0.5f,  - 0.5f);
            GL.Vertex3( - 0.5f,  - 0.5f,  - 0.5f);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3( - 0.5f,  - 0.5f,  + 0.5f);
            GL.Vertex3( + 0.5f,  - 0.5f,  + 0.5f);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3( + 0.5f,  - 0.5f,  + 0.5f);
            GL.Vertex3( + 0.5f,  + 0.5f,  + 0.5f);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3( + 0.5f,  + 0.5f,  + 0.5f);
            GL.Vertex3( - 0.5f,  + 0.5f,  + 0.5f);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3( - 0.5f,  + 0.5f,  + 0.5f);
            GL.Vertex3( - 0.5f,  - 0.5f,  + 0.5f);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3( - 0.5f, - 0.5f,  + 0.5f);
            GL.Vertex3( - 0.5f,  - 0.5f,  - 0.5f);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3( + 0.5f,  - 0.5f,  + 0.5f);
            GL.Vertex3( + 0.5f,  - 0.5f,  - 0.5f);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3( - 0.5f,  + 0.5f,  + 0.5f);
            GL.Vertex3( - 0.5f,  + 0.5f,  - 0.5f);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3( + 0.5f,  + 0.5f,  + 0.5f);
            GL.Vertex3( + 0.5f,  + 0.5f,  - 0.5f);
            GL.End();
            GL.PopMatrix();
        }

        void drawCircle(float radius)
        {

            GL.Color3(Color.White);
            GL.Begin(PrimitiveType.TriangleFan);

            for (int i = 0; i < 360; i++)
            {
                double degInRad = i * 3.1416 / 180;
                GL.Vertex2(Math.Cos(degInRad) * radius, Math.Sin(degInRad) * radius);
            }
            GL.End();
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(Color.SkyBlue);
            GL.Enable(EnableCap.DepthTest);
        }

        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.I)
            {
                rotation_X += 2.0f;
                if (rotation_X >= 360.0f) rotation_X -= 360.0f;
            }
            if (e.KeyCode == Keys.K)
            {
                rotation_X -= 2.0f;
                if (rotation_X <= 0.0f) rotation_X += 360.0f;
            }
            if (e.KeyCode == Keys.J)
            {
                rotation_Y += 2.0f;
                if (rotation_Y >= 360.0f) rotation_Y -= 360.0f;
            }
            if (e.KeyCode == Keys.L)
            {
                rotation_Y -= 2.0f;
                if (rotation_Y <= 0.0f) rotation_Y += 360.0f;
            }
            if (e.KeyCode == Keys.Q)
            {
                cameraDistance -= 0.5f;
            }
            if (e.KeyCode == Keys.E)
            {
                cameraDistance += 0.5f;
            }
            if (e.KeyCode == Keys.A)
            {
                selector.X += 1.0f;
            }
            if (e.KeyCode == Keys.D)
            {
                selector.X -= 1.0f;
            }
            if (e.KeyCode == Keys.S)
            {
                selector.Z += 1.0f;
            }
            if (e.KeyCode == Keys.W)
            {
                selector.Z -= 1.0f;
            }
            if (e.KeyCode == Keys.C)
            {
                selector.Y += 1.0f;
            }
            if (e.KeyCode == Keys.Z)
            {
                selector.Y -= 1.0f;
            }
            glControl1.Invalidate();
        }
    }
}