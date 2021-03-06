﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Inżynierka
{
    public partial class Form1 : Form
    {
        float rotation_X = 40.0f;
        float rotation_Y = 40.0f;
        float boundaries_X = 15.0f;
        float boundaries_Y = 15.0f;
        float boundaries_Z = 15.0f;
        float light_X = 25.0f;
        float light_Y = 100.0f;
        float light_Z = 25.0f;
        float light_Radius = 0.0f;
        Color light_Color = Color.White;
        float cameraDistance = -10.0f;
        Vector3 selector = new Vector3(0.5f, 0.5f, 0.5f);
        bool[,,] cubeMatrix;
        bool[,,] copyMatrix;
        Color[,,] chosenColor;
        Color[,,] colorMatrix;
        Color currentColor = Color.White;
        float sceneSize = 15f;
        float viewRange = 10f;
        bool activateRangeView = false;
        Vector3 point1;
        bool activePoint1 = false;
        Vector3 point2;
        bool activePoint2 = false;
        bool showXY = false;
        bool showXZ = false;
        bool showYZ = false;
        bool onlyBuild = false;
        bool onlyErase = false;
        bool copyReady = false;
        string save1 = "Save1.txt";
        string save1stl = "Save1.stl";

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
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.DepthTest);
            GL.LoadIdentity();     
            GL.Translate(-selector.X, -selector.Y, -selector.Z + cameraDistance);
            GL.PushMatrix();
            GL.Translate(selector.X, selector.Y, selector.Z);
            GL.Rotate(rotation_X, 1,0,0);
            GL.Rotate(rotation_Y, 0,1,0);
            GL.Translate(-selector.X, -selector.Y, -selector.Z);
            GL.LineWidth(1.0f);
            GL.Light(LightName.Light0, LightParameter.Position, new float[] { light_X, light_Y, light_Z, 1.0f });
            GL.Light(LightName.Light0, LightParameter.Diffuse, light_Color);
            GL.Light(LightName.Light0, LightParameter.Ambient, new float[] { 0.15f, 0.15f, 0.15f, 0.05f});
            drawLightSource(new Vector3(light_X, light_Y, light_Z));
            if(activePoint1 && activePoint2)
            {
                DrawAreaSelector(point1, point2, Color.DarkKhaki);
            }
            else DrawSelector(selector.X, selector.Y, selector.Z, Color.DarkKhaki);
            if (activePoint1)
            {
                DrawSelector(point1.X, point1.Y, point1.Z, Color.Indigo);
            }
            if(activePoint2)
            {
                DrawSelector(point2.X, point2.Y, point2.Z, Color.Indigo);
            }
            DrawBoundaries();
            if (showXY)
            {
                if (activateRangeView)
                {
                    for (int i = 0; i < boundaries_X; i++)
                    {
                        for (int j = 0; j < boundaries_Y; j++)
                        {
                            for (int k = (int)(selector.Z-0.5f); k < selector.Z; k++)
                            {
                                if (cubeMatrix[i, j, k] && Vector3.Distance(new Vector3(i, j, k), new Vector3(selector.X - 0.5f, selector.Y - 0.5f, selector.Z - 0.5f)) < viewRange)
                                    DrawBox(new Vector3(i + .5f, j + .5f, k + .5f));
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < boundaries_X; i++)
                    {
                        for (int j = 0; j < boundaries_Y; j++)
                        {
                            for (int k = (int)(selector.Z - 0.5f); k < selector.Z; k++)
                            {
                                if (cubeMatrix[i, j, k])
                                    DrawBox(new Vector3(i + .5f, j + .5f, k + .5f));
                            }
                        }
                    }
                }
            }
            else if (showXZ)
            {
                if (activateRangeView)
                {
                    for (int i = 0; i < boundaries_X; i++)
                    {
                        for (int j = (int)(selector.Y - 0.5f); j < selector.Y; j++)
                        {
                            for (int k = 0; k < boundaries_Z; k++)
                            {
                                if (cubeMatrix[i, j, k] && Vector3.Distance(new Vector3(i, j, k), new Vector3(selector.X - 0.5f, selector.Y - 0.5f, selector.Z - 0.5f)) < viewRange)
                                    DrawBox(new Vector3(i + .5f, j + .5f, k + .5f));
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < boundaries_X; i++)
                    {
                        for (int j = (int)(selector.Y - 0.5f); j < selector.Y; j++)
                        {
                            for (int k = 0; k < boundaries_Z; k++)
                            {
                                if (cubeMatrix[i, j, k])
                                    DrawBox(new Vector3(i + .5f, j + .5f, k + .5f));
                            }
                        }
                    }
                }
            }
            else if (showYZ)
            {
                if (activateRangeView)
                {
                    for (int i = (int)(selector.X - 0.5f); i < selector.X; i++)
                    {
                        for (int j = 0; j < boundaries_Y; j++)
                        {
                            for (int k = 0; k < boundaries_Z; k++)
                            {
                                if (cubeMatrix[i, j, k] && Vector3.Distance(new Vector3(i, j, k), new Vector3(selector.X - 0.5f, selector.Y - 0.5f, selector.Z - 0.5f)) < viewRange)
                                {
                                    DrawFrame(new Vector3(i + .5f, j + .5f, k + .5f));
                                    DrawBox(new Vector3(i + .5f, j + .5f, k + .5f));
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int i = (int)(selector.X - 0.5f); i < selector.X; i++)
                    {
                        for (int j = 0; j < boundaries_Y; j++)
                        {
                            for (int k = 0; k < boundaries_Z; k++)
                            {
                                if (cubeMatrix[i, j, k])
                                {
                                    DrawFrame(new Vector3(i + .5f, j + .5f, k + .5f));
                                    DrawBox(new Vector3(i + .5f, j + .5f, k + .5f));
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (activateRangeView)
                {
                    for (int i = 0; i < boundaries_X; i++)
                    {
                        for (int j = 0; j < boundaries_Y; j++)
                        {
                            for (int k = 0; k < boundaries_Z; k++)
                            {
                                if (cubeMatrix[i, j, k] && Vector3.Distance(new Vector3(i, j, k), new Vector3(selector.X - 0.5f, selector.Y - 0.5f, selector.Z - 0.5f)) < viewRange)
                                {
                                    DrawBox(new Vector3(i + .5f, j + .5f, k + .5f));
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < boundaries_X; i++)
                    {
                        for (int j = 0; j < boundaries_Y; j++)
                        {
                            for (int k = 0; k < boundaries_Z; k++)
                            {
                                if (cubeMatrix[i, j, k])
                                {
                                    DrawFrame(new Vector3(i + .5f, j + .5f, k + .5f));
                                    DrawBox(new Vector3(i + .5f, j + .5f, k + .5f));
                                }
                            }
                        }
                    }
                }
            }
            GL.Disable(EnableCap.Blend);
            GL.PopMatrix();
            GL.Flush();
            glControl1.SwapBuffers();
        }

        private void DrawBox(Vector3 position)
        {
            int calcX = (int)Math.Floor(position.X), calcY = (int)Math.Floor(position.Y), calcZ = (int)Math.Floor(position.Z);
            GL.PushMatrix();
            GL.Translate(position.X, position.Y, position.Z);
            GL.ColorMaterial(MaterialFace.Front, ColorMaterialParameter.AmbientAndDiffuse);
            GL.Color3(chosenColor[calcX, calcY, calcZ]);
            GL.LineWidth(1.0f);
            if (showXZ)
            {
                GL.Begin(PrimitiveType.Quads);
                GL.Normal3(0.0f, 1.0f, 0.0f);   // górna ściana (w płaszczyźnie XZ)
                GL.Vertex4(0.5f, 0.5f, 0.5f,1.0f);
                GL.Vertex4(0.5f, 0.5f, -0.5f, 1.0f);
                GL.Vertex4(-0.5f, 0.5f, -0.5f, 1.0f);
                GL.Vertex4(-0.5f, 0.5f, 0.5f, 1.0f);
                GL.End();
            }
            else if (calcY < boundaries_Y-1)
            {
                if (!cubeMatrix[calcX, calcY + 1, calcZ])
                {
                    GL.Begin(PrimitiveType.Quads);
                    GL.Normal3(0.0f, 1.0f, 0.0f);   // górna ściana (w płaszczyźnie XZ)
                    GL.Vertex4(0.5f, 0.5f, 0.5f, 1.0f);
                    GL.Vertex4(0.5f, 0.5f, -0.5f, 1.0f);
                    GL.Vertex4(-0.5f, 0.5f, -0.5f, 1.0f);
                    GL.Vertex4(-0.5f, 0.5f, 0.5f, 1.0f);
                    GL.End();
                }
            }
            else
            {
                GL.Begin(PrimitiveType.Quads);
                GL.Normal3(0.0f, 1.0f, 0.0f);   // górna ściana (w płaszczyźnie XZ)
                GL.Vertex4(0.5f, 0.5f, 0.5f, 1.0f);
                GL.Vertex4(0.5f, 0.5f, -0.5f, 1.0f);
                GL.Vertex4(-0.5f, 0.5f, -0.5f, 1.0f);
                GL.Vertex4(-0.5f, 0.5f, 0.5f, 1.0f);
                GL.End();
            }
            if (showXY)
            {
                GL.Begin(PrimitiveType.Quads);
                GL.Normal3(0.0f, 0.0f, 1.0f);   // przednia ściana (w płaszczyźnie XY)
                GL.Vertex4(0.5f, 0.5f, 0.5f, 1.0f);
                GL.Vertex4(-0.5f, 0.5f, 0.5f, 1.0f);
                GL.Vertex4(-0.5f, -0.5f, 0.5f, 1.0f);
                GL.Vertex4(0.5f, -0.5f, 0.5f, 1.0f);
                GL.End();
            }
            else if (calcZ < boundaries_Z-1)
            {
                if (!cubeMatrix[calcX, calcY, calcZ + 1])
                {
                    GL.Begin(PrimitiveType.Quads);
                    GL.Normal3(0.0f, 0.0f, 1.0f);   // przednia ściana (w płaszczyźnie XY)
                    GL.Vertex4(0.5f, 0.5f, 0.5f, 1.0f);
                    GL.Vertex4(-0.5f, 0.5f, 0.5f, 1.0f);
                    GL.Vertex4(-0.5f, -0.5f, 0.5f, 1.0f);
                    GL.Vertex4(0.5f, -0.5f, 0.5f, 1.0f);
                    GL.End();
                }
            }
            else
            {
                GL.Begin(PrimitiveType.Quads);
                GL.Normal3(0.0f, 0.0f, 1.0f);   // przednia ściana (w płaszczyźnie XY)
                GL.Vertex4(0.5f, 0.5f, 0.5f, 1.0f);
                GL.Vertex4(-0.5f, 0.5f, 0.5f, 1.0f);
                GL.Vertex4(-0.5f, -0.5f, 0.5f, 1.0f);
                GL.Vertex4(0.5f, -0.5f, 0.5f, 1.0f);
                GL.End();
            }
            if (showYZ)
            {
                GL.Begin(PrimitiveType.Quads);
                GL.Normal3(1.0f, 0.0f, 0.0f);   // prawa ściana (w płaszczyźnie YZ)
                GL.Vertex4(0.5f, 0.5f, 0.5f, 1.0f);
                GL.Vertex4(0.5f, -0.5f, 0.5f, 1.0f);
                GL.Vertex4(0.5f, -0.5f, -0.5f, 1.0f);
                GL.Vertex4(0.5f, 0.5f, -0.5f, 1.0f);
                GL.End();
            }
            else if (calcX < boundaries_X-1)
            {
                if (!cubeMatrix[calcX + 1, calcY, calcZ])
                {
                    GL.Begin(PrimitiveType.Quads);
                    GL.Normal3(1.0f, 0.0f, 0.0f);   // prawa ściana (w płaszczyźnie YZ)
                    GL.Vertex4(0.5f, 0.5f, 0.5f, 1.0f);
                    GL.Vertex4(0.5f, -0.5f, 0.5f, 1.0f);
                    GL.Vertex4(0.5f, -0.5f, -0.5f, 1.0f);
                    GL.Vertex4(0.5f, 0.5f, -0.5f, 1.0f);
                    GL.End();
                }
            }
            else
            {
                GL.Begin(PrimitiveType.Quads);
                GL.Normal3(1.0f, 0.0f, 0.0f);   // prawa ściana (w płaszczyźnie YZ)
                GL.Vertex4(0.5f, 0.5f, 0.5f, 1.0f);
                GL.Vertex4(0.5f, -0.5f, 0.5f, 1.0f);
                GL.Vertex4(0.5f, -0.5f, -0.5f, 1.0f);
                GL.Vertex4(0.5f, 0.5f, -0.5f, 1.0f);
                GL.End();
            }
            if(showYZ)
            {
                GL.Begin(PrimitiveType.Quads);
                GL.Normal3(-1.0f, 0.0f, 0.0f);  // lewa ściana (w płaszczyźnie YZ)
                GL.Vertex4(-0.5f, 0.5f, 0.5f, 1.0f);
                GL.Vertex4(-0.5f, 0.5f, -0.5f, 1.0f);
                GL.Vertex4(-0.5f, -0.5f, -0.5f, 1.0f);
                GL.Vertex4(-0.5f, -0.5f, 0.5f, 1.0f);
                GL.End();
            }
            else if (calcX > 0)
            {
                if (!cubeMatrix[calcX - 1, calcY, calcZ])
                {
                    GL.Begin(PrimitiveType.Quads);
                    GL.Normal3(-1.0f, 0.0f, 0.0f);  // lewa ściana (w płaszczyźnie YZ)
                    GL.Vertex4(-0.5f, 0.5f, 0.5f, 1.0f);
                    GL.Vertex4(-0.5f, 0.5f, -0.5f, 1.0f);
                    GL.Vertex4(-0.5f, -0.5f, -0.5f, 1.0f);
                    GL.Vertex4(-0.5f, -0.5f, 0.5f, 1.0f);
                    GL.End();
                }
            }
            else
            {
                GL.Begin(PrimitiveType.Quads);
                GL.Normal3(-1.0f, 0.0f, 0.0f);  // lewa ściana (w płaszczyźnie YZ)
                GL.Vertex4(-0.5f, 0.5f, 0.5f, 1.0f);
                GL.Vertex4(-0.5f, 0.5f, -0.5f, 1.0f);
                GL.Vertex4(-0.5f, -0.5f, -0.5f, 1.0f);
                GL.Vertex4(-0.5f, -0.5f, 0.5f, 1.0f);
                GL.End();
            }
            if (showXZ)
            {
                GL.Begin(PrimitiveType.Quads);
                GL.Normal3(0.0f, -1.0f, 0.0f);  // dolna ściana (w płaszczyźnie XZ)
                GL.Vertex4(-0.5f, -0.5f, 0.5f, 1.0f);
                GL.Vertex4(-0.5f, -0.5f, -0.5f, 1.0f);
                GL.Vertex4(0.5f, -0.5f, -0.5f, 1.0f);
                GL.Vertex4(0.5f, -0.5f, 0.5f, 1.0f);
                GL.End();
            }
            else if (calcY > 0)
            {
                if (!cubeMatrix[calcX, calcY - 1, calcZ])
                {
                    GL.Begin(PrimitiveType.Quads);
                    GL.Normal3(0.0f, -1.0f, 0.0f);  // dolna ściana (w płaszczyźnie XZ)
                    GL.Vertex4(-0.5f, -0.5f, 0.5f, 1.0f);
                    GL.Vertex4(-0.5f, -0.5f, -0.5f, 1.0f);
                    GL.Vertex4(0.5f, -0.5f, -0.5f, 1.0f);
                    GL.Vertex4(0.5f, -0.5f, 0.5f, 1.0f);
                    GL.End();
                }
            }
            else
            {
                GL.Begin(PrimitiveType.Quads);
                GL.Normal3(0.0f, -1.0f, 0.0f);  // dolna ściana (w płaszczyźnie XZ)
                GL.Vertex4(-0.5f, -0.5f, 0.5f, 1.0f);
                GL.Vertex4(-0.5f, -0.5f, -0.5f, 1.0f);
                GL.Vertex4(0.5f, -0.5f, -0.5f, 1.0f);
                GL.Vertex4(0.5f, -0.5f, 0.5f, 1.0f);
                GL.End();
            }
            if (showXY)
            {
                GL.Begin(PrimitiveType.Quads);
                GL.Normal3(0.0f, 0.0f, -1.0f);  // tylna ściana (w płaszczyźnie XY)
                GL.Vertex4(0.5f, -0.5f, -0.5f, 1.0f);
                GL.Vertex4(-0.5f, -0.5f, -0.5f, 1.0f);
                GL.Vertex4(-0.5f, 0.5f, -0.5f, 1.0f);
                GL.Vertex4(0.5f, 0.5f, -0.5f, 1.0f);
                GL.End();
            }
            else if (calcZ > 0)
            {
                if (!cubeMatrix[calcX, calcY, calcZ - 1])
                {
                    GL.Begin(PrimitiveType.Quads);
                    GL.Normal3(0.0f, 0.0f, -1.0f);  // tylna ściana (w płaszczyźnie XY)
                    GL.Vertex4(0.5f, -0.5f, -0.5f, 1.0f);
                    GL.Vertex4(-0.5f, -0.5f, -0.5f, 1.0f);
                    GL.Vertex4(-0.5f, 0.5f, -0.5f, 1.0f);
                    GL.Vertex4(0.5f, 0.5f, -0.5f, 1.0f);
                    GL.End();
                }
            }
            else
            {
                GL.Begin(PrimitiveType.Quads);
                GL.Normal3(0.0f, 0.0f, -1.0f);  // tylna ściana (w płaszczyźnie XY)
                GL.Vertex4(0.5f, -0.5f, -0.5f, 1.0f);
                GL.Vertex4(-0.5f, -0.5f, -0.5f, 1.0f);
                GL.Vertex4(-0.5f, 0.5f, -0.5f, 1.0f);
                GL.Vertex4(0.5f, 0.5f, -0.5f, 1.0f);
                GL.End();
            }
            GL.PopMatrix();
        }

        private void DrawFrame(Vector3 position)
        {
            int calcX = (int)Math.Floor(position.X), calcY = (int)Math.Floor(position.Y), calcZ = (int)Math.Floor(position.Z);
            GL.PushMatrix();
            GL.Translate(position.X, position.Y, position.Z);
            GL.ColorMaterial(MaterialFace.Front, ColorMaterialParameter.AmbientAndDiffuse);
            GL.Color3(Color.Black);
            GL.LineWidth(1.5f);
            if (calcZ > 0 && calcY < boundaries_Y - 1)
            {
                if ((!cubeMatrix[calcX, calcY, calcZ - 1] && !cubeMatrix[calcX, calcY + 1, calcZ])
                    || (cubeMatrix[calcX, calcY, calcZ - 1] && cubeMatrix[calcX, calcY + 1, calcZ - 1] && !cubeMatrix[calcX, calcY + 1, calcZ])
                    || (cubeMatrix[calcX, calcY + 1, calcZ - 1] && cubeMatrix[calcX, calcY + 1, calcZ] && !cubeMatrix[calcX, calcY, calcZ - 1])
                    || (!cubeMatrix[calcX, calcY + 1, calcZ - 1] && cubeMatrix[calcX, calcY + 1, calcZ] && cubeMatrix[calcX, calcY, calcZ - 1]))
                {
                    GL.Begin(PrimitiveType.Lines);
                    GL.Normal3(0.0f, 1.0f, -1.0f);
                    GL.Vertex4(+0.51f, +0.51f, -0.51f, 1.0f);
                    GL.Vertex4(-0.51f, +0.51f, -0.51f, 1.0f);
                    GL.End();
                }
            }
            else
            {
                GL.Begin(PrimitiveType.Lines);
                GL.Normal3(0.0f, 1.0f, -1.0f);
                GL.Vertex4(+0.51f, +0.51f, -0.51f, 1.0f);
                GL.Vertex4(-0.51f, +0.51f, -0.51f, 1.0f);
                GL.End();
            }
            if (calcZ < boundaries_Z - 1 && calcY < boundaries_Y - 1)
            {
                if ((!cubeMatrix[calcX, calcY, calcZ + 1] && !cubeMatrix[calcX, calcY + 1, calcZ])
                    || (cubeMatrix[calcX, calcY, calcZ + 1] && cubeMatrix[calcX, calcY + 1, calcZ + 1] && !cubeMatrix[calcX, calcY + 1, calcZ])
                    || (cubeMatrix[calcX, calcY + 1, calcZ + 1] && cubeMatrix[calcX, calcY + 1, calcZ] && !cubeMatrix[calcX, calcY, calcZ + 1])
                    || (!cubeMatrix[calcX, calcY + 1, calcZ + 1] && cubeMatrix[calcX, calcY + 1, calcZ] && cubeMatrix[calcX, calcY, calcZ + 1]))
                {
                    GL.Begin(PrimitiveType.Lines);
                    GL.Vertex4(+0.51f, +0.51f, +0.51f, 1.0f);
                    GL.Vertex4(-0.51f, +0.51f, +0.51f, 1.0f);
                    GL.End();
                }
            }
            else
            {
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex4(+0.51f, +0.51f, +0.51f, 1.0f);
                GL.Vertex4(-0.51f, +0.51f, +0.51f, 1.0f);
                GL.End();
            }
            if (calcX > 0 && calcY < boundaries_Y - 1)
            {
                if ((!cubeMatrix[calcX - 1, calcY, calcZ] && !cubeMatrix[calcX, calcY + 1, calcZ])
                    || (cubeMatrix[calcX - 1, calcY, calcZ] && cubeMatrix[calcX - 1, calcY + 1, calcZ] && !cubeMatrix[calcX, calcY + 1, calcZ])
                    || (cubeMatrix[calcX - 1, calcY + 1, calcZ] && cubeMatrix[calcX, calcY + 1, calcZ] && !cubeMatrix[calcX - 1, calcY, calcZ])
                    || (!cubeMatrix[calcX - 1, calcY + 1, calcZ] && cubeMatrix[calcX, calcY + 1, calcZ] && cubeMatrix[calcX - 1, calcY, calcZ]))
                {
                    GL.Begin(PrimitiveType.Lines);
                    GL.Vertex4(-0.51f, +0.51f, +0.51f, 1.0f);
                    GL.Vertex4(-0.51f, +0.51f, -0.51f, 1.0f);
                    GL.End();
                }
            }
            else
            {
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex4(-0.51f, +0.51f, +0.51f, 1.0f);
                GL.Vertex4(-0.51f, +0.51f, -0.51f, 1.0f);
                GL.End();
            }
            if (calcX < boundaries_X - 1 && calcY < boundaries_Y - 1)
            {
                if ((!cubeMatrix[calcX + 1, calcY, calcZ] && !cubeMatrix[calcX, calcY + 1, calcZ])
                    || (cubeMatrix[calcX + 1, calcY, calcZ] && cubeMatrix[calcX + 1, calcY + 1, calcZ] && !cubeMatrix[calcX, calcY + 1, calcZ])
                    || (cubeMatrix[calcX + 1, calcY + 1, calcZ] && cubeMatrix[calcX, calcY + 1, calcZ] && !cubeMatrix[calcX + 1, calcY, calcZ])
                    || (!cubeMatrix[calcX + 1, calcY + 1, calcZ] && cubeMatrix[calcX, calcY + 1, calcZ] && cubeMatrix[calcX + 1, calcY, calcZ]))
                {
                    GL.Begin(PrimitiveType.Lines);
                    GL.Vertex4(+0.51f, +0.51f, +0.51f, 1.0f);
                    GL.Vertex4(+0.51f, +0.51f, -0.51f, 1.0f);
                    GL.End();
                }
            }
            else
            {
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex4(+0.51f, +0.51f, +0.51f, 1.0f);
                GL.Vertex4(+0.51f, +0.51f, -0.51f, 1.0f);
                GL.End();
            }
            if (calcZ > 0 && calcY > 0)
            {
                if ((!cubeMatrix[calcX, calcY, calcZ - 1] && !cubeMatrix[calcX, calcY - 1, calcZ])
                    || (cubeMatrix[calcX, calcY, calcZ - 1] && cubeMatrix[calcX, calcY - 1, calcZ - 1] && !cubeMatrix[calcX, calcY - 1, calcZ])
                    || (cubeMatrix[calcX, calcY - 1, calcZ - 1] && cubeMatrix[calcX, calcY - 1, calcZ] && !cubeMatrix[calcX, calcY, calcZ - 1])
                    || (!cubeMatrix[calcX, calcY - 1, calcZ - 1] && cubeMatrix[calcX, calcY - 1, calcZ] && cubeMatrix[calcX, calcY, calcZ - 1]))
                {
                    GL.Begin(PrimitiveType.Lines);
                    GL.Vertex4(-0.51f, -0.51f, -0.51f, 1.0f);
                    GL.Vertex4(+0.51f, -0.51f, -0.51f, 1.0f);
                    GL.End();
                }
            }
            else
            {
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex4(-0.51f, -0.51f, -0.51f, 1.0f);
                GL.Vertex4(+0.51f, -0.51f, -0.51f, 1.0f);
                GL.End();
            }
            if (calcX > 0 && calcY > 0)
            {
                if ((!cubeMatrix[calcX - 1, calcY, calcZ] && !cubeMatrix[calcX, calcY - 1, calcZ])
                    || (cubeMatrix[calcX - 1, calcY, calcZ] && cubeMatrix[calcX - 1, calcY - 1, calcZ] && !cubeMatrix[calcX, calcY - 1, calcZ])
                    || (cubeMatrix[calcX - 1, calcY - 1, calcZ] && cubeMatrix[calcX, calcY - 1, calcZ] && !cubeMatrix[calcX - 1, calcY, calcZ])
                    || (!cubeMatrix[calcX - 1, calcY - 1, calcZ] && cubeMatrix[calcX, calcY - 1, calcZ] && cubeMatrix[calcX - 1, calcY, calcZ]))
                {
                    GL.Begin(PrimitiveType.Lines);
                    GL.Vertex4(-0.51f, -0.51f, +0.51f, 1.0f);
                    GL.Vertex4(-0.51f, -0.51f, -0.51f, 1.0f);
                    GL.End();
                }
            }
            else
            {
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex4(-0.51f, -0.51f, +0.51f, 1.0f);
                GL.Vertex4(-0.51f, -0.51f, -0.51f, 1.0f);
                GL.End();
            }
            if (calcX < boundaries_X - 1 && calcY > 0)
            {
                if ((!cubeMatrix[calcX + 1, calcY, calcZ] && !cubeMatrix[calcX, calcY - 1, calcZ])
                    || (cubeMatrix[calcX + 1, calcY, calcZ] && cubeMatrix[calcX + 1, calcY - 1, calcZ] && !cubeMatrix[calcX, calcY - 1, calcZ])
                    || (cubeMatrix[calcX + 1, calcY - 1, calcZ] && cubeMatrix[calcX, calcY - 1, calcZ] && !cubeMatrix[calcX + 1, calcY, calcZ])
                    || (!cubeMatrix[calcX + 1, calcY - 1, calcZ] && cubeMatrix[calcX, calcY - 1, calcZ] && cubeMatrix[calcX + 1, calcY, calcZ]))
                {
                    GL.Begin(PrimitiveType.Lines);
                    GL.Vertex4(+0.51f, -0.51f, +0.51f, 1.0f);
                    GL.Vertex4(+0.51f, -0.51f, -0.51f, 1.0f);
                    GL.End();
                }
            }
            else
            {
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex4(+0.51f, -0.51f, +0.51f, 1.0f);
                GL.Vertex4(+0.51f, -0.51f, -0.51f, 1.0f);
                GL.End();
            }
            if (calcZ < boundaries_Z - 1 && calcY > 0)
            {
                if ((!cubeMatrix[calcX, calcY, calcZ + 1] && !cubeMatrix[calcX, calcY - 1, calcZ])
                    || (cubeMatrix[calcX, calcY, calcZ + 1] && cubeMatrix[calcX, calcY - 1, calcZ + 1] && !cubeMatrix[calcX, calcY - 1, calcZ])
                    || (cubeMatrix[calcX, calcY - 1, calcZ + 1] && cubeMatrix[calcX, calcY - 1, calcZ] && !cubeMatrix[calcX, calcY, calcZ + 1])
                    || (!cubeMatrix[calcX, calcY - 1, calcZ + 1] && cubeMatrix[calcX, calcY - 1, calcZ] && cubeMatrix[calcX, calcY, calcZ + 1]))
                {
                    GL.Begin(PrimitiveType.Lines);
                    GL.Vertex4(+0.51f, -0.51f, +0.51f, 1.0f);
                    GL.Vertex4(-0.51f, -0.51f, +0.51f, 1.0f);
                    GL.End();
                }
            }
            else
            {
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex4(+0.51f, -0.51f, +0.51f, 1.0f);
                GL.Vertex4(-0.51f, -0.51f, +0.51f, 1.0f);
                GL.End();
            }
            if (calcX < boundaries_X - 1 && calcZ < boundaries_Z - 1)
            {
                if ((!cubeMatrix[calcX, calcY, calcZ + 1] && !cubeMatrix[calcX + 1, calcY, calcZ])
                    || (cubeMatrix[calcX, calcY, calcZ + 1] && cubeMatrix[calcX + 1, calcY, calcZ + 1] && !cubeMatrix[calcX + 1, calcY, calcZ])
                    || (cubeMatrix[calcX + 1, calcY, calcZ + 1] && cubeMatrix[calcX + 1, calcY, calcZ] && !cubeMatrix[calcX, calcY, calcZ + 1])
                    || (!cubeMatrix[calcX + 1, calcY, calcZ + 1] && cubeMatrix[calcX + 1, calcY, calcZ] && cubeMatrix[calcX, calcY, calcZ + 1]))
                {
                    GL.Begin(PrimitiveType.Lines);
                    GL.Vertex4(+0.51f, -0.51f, +0.51f, 1.0f);
                    GL.Vertex4(+0.51f, +0.51f, +0.51f, 1.0f);
                    GL.End();
                }
            }
            else
            {
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex4(+0.51f, -0.51f, +0.51f, 1.0f);
                GL.Vertex4(+0.51f, +0.51f, +0.51f, 1.0f);
                GL.End();
            }
            if (calcX > 0 && calcZ < boundaries_Z - 1)
            {
                if ((!cubeMatrix[calcX, calcY, calcZ + 1] && !cubeMatrix[calcX - 1, calcY, calcZ])
                    || (cubeMatrix[calcX, calcY, calcZ + 1] && cubeMatrix[calcX - 1, calcY, calcZ + 1] && !cubeMatrix[calcX - 1, calcY, calcZ])
                    || (cubeMatrix[calcX - 1, calcY, calcZ + 1] && cubeMatrix[calcX - 1, calcY, calcZ] && !cubeMatrix[calcX, calcY, calcZ + 1])
                    || (!cubeMatrix[calcX - 1, calcY, calcZ + 1] && cubeMatrix[calcX - 1, calcY, calcZ] && cubeMatrix[calcX, calcY, calcZ + 1]))
                {
                    GL.Begin(PrimitiveType.Lines);
                    GL.Vertex4(-0.51f, +0.51f, +0.51f, 1.0f);
                    GL.Vertex4(-0.51f, -0.51f, +0.51f, 1.0f);
                    GL.End();
                }
            }
            else
            {
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex4(-0.51f, +0.51f, +0.51f, 1.0f);
                GL.Vertex4(-0.51f, -0.51f, +0.51f, 1.0f);
                GL.End();
            }
            if (calcX < boundaries_X - 1 && calcZ > 0)
            {
                if ((!cubeMatrix[calcX, calcY, calcZ - 1] && !cubeMatrix[calcX + 1, calcY, calcZ])
                    || (cubeMatrix[calcX, calcY, calcZ - 1] && cubeMatrix[calcX + 1, calcY, calcZ - 1] && !cubeMatrix[calcX + 1, calcY, calcZ])
                    || (cubeMatrix[calcX + 1, calcY, calcZ - 1] && cubeMatrix[calcX + 1, calcY, calcZ] && !cubeMatrix[calcX, calcY, calcZ - 1])
                    || (!cubeMatrix[calcX + 1, calcY, calcZ - 1] && cubeMatrix[calcX + 1, calcY, calcZ] && cubeMatrix[calcX, calcY, calcZ - 1]))
                {
                    GL.Begin(PrimitiveType.Lines);
                    GL.Vertex4(+0.51f, +0.51f, -0.51f, 1.0f);
                    GL.Vertex4(+0.51f, -0.51f, -0.51f, 1.0f);
                    GL.End();
                }
            }
            else
            {
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex4(+0.51f, -0.51f, -0.51f, 1.0f);
                GL.Vertex4(+0.51f, +0.51f, -0.51f, 1.0f);
                GL.End();
            }
            if (calcX > 0 && calcZ > 0)
            {
                if ((!cubeMatrix[calcX, calcY, calcZ - 1] && !cubeMatrix[calcX - 1, calcY, calcZ])
                    || (cubeMatrix[calcX, calcY, calcZ - 1] && cubeMatrix[calcX - 1, calcY, calcZ - 1] && !cubeMatrix[calcX - 1, calcY, calcZ])
                    || (cubeMatrix[calcX - 1, calcY, calcZ - 1] && cubeMatrix[calcX - 1, calcY, calcZ] && !cubeMatrix[calcX, calcY, calcZ - 1])
                    || (!cubeMatrix[calcX - 1, calcY, calcZ - 1] && cubeMatrix[calcX - 1, calcY, calcZ] && cubeMatrix[calcX, calcY, calcZ - 1]))
                {
                    GL.Begin(PrimitiveType.Lines);
                    GL.Vertex4(-0.51f, +0.51f, -0.51f, 1.0f);
                    GL.Vertex4(-0.51f, -0.51f, -0.51f, 1.0f);
                    GL.End();
                }
            }
            else
            {
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex4(-0.51f, +0.51f, -0.51f, 1.0f);
                GL.Vertex4(-0.51f, -0.51f, -0.51f, 1.0f);
                GL.End();
            }
            GL.PopMatrix();
        }

        private void DrawBox(float size, Color color)
        {
            GL.ColorMaterial(MaterialFace.Front, ColorMaterialParameter.AmbientAndDiffuse);
            GL.Color3(Color.Black);
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
            GL.ColorMaterial(MaterialFace.Front, ColorMaterialParameter.AmbientAndDiffuse);
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

        void DrawSelector(float x,float y,float z,Color color)
        {
            GL.PushMatrix();
            GL.Translate(x, y, z);
            GL.ColorMaterial(MaterialFace.Front, ColorMaterialParameter.AmbientAndDiffuse);
            GL.Color3(color);
            GL.Begin(PrimitiveType.Lines);
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

        void DrawAreaSelector(Vector3 p1,Vector3 p2, Color color)
        {
            GL.PushMatrix();
            GL.LineWidth(3.0f);
            float lowX, hiX, lowY, hiY, lowZ, hiZ, distX, distY, distZ;
            if (p1.X>p2.X)
            {
                lowX = p2.X;
                hiX = p1.X;
            }
            else
            {
                lowX = p1.X;
                hiX = p2.X;
            }
            if (p1.Y > p2.Y)
            {
                lowY = p2.Y;
                hiY = p1.Y;
            }
            else
            {
                lowY = p1.Y;
                hiY = p2.Y;
            }
            if (p1.Z > p2.Z)
            {
                lowZ = p2.Z;
                hiZ = p1.Z;
            }
            else
            {
                lowZ = p1.Z;
                hiZ = p2.Z;
            }
            distX = Math.Abs(p1.X - p2.X) +1.0f;
            distY = Math.Abs(p1.Y - p2.Y) + 1.0f;
            distZ = Math.Abs(p1.Z - p2.Z) + 1.0f;
            GL.Translate((p1.X+p2.X)/2.0f, (p1.Y + p2.Y) / 2.0f, (p1.Z + p2.Z) / 2.0f);
            GL.ColorMaterial(MaterialFace.Front, ColorMaterialParameter.AmbientAndDiffuse);
            GL.Color3(color);
            GL.Begin(PrimitiveType.Lines);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(-distX / 2.0f, -distY / 2.0f, -distZ / 2.0f);
            GL.Vertex3(+distX / 2.0f, -distY / 2.0f, -distZ / 2.0f);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(+distX / 2.0f, -distY / 2.0f, -distZ / 2.0f);
            GL.Vertex3(+distX / 2.0f, +distY / 2.0f, -distZ / 2.0f);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(+distX / 2.0f, +distY / 2.0f, -distZ / 2.0f);
            GL.Vertex3(-distX / 2.0f, +distY / 2.0f, -distZ / 2.0f);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(-distX / 2.0f, +distY / 2.0f, -distZ / 2.0f);
            GL.Vertex3(-distX / 2.0f, -distY / 2.0f, -distZ / 2.0f);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(-distX / 2.0f, -distY / 2.0f, +distZ / 2.0f);
            GL.Vertex3(+distX / 2.0f, -distY / 2.0f, +distZ / 2.0f);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(+distX / 2.0f, -distY / 2.0f, +distZ / 2.0f);
            GL.Vertex3(+distX / 2.0f, +distY / 2.0f, +distZ / 2.0f);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(+distX / 2.0f, +distY / 2.0f, +distZ / 2.0f);
            GL.Vertex3(-distX / 2.0f, +distY / 2.0f, +distZ / 2.0f);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(-distX / 2.0f, +distY / 2.0f, +distZ / 2.0f);
            GL.Vertex3(-distX / 2.0f, -distY / 2.0f, +distZ / 2.0f);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(-distX / 2.0f, -distY / 2.0f, +distZ / 2.0f);
            GL.Vertex3(-distX / 2.0f, -distY / 2.0f, -distZ / 2.0f);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(+distX / 2.0f, -distY / 2.0f, +distZ / 2.0f);
            GL.Vertex3(+distX / 2.0f, -distY / 2.0f, -distZ / 2.0f);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(-distX / 2.0f, +distY / 2.0f, +distZ / 2.0f);
            GL.Vertex3(-distX / 2.0f, +distY / 2.0f, -distZ / 2.0f);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(+distX / 2.0f, +distY / 2.0f, +distZ / 2.0f);
            GL.Vertex3(+distX / 2.0f, +distY / 2.0f, -distZ / 2.0f);
            GL.End();
            GL.LineWidth(1.0f);
            GL.PopMatrix();
        }

        void drawCircle(float radius)
        {
            GL.ColorMaterial(MaterialFace.Front, ColorMaterialParameter.AmbientAndDiffuse);
            GL.Color3(Color.White);
            //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.AmbientAndDiffuse, Color.White);
            GL.Begin(PrimitiveType.TriangleFan);

            for (int i = 0; i < 360; i++)
            {
                double degInRad = i * 3.1416 / 180;
                GL.Vertex2(Math.Cos(degInRad) * radius, Math.Sin(degInRad) * radius);
            }
            GL.End();
        }

        void drawLightSource(Vector3 position)
        {
            GL.PushMatrix();
            GL.Translate(position.X, position.Y, position.Z);
            GL.ColorMaterial(MaterialFace.Front, ColorMaterialParameter.AmbientAndDiffuse);
            GL.Color3(Color.White);
            GL.Begin(PrimitiveType.Quads);
            GL.Normal3(0.0f, 1.0f, 0.0f);   // górna ściana (w płaszczyźnie XZ)
            GL.Vertex4(0.5f, 0.5f, 0.5f, 1.0f);
            GL.Vertex4(0.5f, 0.5f, -0.5f, 1.0f);
            GL.Vertex4(-0.5f, 0.5f, -0.5f, 1.0f);
            GL.Vertex4(-0.5f, 0.5f, 0.5f, 1.0f);
            GL.End();
            GL.Begin(PrimitiveType.Quads);
            GL.Normal3(0.0f, 0.0f, 1.0f);   // przednia ściana (w płaszczyźnie XY)
            GL.Vertex4(0.5f, 0.5f, 0.5f, 1.0f);
            GL.Vertex4(-0.5f, 0.5f, 0.5f, 1.0f);
            GL.Vertex4(-0.5f, -0.5f, 0.5f, 1.0f);
            GL.Vertex4(0.5f, -0.5f, 0.5f, 1.0f);
            GL.End();
            GL.Begin(PrimitiveType.Quads);
            GL.Normal3(1.0f, 0.0f, 0.0f);   // prawa ściana (w płaszczyźnie YZ)
            GL.Vertex4(0.5f, 0.5f, 0.5f, 1.0f);
            GL.Vertex4(0.5f, -0.5f, 0.5f, 1.0f);
            GL.Vertex4(0.5f, -0.5f, -0.5f, 1.0f);
            GL.Vertex4(0.5f, 0.5f, -0.5f, 1.0f);
            GL.End();
            GL.Begin(PrimitiveType.Quads);
            GL.Normal3(-1.0f, 0.0f, 0.0f);  // lewa ściana (w płaszczyźnie YZ)
            GL.Vertex4(-0.5f, 0.5f, 0.5f, 1.0f);
            GL.Vertex4(-0.5f, 0.5f, -0.5f, 1.0f);
            GL.Vertex4(-0.5f, -0.5f, -0.5f, 1.0f);
            GL.Vertex4(-0.5f, -0.5f, 0.5f, 1.0f);
            GL.End();
            GL.Begin(PrimitiveType.Quads);
            GL.Normal3(0.0f, -1.0f, 0.0f);  // dolna ściana (w płaszczyźnie XZ)
            GL.Vertex4(-0.5f, -0.5f, 0.5f, 1.0f);
            GL.Vertex4(-0.5f, -0.5f, -0.5f, 1.0f);
            GL.Vertex4(0.5f, -0.5f, -0.5f, 1.0f);
            GL.Vertex4(0.5f, -0.5f, 0.5f, 1.0f);
            GL.End();
            GL.Begin(PrimitiveType.Quads);
            GL.Normal3(0.0f, 0.0f, -1.0f);  // tylna ściana (w płaszczyźnie XY)
            GL.Vertex4(0.5f, -0.5f, -0.5f, 1.0f);
            GL.Vertex4(-0.5f, -0.5f, -0.5f, 1.0f);
            GL.Vertex4(-0.5f, 0.5f, -0.5f, 1.0f);
            GL.Vertex4(0.5f, 0.5f, -0.5f, 1.0f);
            GL.End();
            GL.PopMatrix();
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(Color.SkyBlue);
            //GL.Enable(EnableCap.Lighting);
            //GL.Enable(EnableCap.Light0);
            //GL.Enable(EnableCap.ColorMaterial);
            //GL.Enable(EnableCap.DepthTest);

            cubeMatrix = new bool[(int)boundaries_X, (int)boundaries_Y, (int)boundaries_Z];
            chosenColor = new Color[(int)boundaries_X, (int)boundaries_Y, (int)boundaries_Z];
            numericUpDown1.Minimum = -100;
            numericUpDown1.Maximum = (int)boundaries_X * 2;
            numericUpDown2.Minimum = -100;
            numericUpDown2.Maximum = (int)boundaries_Y * 2;
            numericUpDown3.Minimum = -100;
            numericUpDown3.Maximum = (int)boundaries_Z * 2;
        }

        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.I)
            {
                if (rotation_X< 75.0f)
                    rotation_X += 2.0f;
            }
            if (e.KeyCode == Keys.K)
            {
                if (rotation_X > -75.0f)
                    rotation_X -= 2.0f;
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
            if (e.KeyCode == Keys.D)
            {
                int quarter = (int)(Math.Round(rotation_Y / 90.0f));
                switch(quarter)
                {
                    case 0:
                        {
                            if(activePoint1 && activePoint2)
                            {
                                if (selector.X + 1.0f < boundaries_X && point1.X+1.0f<boundaries_X && point2.X+1.0f<boundaries_X)
                                {
                                    selector.X += 1.0f;
                                    point1.X += 1.0f;
                                    point2.X += 1.0f;
                                }
                            }
                            else if (selector.X + 1.0f < boundaries_X)
                                selector.X += 1.0f;
                            break;
                        }
                    case 1:
                        {
                            if (activePoint1 && activePoint2)
                            {
                                if (selector.Z + 1.0f < boundaries_Z && point1.Z + 1.0f < boundaries_Z && point2.Z + 1.0f < boundaries_Z)
                                {
                                    selector.Z += 1.0f;
                                    point1.Z += 1.0f;
                                    point2.Z += 1.0f;
                                }
                            }
                            else if (selector.Z + 1.0f < boundaries_Z)
                                selector.Z += 1.0f;
                            break;
                        }
                    case 2:
                        {
                            if (activePoint1 && activePoint2)
                            {
                                if (selector.X > 1.0f && point1.X > 1.0f && point2.X > 1.0f)
                                {
                                    selector.X -= 1.0f;
                                    point1.X -= 1.0f;
                                    point2.X -= 1.0f;
                                }
                            }
                            else if (selector.X > 1.0f)
                                selector.X -= 1.0f;
                            break;

                        }
                    case 3:
                        {
                            if (activePoint1 && activePoint2)
                            {
                                if (selector.Z > 1.0f && point1.Z > 1.0f && point2.Z > 1.0f)
                                {
                                    selector.Z -= 1.0f;
                                    point1.Z -= 1.0f;
                                    point2.Z -= 1.0f;
                                }
                            }
                            else if (selector.Z > 1.0f)
                                selector.Z -= 1.0f;
                            break;
                        }
                    default:
                        {
                            if (activePoint1 && activePoint2)
                            {
                                if (selector.X + 1.0f < boundaries_X && point1.X + 1.0f < boundaries_X && point2.X + 1.0f < boundaries_X)
                                {
                                    selector.X += 1.0f;
                                    point1.X += 1.0f;
                                    point2.X += 1.0f;
                                }
                            }
                            else if (selector.X + 1.0f < boundaries_X)
                                selector.X += 1.0f;
                            break;
                        }
                }
            }
            if (e.KeyCode == Keys.A)
            {
                int quarter = (int)(Math.Round(rotation_Y / 90.0f));
                switch (quarter)
                {
                    case 0:
                        {
                            if (activePoint1 && activePoint2)
                            {
                                if (selector.X > 1.0f && point1.X > 1.0f && point2.X > 1.0f)
                                {
                                    selector.X -= 1.0f;
                                    point1.X -= 1.0f;
                                    point2.X -= 1.0f;
                                }
                            }
                            else if (selector.X > 1.0f)
                                selector.X -= 1.0f;
                            break;
                        }
                    case 1:
                        {
                            if (activePoint1 && activePoint2)
                            {
                                if (selector.Z > 1.0f && point1.Z > 1.0f && point2.Z > 1.0f)
                                {
                                    selector.Z -= 1.0f;
                                    point1.Z -= 1.0f;
                                    point2.Z -= 1.0f;
                                }
                            }
                            else if (selector.Z > 1.0f)
                                selector.Z -= 1.0f;
                            break;
                        }
                    case 2:
                        {
                            if (activePoint1 && activePoint2)
                            {
                                if (selector.X + 1.0f < boundaries_X && point1.X + 1.0f < boundaries_X && point2.X + 1.0f < boundaries_X)
                                {
                                    selector.X += 1.0f;
                                    point1.X += 1.0f;
                                    point2.X += 1.0f;
                                }
                            }
                            else if (selector.X + 1.0f < boundaries_X)
                                selector.X += 1.0f;
                            break;
                        }
                    case 3:
                        {
                            if (activePoint1 && activePoint2)
                            {
                                if (selector.Z + 1.0f < boundaries_X && point1.Z + 1.0f < boundaries_X && point2.Z + 1.0f < boundaries_X)
                                {
                                    selector.Z += 1.0f;
                                    point1.Z += 1.0f;
                                    point2.Z += 1.0f;
                                }
                            }
                            else if (selector.Z + 1.0f < boundaries_Z)
                                selector.Z += 1.0f;
                            break;
                        }
                    default:
                        {
                            if (activePoint1 && activePoint2)
                            {
                                if (selector.X > 1.0f && point1.X > 1.0f && point2.X > 1.0f)
                                {
                                    selector.X -= 1.0f;
                                    point1.X -= 1.0f;
                                    point2.X -= 1.0f;
                                }
                            }
                            else if (selector.X > 1.0f)
                                selector.X -= 1.0f;
                            break;
                        }
                }
            }
            if (e.KeyCode == Keys.S)
            {
                int quarter = (int)(Math.Round(rotation_Y / 90.0f));
                switch (quarter)
                {
                    case 0:
                        {
                            if (activePoint1 && activePoint2)
                            {
                                if (selector.Z + 1.0f < boundaries_X && point1.Z + 1.0f < boundaries_X && point2.Z + 1.0f < boundaries_X)
                                {
                                    selector.Z += 1.0f;
                                    point1.Z += 1.0f;
                                    point2.Z += 1.0f;
                                }
                            }
                            else if (selector.Z + 1.0f < boundaries_Z)
                                selector.Z += 1.0f;
                            break;
                        }
                    case 1:
                        {
                            if (activePoint1 && activePoint2)
                            {
                                if (selector.X > 1.0f && point1.X > 1.0f && point2.X > 1.0f)
                                {
                                    selector.X -= 1.0f;
                                    point1.X -= 1.0f;
                                    point2.X -= 1.0f;
                                }
                            }
                            else if (selector.X > 1.0f)
                                selector.X -= 1.0f;
                            break;
                        }
                    case 2:
                        {
                            if (activePoint1 && activePoint2)
                            {
                                if (selector.Z > 1.0f && point1.Z > 1.0f && point2.Z > 1.0f)
                                {
                                    selector.Z -= 1.0f;
                                    point1.Z -= 1.0f;
                                    point2.Z -= 1.0f;
                                }
                            }
                            else if (selector.Z > 1.0f)
                                selector.Z -= 1.0f;
                            break;
                        }
                    case 3:
                        {
                            if (activePoint1 && activePoint2)
                            {
                                if (selector.X + 1.0f < boundaries_X && point1.X + 1.0f < boundaries_X && point2.X + 1.0f < boundaries_X)
                                {
                                    selector.X += 1.0f;
                                    point1.X += 1.0f;
                                    point2.X += 1.0f;
                                }
                            }
                            else if (selector.X + 1.0f < boundaries_X)
                                selector.X += 1.0f;
                            break;
                        }
                    default:
                        {
                            if (activePoint1 && activePoint2)
                            {
                                if (selector.Z + 1.0f < boundaries_X && point1.Z + 1.0f < boundaries_X && point2.Z + 1.0f < boundaries_X)
                                {
                                    selector.Z += 1.0f;
                                    point1.Z += 1.0f;
                                    point2.Z += 1.0f;
                                }
                            }
                            else if (selector.Z + 1.0f < boundaries_Z)
                                selector.Z += 1.0f;
                            break;
                        }
                }
            }
            if (e.KeyCode == Keys.W)
            {
                int quarter = (int)(Math.Round(rotation_Y / 90.0f));
                switch (quarter)
                {
                    case 0:
                        {
                            if (activePoint1 && activePoint2)
                            {
                                if (selector.Z > 1.0f && point1.Z > 1.0f && point2.Z > 1.0f)
                                {
                                    selector.Z -= 1.0f;
                                    point1.Z -= 1.0f;
                                    point2.Z -= 1.0f;
                                }
                            }
                            else if (selector.Z > 1.0f)
                                selector.Z -= 1.0f;
                            break;
                        }
                    case 1:
                        {
                            if (activePoint1 && activePoint2)
                            {
                                if (selector.X + 1.0f < boundaries_X && point1.X + 1.0f < boundaries_X && point2.X + 1.0f < boundaries_X)
                                {
                                    selector.X += 1.0f;
                                    point1.X += 1.0f;
                                    point2.X += 1.0f;
                                }
                            }
                            else if (selector.X + 1.0f < boundaries_X)
                                selector.X += 1.0f;
                            break;
                        }
                    case 2:
                        {
                            if (activePoint1 && activePoint2)
                            {
                                if (selector.Z + 1.0f < boundaries_X && point1.Z + 1.0f < boundaries_X && point2.Z + 1.0f < boundaries_X)
                                {
                                    selector.Z += 1.0f;
                                    point1.Z += 1.0f;
                                    point2.Z += 1.0f;
                                }
                            }
                            else if (selector.Z + 1.0f < boundaries_Z)
                                selector.Z += 1.0f;
                            break;
                        }
                    case 3:
                        {
                            if (activePoint1 && activePoint2)
                            {
                                if (selector.X > 1.0f && point1.X > 1.0f && point2.X > 1.0f)
                                {
                                    selector.X -= 1.0f;
                                    point1.X -= 1.0f;
                                    point2.X -= 1.0f;
                                }
                            }
                            else if (selector.X > 1.0f)
                                selector.X -= 1.0f;
                            break;
                        }
                    default:
                        {
                            if (activePoint1 && activePoint2)
                            {
                                if (selector.Z > 1.0f && point1.Z > 1.0f && point2.Z > 1.0f)
                                {
                                    selector.Z -= 1.0f;
                                    point1.Z -= 1.0f;
                                    point2.Z -= 1.0f;
                                }
                            }
                            else if (selector.Z > 1.0f)
                                selector.Z -= 1.0f;
                            break;
                        }
                }
            }
            if (e.KeyCode == Keys.C)
            {
                if (activePoint1 && activePoint2)
                {
                    if (selector.Y + 1.0f < boundaries_Y && point1.Y + 1.0f < boundaries_Y && point2.Y + 1.0f < boundaries_Y)
                    {
                        selector.Y += 1.0f;
                        point1.Y += 1.0f;
                        point2.Y += 1.0f;
                    }
                }
                else if (selector.Y + 1.0f < boundaries_Y)
                    selector.Y += 1.0f;
            }
            if (e.KeyCode == Keys.Z)
            {
                if (activePoint1 && activePoint2)
                {
                    if (selector.Y > 1.0f && point1.Y > 1.0f && point2.Y > 1.0f)
                    {
                        selector.Y -= 1.0f;
                        point1.Y -= 1.0f;
                        point2.Y -= 1.0f;
                    }
                }
                else if (selector.Y > 1.0f)
                    selector.Y -= 1.0f;
            }
            if (e.KeyCode == Keys.G)
            {
                if (activePoint1 && activePoint2)
                {
                    float lowX, hiX, lowY, hiY, lowZ, hiZ, distX, distY, distZ;
                    if (point1.X > point2.X)
                    {
                        lowX = point2.X;
                        hiX = point1.X;
                    }
                    else
                    {
                        lowX = point1.X;
                        hiX = point2.X;
                    }
                    if (point1.Y > point2.Y)
                    {
                        lowY = point2.Y;
                        hiY = point1.Y;
                    }
                    else
                    {
                        lowY = point1.Y;
                        hiY = point2.Y;
                    }
                    if (point1.Z > point2.Z)
                    {
                        lowZ = point2.Z;
                        hiZ = point1.Z;
                    }
                    else
                    {
                        lowZ = point1.Z;
                        hiZ = point2.Z;
                    }
                    distX = Math.Abs(point1.X - point2.X) +1.0f;
                    distY = Math.Abs(point1.Y - point2.Y) + 1.0f;
                    distZ = Math.Abs(point1.Z - point2.Z) + 1.0f;
                    if (onlyBuild)
                    {
                        for (int i=(int)(Math.Floor(lowX)); i <= (int)(Math.Floor(hiX)); i++)
                        {
                            for (int j = (int)(Math.Floor(lowY)); j <= (int)(Math.Floor(hiY)); j++)
                            {
                                for (int k = (int)(Math.Floor(lowZ)); k <= (int)(Math.Floor(hiZ)); k++)
                                {
                                    cubeMatrix[i, j, k] = true;
                                    chosenColor[i, j, k] = currentColor;
                                }
                            }
                        }
                    }
                    else if (onlyErase)
                    {
                        for (int i = (int)(Math.Floor(lowX)); i <= (int)(Math.Floor(hiX)); i++)
                        {
                            for (int j = (int)(Math.Floor(lowY)); j <= (int)(Math.Floor(hiY)); j++)
                            {
                                for (int k = (int)(Math.Floor(lowZ)); k <= (int)(Math.Floor(hiZ)); k++)
                                {
                                    cubeMatrix[i, j, k] = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = (int)(Math.Floor(lowX)); i <= (int)(Math.Floor(hiX)); i++)
                        {
                            for (int j = (int)(Math.Floor(lowY)); j <= (int)(Math.Floor(hiY)); j++)
                            {
                                for (int k = (int)(Math.Floor(lowZ)); k <= (int)(Math.Floor(hiZ)); k++)
                                {
                                    if (!cubeMatrix[i, j, k])
                                    {
                                        chosenColor[i, j, k] = currentColor;
                                    }
                                    cubeMatrix[i, j, k] = cubeMatrix[i, j, k] ? false : true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (onlyBuild)
                    {
                        cubeMatrix[(int)Math.Floor(selector.X), (int)Math.Floor(selector.Y), (int)Math.Floor(selector.Z)] = true;
                        chosenColor[(int)Math.Floor(selector.X), (int)Math.Floor(selector.Y), (int)Math.Floor(selector.Z)] = currentColor;
                    }
                    else if (onlyErase)
                    {
                        cubeMatrix[(int)Math.Floor(selector.X), (int)Math.Floor(selector.Y), (int)Math.Floor(selector.Z)] = false;
                    }
                    else
                    {
                        cubeMatrix[(int)Math.Floor(selector.X), (int)Math.Floor(selector.Y), (int)Math.Floor(selector.Z)] = cubeMatrix[(int)Math.Floor(selector.X), (int)Math.Floor(selector.Y), (int)Math.Floor(selector.Z)] ? false : true;
                        chosenColor[(int)Math.Floor(selector.X), (int)Math.Floor(selector.Y), (int)Math.Floor(selector.Z)] = currentColor;
                    }
                }
            }
            glControl1.Invalidate();
        }

        private void WartoscWielkosciSceny_ValueChanged(object sender, EventArgs e)
        {
            sceneSize = (float)WartoscWielkosciSceny.Value;
        }

        private void UtworzScene_Click(object sender, EventArgs e)
        {
            boundaries_X = sceneSize;
            boundaries_Y = sceneSize;
            boundaries_Z = sceneSize;
            cubeMatrix = new bool[(int)boundaries_X, (int)boundaries_Y, (int)boundaries_Z];
            chosenColor = new Color[(int)boundaries_X, (int)boundaries_Y, (int)boundaries_Z];
            selector = new Vector3(0.5f, 0.5f, 0.5f);
            glControl1.Invalidate();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e) //wyświetl płaszczyzne XY
        {
            showXY = radioButton1.Checked;
            glControl1.Invalidate();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e) //wyświetl płaszczyzne XZ
        {
            showXZ = radioButton2.Checked;
            glControl1.Invalidate();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e) //wyświetl płaszczyzne YZ
        {
            showYZ= radioButton3.Checked;
            glControl1.Invalidate();
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e) //wszystkie płaszczyzny
        {

        }

        private void ZasiegOgraniczenia_ValueChanged(object sender, EventArgs e) //zmień zasięg widoku
        {
            viewRange = (float)ZasiegOgraniczenia.Value;
            glControl1.Invalidate();
        }

        private void button4_Click(object sender, EventArgs e) //włącz/wyłącz zasięg widoku
        {
            activateRangeView = !activateRangeView;
            glControl1.Invalidate();
        }

        private void button5_Click(object sender, EventArgs e) //punkt1
        {
            point1 = selector;
            activePoint1 = true;
            glControl1.Invalidate();
        }

        private void button6_Click(object sender, EventArgs e) //punkt2
        {
            point2 = selector;
            activePoint2 = true;
            glControl1.Invalidate();
        }

        private void button7_Click(object sender, EventArgs e) //funkcja kopiująca
        {
            if (activePoint1 && activePoint2)
            {
                float lowX, hiX, lowY, hiY, lowZ, hiZ, distX, distY, distZ;
                if (point1.X > point2.X)
                {
                    lowX = point2.X;
                    hiX = point1.X;
                }
                else
                {
                    lowX = point1.X;
                    hiX = point2.X;
                }
                if (point1.Y > point2.Y)
                {
                    lowY = point2.Y;
                    hiY = point1.Y;
                }
                else
                {
                    lowY = point1.Y;
                    hiY = point2.Y;
                }
                if (point1.Z > point2.Z)
                {
                    lowZ = point2.Z;
                    hiZ = point1.Z;
                }
                else
                {
                    lowZ = point1.Z;
                    hiZ = point2.Z;
                }
                distX = Math.Abs(point1.X - point2.X);
                distY = Math.Abs(point1.Y - point2.Y);
                distZ = Math.Abs(point1.Z - point2.Z);
                copyMatrix = new bool[(int)distX+1, (int)distY+1, (int)distZ+1];
                colorMatrix = new Color[(int)distX+1, (int)distY+1, (int)distZ+1];
                for (int i = 0; i < distX+1; i++)
                {
                    for (int j = 0; j < distY+1; j++)
                    {
                        for (int k = 0; k < distZ+1; k++)
                        {
                            copyMatrix[i, j, k] = cubeMatrix[(int)(i +Math.Floor(lowX)), (int)(j + Math.Floor(lowY)), (int)(k + Math.Floor(lowZ))];
                            colorMatrix[i, j, k] = chosenColor[(int)(i + Math.Floor(lowX)), (int)(j + Math.Floor(lowY)), (int)(k + Math.Floor(lowZ))];
                        }
                    }
                }
                copyReady = true;
            }
        }

        private void button8_Click(object sender, EventArgs e) //funkcja wklejająca
        {
            if (activePoint1 && activePoint2 && copyReady)
            {
                float lowX, hiX, lowY, hiY, lowZ, hiZ, distX, distY, distZ;
                if (point1.X > point2.X)
                {
                    lowX = point2.X;
                    hiX = point1.X;
                }
                else
                {
                    lowX = point1.X;
                    hiX = point2.X;
                }
                if (point1.Y > point2.Y)
                {
                    lowY = point2.Y;
                    hiY = point1.Y;
                }
                else
                {
                    lowY = point1.Y;
                    hiY = point2.Y;
                }
                if (point1.Z > point2.Z)
                {
                    lowZ = point2.Z;
                    hiZ = point1.Z;
                }
                else
                {
                    lowZ = point1.Z;
                    hiZ = point2.Z;
                }
                distX = Math.Abs(point1.X - point2.X);
                distY = Math.Abs(point1.Y - point2.Y);
                distZ = Math.Abs(point1.Z - point2.Z);
                for (int i = 0; i < distX+1; i++)
                {
                    for (int j = 0; j < distY+1; j++)
                    {
                        for (int k = 0; k < distZ+1; k++)
                        {
                            cubeMatrix[(int)(i + Math.Floor(lowX)), (int)(j + Math.Floor(lowY)), (int)(k + Math.Floor(lowZ))] = copyMatrix[i, j, k];
                            chosenColor[(int)(i + Math.Floor(lowX)), (int)(j + Math.Floor(lowY)), (int)(k + Math.Floor(lowZ))] = colorMatrix[i, j, k];
                        }
                    }
                }
                glControl1.Invalidate();
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e) //ustawienie wypełniania
        {
            onlyBuild = radioButton4.Checked;
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e) //ustawienie usuwania
        {
            onlyErase = radioButton5.Checked;
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e) //ustawienie negacji
        {
            onlyBuild = false;
            onlyErase = false;
        }

        private void button1_Click(object sender, EventArgs e) //anuluj zaznaczenie obszaru
        {
            activePoint1 = false;
            activePoint2 = false;
            copyReady = false;
            glControl1.Invalidate();
        }

        private void button5_Click_1(object sender, EventArgs e) //zmiana koloru
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                currentColor = colorDialog1.Color;
                textBox2.BackColor = currentColor;
            }
        }

        private void CreateStlFile() //zapis sceny jako plik ASCII STL
        {
            Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "stl files (*.stl)|*.stl";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    myStream.Close();
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName))
                        {
                            sw.WriteLine("solid save1");
                            for (int i = 0; i < boundaries_X; i++)
                            {
                                for (int j = 0; j < boundaries_Y; j++)
                                {
                                    for (int k = 0; k < boundaries_Z; k++)
                                    {
                                        if (cubeMatrix[i, j, k])
                                        {
                                            if (j < boundaries_Y - 1)
                                            {
                                                if (!cubeMatrix[i, j + 1, k])
                                                {
                                                    sw.WriteLine("facet normal 0.0 1.0 0.0");
                                                    sw.WriteLine("outer loop");
                                                    sw.Write("vertex ");
                                                    sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.Write("vertex ");
                                                    sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.Write("vertex ");
                                                    sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.WriteLine("endloop");
                                                    sw.WriteLine("endfacet");
                                                    sw.WriteLine("facet normal 0.0 1.0 0.0");
                                                    sw.WriteLine("outer loop");
                                                    sw.Write("vertex ");
                                                    sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.Write("vertex ");
                                                    sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.Write("vertex ");
                                                    sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.WriteLine("endloop");
                                                    sw.WriteLine("endfacet");
                                                }
                                            }
                                            else
                                            {
                                                sw.WriteLine("facet normal 0.0 1.0 0.0");
                                                sw.WriteLine("outer loop");
                                                sw.Write("vertex ");
                                                sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.Write("vertex ");
                                                sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.Write("vertex ");
                                                sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.WriteLine("endloop");
                                                sw.WriteLine("endfacet");
                                                sw.WriteLine("facet normal 0.0 1.0 0.0");
                                                sw.WriteLine("outer loop");
                                                sw.Write("vertex ");
                                                sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.Write("vertex ");
                                                sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.Write("vertex ");
                                                sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.WriteLine("endloop");
                                                sw.WriteLine("endfacet");
                                            }
                                            if (j > 0)
                                            {
                                                /*GL.Normal3(0.0f, -1.0f, 0.0f);  // dolna ściana (w płaszczyźnie XZ)
                                                GL.Vertex4(-0.5f, -0.5f, 0.5f, 1.0f);
                                                GL.Vertex4(-0.5f, -0.5f, -0.5f, 1.0f);
                                                GL.Vertex4(0.5f, -0.5f, -0.5f, 1.0f);
                                                GL.Vertex4(0.5f, -0.5f, 0.5f, 1.0f);*/
                                                if (!cubeMatrix[i, j - 1, k]) //czy ma być dolna ściana
                                                {
                                                    sw.WriteLine("facet normal 0.0 -1.0 0.0");
                                                    sw.WriteLine("outer loop");
                                                    sw.Write("vertex ");
                                                    sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.Write("vertex ");
                                                    sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.Write("vertex ");
                                                    sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.WriteLine("endloop");
                                                    sw.WriteLine("endfacet");
                                                    sw.WriteLine("facet normal 0.0 -1.0 0.0");
                                                    sw.WriteLine("outer loop");
                                                    sw.Write("vertex ");
                                                    sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.Write("vertex ");
                                                    sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.Write("vertex ");
                                                    sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.WriteLine("endloop");
                                                    sw.WriteLine("endfacet");
                                                }
                                            }
                                            else
                                            {
                                                sw.WriteLine("facet normal 0.0 -1.0 0.0");
                                                sw.WriteLine("outer loop");
                                                sw.Write("vertex ");
                                                sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.Write("vertex ");
                                                sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.Write("vertex ");
                                                sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.WriteLine("endloop");
                                                sw.WriteLine("endfacet");
                                                sw.WriteLine("facet normal 0.0 -1.0 0.0");
                                                sw.WriteLine("outer loop");
                                                sw.Write("vertex ");
                                                sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.Write("vertex ");
                                                sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.Write("vertex ");
                                                sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.WriteLine("endloop");
                                                sw.WriteLine("endfacet");
                                            }
                                            if (k < boundaries_Z - 1)
                                            {
                                                /*GL.Normal3(0.0f, 0.0f, 1.0f);   // przednia ściana (w płaszczyźnie XY)
                                                GL.Vertex4(0.5f, 0.5f, 0.5f, 1.0f);
                                                GL.Vertex4(-0.5f, 0.5f, 0.5f, 1.0f);
                                                GL.Vertex4(-0.5f, -0.5f, 0.5f, 1.0f);
                                                GL.Vertex4(0.5f, -0.5f, 0.5f, 1.0f);*/
                                                if (!cubeMatrix[i, j, k + 1]) //czy ma być przednia ściana
                                                {
                                                    sw.WriteLine("facet normal 0.0 0.0 1.0");
                                                    sw.WriteLine("outer loop");
                                                    sw.Write("vertex ");
                                                    sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.Write("vertex ");
                                                    sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.Write("vertex ");
                                                    sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.WriteLine("endloop");
                                                    sw.WriteLine("endfacet");
                                                    sw.WriteLine("facet normal 0.0 0.0 -1.0");
                                                    sw.WriteLine("outer loop");
                                                    sw.Write("vertex ");
                                                    sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.Write("vertex ");
                                                    sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.Write("vertex ");
                                                    sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.WriteLine("endloop");
                                                    sw.WriteLine("endfacet");
                                                }
                                            }
                                            else
                                            {
                                                sw.WriteLine("facet normal 0.0 0.0 1.0");
                                                sw.WriteLine("outer loop");
                                                sw.Write("vertex ");
                                                sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.Write("vertex ");
                                                sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.Write("vertex ");
                                                sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.WriteLine("endloop");
                                                sw.WriteLine("endfacet");
                                                sw.WriteLine("facet normal 0.0 0.0 -1.0");
                                                sw.WriteLine("outer loop");
                                                sw.Write("vertex ");
                                                sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.Write("vertex ");
                                                sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.Write("vertex ");
                                                sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.WriteLine("endloop");
                                                sw.WriteLine("endfacet");
                                            }
                                            if (k > 0)
                                            {
                                                /*GL.Normal3(0.0f, 0.0f, -1.0f);  // tylna ściana (w płaszczyźnie XY)
                                                GL.Vertex4(0.5f, -0.5f, -0.5f, 1.0f);
                                                GL.Vertex4(-0.5f, -0.5f, -0.5f, 1.0f);
                                                GL.Vertex4(-0.5f, 0.5f, -0.5f, 1.0f);
                                                GL.Vertex4(0.5f, 0.5f, -0.5f, 1.0f);*/
                                                if (!cubeMatrix[i, j, k - 1]) //czy ma być tylna ściana
                                                {
                                                    sw.WriteLine("facet normal 0.0 0.0 -1.0");
                                                    sw.WriteLine("outer loop");
                                                    sw.Write("vertex ");
                                                    sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.Write("vertex ");
                                                    sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.Write("vertex ");
                                                    sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.WriteLine("endloop");
                                                    sw.WriteLine("endfacet");
                                                    sw.WriteLine("facet normal 0.0 0.0 -1.0");
                                                    sw.WriteLine("outer loop");
                                                    sw.Write("vertex ");
                                                    sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.Write("vertex ");
                                                    sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.Write("vertex ");
                                                    sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.WriteLine("endloop");
                                                    sw.WriteLine("endfacet");
                                                }
                                            }
                                            else
                                            {
                                                sw.WriteLine("facet normal 0.0 0.0 -1.0");
                                                sw.WriteLine("outer loop");
                                                sw.Write("vertex ");
                                                sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.Write("vertex ");
                                                sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.Write("vertex ");
                                                sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.WriteLine("endloop");
                                                sw.WriteLine("endfacet");
                                                sw.WriteLine("facet normal 0.0 0.0 -1.0");
                                                sw.WriteLine("outer loop");
                                                sw.Write("vertex ");
                                                sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.Write("vertex ");
                                                sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.Write("vertex ");
                                                sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.WriteLine("endloop");
                                                sw.WriteLine("endfacet");
                                            }
                                            if (i < boundaries_X - 1)
                                            {
                                                /*GL.Normal3(1.0f, 0.0f, 0.0f);   // prawa ściana (w płaszczyźnie YZ)
                                                GL.Vertex4(0.5f, 0.5f, 0.5f, 1.0f);
                                                GL.Vertex4(0.5f, -0.5f, 0.5f, 1.0f);
                                                GL.Vertex4(0.5f, -0.5f, -0.5f, 1.0f);
                                                GL.Vertex4(0.5f, 0.5f, -0.5f, 1.0f);*/
                                                if (!cubeMatrix[i + 1, j, k]) //czy ma być prawa ściana
                                                {
                                                    sw.WriteLine("facet normal 1.0 0.0 0.0");
                                                    sw.WriteLine("outer loop");
                                                    sw.Write("vertex ");
                                                    sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.Write("vertex ");
                                                    sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.Write("vertex ");
                                                    sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.WriteLine("endloop");
                                                    sw.WriteLine("endfacet");
                                                    sw.WriteLine("facet normal 1.0 0.0 0.0");
                                                    sw.WriteLine("outer loop");
                                                    sw.Write("vertex ");
                                                    sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.Write("vertex ");
                                                    sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.Write("vertex ");
                                                    sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.WriteLine("endloop");
                                                    sw.WriteLine("endfacet");
                                                }
                                            }
                                            else
                                            {
                                                sw.WriteLine("facet normal 1.0 0.0 0.0");
                                                sw.WriteLine("outer loop");
                                                sw.Write("vertex ");
                                                sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.Write("vertex ");
                                                sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.Write("vertex ");
                                                sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.WriteLine("endloop");
                                                sw.WriteLine("endfacet");
                                                sw.WriteLine("facet normal 1.0 0.0 0.0");
                                                sw.WriteLine("outer loop");
                                                sw.Write("vertex ");
                                                sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.Write("vertex ");
                                                sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.Write("vertex ");
                                                sw.Write((i + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.WriteLine("endloop");
                                                sw.WriteLine("endfacet");
                                            }
                                            if (i > 0)
                                            {
                                                /*GL.Normal3(-1.0f, 0.0f, 0.0f);  // lewa ściana (w płaszczyźnie YZ)
                                                GL.Vertex4(-0.5f, 0.5f, 0.5f, 1.0f);
                                                GL.Vertex4(-0.5f, 0.5f, -0.5f, 1.0f);
                                                GL.Vertex4(-0.5f, -0.5f, -0.5f, 1.0f);
                                                GL.Vertex4(-0.5f, -0.5f, 0.5f, 1.0f);*/
                                                if (!cubeMatrix[i - 1, j, k]) //czy ma być prawa ściana
                                                {
                                                    sw.WriteLine("facet normal -1.0 0.0 0.0");
                                                    sw.WriteLine("outer loop");
                                                    sw.Write("vertex ");
                                                    sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.Write("vertex ");
                                                    sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.Write("vertex ");
                                                    sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.WriteLine("endloop");
                                                    sw.WriteLine("endfacet");
                                                    sw.WriteLine("facet normal -1.0 0.0 0.0");
                                                    sw.WriteLine("outer loop");
                                                    sw.Write("vertex ");
                                                    sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.Write("vertex ");
                                                    sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.Write("vertex ");
                                                    sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                    sw.Write(Environment.NewLine);
                                                    sw.WriteLine("endloop");
                                                    sw.WriteLine("endfacet");
                                                }
                                            }
                                            else
                                            {
                                                sw.WriteLine("facet normal -1.0 0.0 0.0");
                                                sw.WriteLine("outer loop");
                                                sw.Write("vertex ");
                                                sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.Write("vertex ");
                                                sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.Write("vertex ");
                                                sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.WriteLine("endloop");
                                                sw.WriteLine("endfacet");
                                                sw.WriteLine("facet normal -1.0 0.0 0.0");
                                                sw.WriteLine("outer loop");
                                                sw.Write("vertex ");
                                                sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k - 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.Write("vertex ");
                                                sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.Write("vertex ");
                                                sw.Write((i - 0.5 + " ").Replace(',', '.'));
                                                sw.Write((j + 0.5 + " ").Replace(',', '.'));
                                                sw.Write((k + 0.5 + " ").Replace(',', '.'));
                                                sw.Write(Environment.NewLine);
                                                sw.WriteLine("endloop");
                                                sw.WriteLine("endfacet");
                                            }
                                            //ogarnięcie wpisu trisów boxa
                                            /*facet normal ni nj nk
                                                outer loop
                                                    vertex v1x v1y v1z
                                                    vertex v2x v2y v2z
                                                    vertex v3x v3y v3z
                                                endloop
                                            endfacet
                                            */
                                        }
                                    }
                                }
                            }
                            sw.WriteLine("endsolid save1");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e) //zapisz scenę
        {
            Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    myStream.Close();
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName))
                        {
                            sw.WriteLine(Convert.ToString(boundaries_X));
                            for (int i = 0; i < boundaries_X; i++)
                            {
                                for (int j = 0; j < boundaries_Y; j++)
                                {
                                    for (int k = 0; k < boundaries_Z; k++)
                                    {
                                        if (cubeMatrix[i, j, k]) sw.Write("1 ");
                                        else sw.Write("0 ");
                                    }
                                    sw.Write(Environment.NewLine);
                                }
                            } //zapis pozycji wokseli
                            for (int i = 0; i < boundaries_X; i++)
                            {
                                for (int j = 0; j < boundaries_Y; j++)
                                {
                                    for (int k = 0; k < boundaries_Z; k++)
                                    {
                                        sw.Write(Convert.ToString((int)chosenColor[i, j, k].R));
                                        sw.Write(" ");
                                        sw.Write(Convert.ToString((int)chosenColor[i, j, k].G));
                                        sw.Write(" ");
                                        sw.Write(Convert.ToString((int)chosenColor[i, j, k].B));
                                        sw.Write(" ");
                                    }
                                    sw.Write(Environment.NewLine);
                                }
                            } //zapis kolorów wokseli
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e) //wczytaj scenę
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamReader sr = new StreamReader(openFileDialog1.FileName))
                    {
                        boundaries_X = Convert.ToSingle(sr.ReadLine());
                        boundaries_Y = boundaries_X;
                        boundaries_Z = boundaries_X;
                        for (int i = 0; i < boundaries_X; i++)
                        {
                            for (int j = 0; j < boundaries_Y; j++)
                            {
                                string line = sr.ReadLine();
                                string[] splited = line.Split(' ');
                                for (int k = 0; k < boundaries_Z; k++)
                                {
                                    if (splited[k] == "1") cubeMatrix[i, j, k] = true;
                                    else cubeMatrix[i, j, k] = false;
                                }
                            }
                        }
                        for (int i = 0; i < boundaries_X; i++)
                        {
                            for (int j = 0; j < boundaries_Y; j++)
                            {
                                string line = sr.ReadLine();
                                string[] splited = line.Split(' ');
                                for (int k = 0; k < boundaries_Z; k++)
                                {
                                    chosenColor[i, j, k] = Color.FromArgb(Convert.ToByte(splited[3 * k]), Convert.ToByte(splited[3 * k + 1]), Convert.ToByte(splited[3 * k + 2]));
                                }
                            }
                        }
                        selector = new Vector3(0.5f, 0.5f, 0.5f);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            glControl1.Invalidate();
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            CreateStlFile();
        } //guzik od zapisu do stl

        private void numericUpDown1_ValueChanged(object sender, EventArgs e) //pozycja światła X
        {
            light_X = (float)numericUpDown1.Value;
            glControl1.Invalidate();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e) //pozycja światła Y
        {
            light_Y = (float)numericUpDown2.Value;
            glControl1.Invalidate();
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e) //pozycja światła Z
        {
            light_Z = (float)numericUpDown3.Value;
            glControl1.Invalidate();
        }

        private void button9_Click(object sender, EventArgs e) //kolor światła
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                light_Color = colorDialog1.Color;
                textBox1.BackColor = light_Color;
            }
        }
    }
}
 