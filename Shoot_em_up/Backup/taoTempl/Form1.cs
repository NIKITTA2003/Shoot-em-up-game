using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tao.OpenGl;
using System.Drawing.Imaging;

namespace taoTempl
{

    public partial class Form1 : Form
    {
        int[] texture = new int[1];
        Glu.GLUquadric quadr;
        public Form1()
        {
            InitializeComponent();
            simpleOpenGlControl1.InitializeContexts();
        }

        private void LoadTexture(string fname, int elementIndex)
        {
            Bitmap textureImage = new Bitmap(fname);
            if (textureImage != null)
            {
                Gl.glGenTextures(1, out texture[elementIndex]);
                textureImage.RotateFlip(RotateFlipType.Rotate180FlipX);
                Rectangle rectangle = new Rectangle(0, 0, textureImage.Width, textureImage.Height);
                BitmapData bitmapData = textureImage.LockBits(rectangle, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture[elementIndex]);
                Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB, textureImage.Width, textureImage.Height, 0, Gl.GL_BGR_EXT, Gl.GL_UNSIGNED_BYTE, bitmapData.Scan0);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
                textureImage.UnlockBits(bitmapData);
                textureImage.Dispose();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Gl.glViewport(0, 0, simpleOpenGlControl1.Width, simpleOpenGlControl1.Height);

            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();

            Glu.gluPerspective(5, (float)simpleOpenGlControl1.Width / (float)simpleOpenGlControl1.Height, 0.1, 70);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);


            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            Gl.glClearColor(0, 0, 0, 0);

            Gl.glLoadIdentity();

            Gl.glTranslated(0,0,-8);
            Gl.glRotated(-60,1,0,0);

            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture[0]);
            Glu.gluSphere(quadr, 0.1, 50, 50);

            Gl.glDisable(Gl.GL_TEXTURE_2D);

            simpleOpenGlControl1.Invalidate();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Gl.glEnable(Gl.GL_DEPTH_TEST);

            Gl.glEnable(Gl.GL_LIGHTING);

            Gl.glEnable(Gl.GL_LIGHT0);

            LoadTexture(@"1m.bmp", 0);

            quadr = Glu.gluNewQuadric();

            Glu.gluQuadricTexture(quadr, Gl.GL_TRUE);
        }
    }
}
