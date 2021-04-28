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
using System.Runtime.InteropServices;

namespace taoTempl
{

    public partial class Form1 : Form
    {
        int[] texture = new int[2];
        Glu.GLUquadric quadr;
        double angle = 0;
        float dx=0, camera_x=0, camera_temp=0;
        float speed = 2f;
        int player_way = 2;
        Random rand = new Random();
        float bullet_speed = 4f;
        float time_spawning = 10, time_temp=0;
        int score = 0, lives = 3;
        int moves = 0;
        public struct obj
        {
            public float trans_y, trans_x;
            public int way;
        };

        List<obj> enemy, bullet;

        private void SimpleOpenGlControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (Math.Abs(camera_x+22) <5|| Math.Abs(camera_x - 0) < 5 || Math.Abs(camera_x - 22) < 5)
                {
                    obj bul = new obj();
                    bul.trans_y = 0;
                    bul.trans_x = camera_x;
                    bul.way = player_way;
                    bullet.Add(bul);
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
            simpleOpenGlControl1.InitializeContexts();
            enemy = new List<obj>();
            bullet = new List<obj>();
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
            label1.Text = "Score: " + score;
            label2.Text = "Lives: " + lives;

            if (lives <= 0)
            {
                timer1.Enabled = false;
                MessageBox.Show("You lose!");
                Application.Exit();

            }
            else
            {
                angle = angle + 0.2;
                Gl.glViewport(0, 0, simpleOpenGlControl1.Width, simpleOpenGlControl1.Height);

                Gl.glMatrixMode(Gl.GL_PROJECTION);
                Gl.glLoadIdentity();

                Glu.gluPerspective(40, (float)simpleOpenGlControl1.Width / (float)simpleOpenGlControl1.Height, 0.1, 5000);

                Gl.glMatrixMode(Gl.GL_MODELVIEW);

                Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

                Gl.glClearColor(0, 0, 0, 0);

                Gl.glLoadIdentity();

                Gl.glEnable(Gl.GL_TEXTURE_2D);
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture[0]);

                Gl.glTranslated(0, -5, -70);
                Gl.glRotated(-75, 1, 0, 0);

                Gl.glDisable(Gl.GL_LIGHT0);

                //поверхность
                Gl.glPushMatrix();
                Gl.glRotated(-90, 0, 1, 0);
                Gl.glTranslated(-10002, 350, -40);
                Gl.glRotated(-angle, 0, 0, 1);

                Glu.gluCylinder(quadr, 10000, 10000, 80, 900000000 * 2, 3);

                Gl.glPopMatrix();

                Gl.glPushMatrix();
                Gl.glRotated(-90, 0, 1, 0);
                Gl.glTranslated(-100002, 1700, -44);
                Gl.glRotated(-angle, 0, 0, 1);

                Glu.gluCylinder(quadr, 100000, 100000, 88, 900000000 * 2, 3);

                Gl.glPopMatrix();

                Gl.glDisable(Gl.GL_TEXTURE_2D);

                Gl.glEnable(Gl.GL_LIGHT0);
                float[] light0_AMBIENT = { 0, 0, 1, 1 };
                Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_AMBIENT, light0_AMBIENT);

                Gl.glLightf(Gl.GL_LIGHT0, Gl.GL_SPECULAR, 250);

                //боковые стены
                float[] wall = { 1, 0, 1, 1 };
                float[] Wall_emission = { 0.3f, 0, 1, 0 };

                Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_EMISSION, Wall_emission);
                Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, wall);
                Gl.glMaterialf(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, 250);
                Gl.glDisable(Gl.GL_TEXTURE_2D);

                Gl.glTranslated(-33, -30, 0);
                Gl.glRotated(-90, 1, 0, 0);
                Glu.gluCylinder(quadr, 3, 3, 5000, 50, 50);

                Gl.glTranslated(66, 0, 0);
                Glu.gluCylinder(quadr, 3, 3, 5000, 50, 50);

                Gl.glTranslated(-33, 0, 20);

                //генерация врагов
                time_temp += 0.3f;
                if (enemy.Count < 20 && time_temp >= time_spawning)
                {
                    time_temp = 0;
                    obj enm = new obj();
                    enm.way = rand.Next(1, 4);
                    if (enm.way == 1)
                        enm.trans_x = -22;
                    if (enm.way == 2)
                        enm.trans_x = 0;
                    if (enm.way == 3)
                        enm.trans_x = 22;
                    enm.trans_y = 1500;
                    enemy.Add(enm);
                }

                //враги
                float[] ae = { 0, 1, 0, 1 };
                float[] e_emission = { 0, 1, 0, 1 };

                Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_EMISSION, e_emission);
                Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, ae);
                Gl.glMaterialf(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, 250);
                Gl.glDisable(Gl.GL_TEXTURE_2D);
                for (int i = 0; i < enemy.Count; i++)
                {
                    Gl.glPushMatrix();
                    obj enm = new obj();
                    enm.way = enemy[i].way;
                    enm.trans_y = enemy[i].trans_y - 3;
                    enm.trans_x = enemy[i].trans_x;
                    enemy[i] = enm;
                    if (enemy[i].trans_y < -20)
                    {
                        enemy.RemoveAt(i);
                        --lives;
                    }
                    else
                    {
                        Gl.glTranslated(enemy[i].trans_x, 0, enemy[i].trans_y);
                        Glu.gluSphere(quadr, 2, 50, 50);
                    }
                    Gl.glPopMatrix();
                }
                Gl.glEnable(Gl.GL_TEXTURE_2D);

                if (Math.Abs(camera_temp - camera_x ) < 22)
                    camera_x += dx;
                else
                {
                    camera_temp = camera_x;
                    dx = 0;
                    moves = 0;
                }
                //пули
                float[] ab = { 0, 0, 1, 1 };
                float[] b_emission = { 0, 0, 1, 1 };

                Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_EMISSION, b_emission);
                Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, ab);
                Gl.glMaterialf(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, 250);
                Gl.glDisable(Gl.GL_TEXTURE_2D);

                for (int i = 0; i < bullet.Count; i++)
                {
                    obj bul = new obj();
                    bul.trans_y = bullet[i].trans_y + bullet_speed;
                    bul.way = bullet[i].way;
                    bul.trans_x = bullet[i].trans_x;
                    bullet[i] = bul;
                    if (bullet[i].trans_y < 800)
                    {
                        Gl.glPushMatrix();
                        Gl.glTranslated(bullet[i].trans_x, 0, bullet[i].trans_y);
                        Glu.gluCylinder(quadr, 2, 0.4, 7, 10, 10);
                        Gl.glPopMatrix();
                    }
                    else
                        bullet.RemoveAt(i);
                }

                for (int i = 0; i < enemy.Count; i++)
                {
                    for (int j = 0; j < bullet.Count; j++)
                    {
                        if (enemy[i].way == bullet[j].way)
                            if (Math.Abs(enemy[i].trans_y - bullet[j].trans_y) < 5)
                            {
                                enemy.RemoveAt(i);
                                bullet.RemoveAt(j);
                                score += 10;
                            }
                    }
                }

                Gl.glTranslated(camera_x, 0, 0);

                //звездолет
                float[] a = { 0.8f, 1, 0, 1 };
                float[] Plane_emission = { 0.3f, 0, 0, 0 };

                Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_EMISSION, Plane_emission);
                Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, a);
                Gl.glMaterialf(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, 250);
                Gl.glDisable(Gl.GL_TEXTURE_2D);



                Glu.gluCylinder(quadr, 2, 0, 5, 50, 50);
                Gl.glTranslated(0, 0, -5);
                Glu.gluCylinder(quadr, 2, 2, 5, 50, 50);
                Gl.glTranslated(0, 0, -3);
                Glu.gluCylinder(quadr, 1, 2, 3, 50, 50);

                Gl.glTranslated(-1.8, 0, 5);
                Gl.glRotated(-90, 0, 1, 0);
                Glu.gluCylinder(quadr, 1, 0.5, 5, 50, 50);
                Gl.glRotated(-180, 0, 1, 0);
                Gl.glTranslated(0, 0, 3.6);
                Glu.gluCylinder(quadr, 1, 0.5, 5, 50, 50);

                Gl.glEnable(Gl.GL_LIGHT1);
                float[] light1_AMBIENT = { 0, 0, 0, 1 };
                float[] light1_location = { 0, 0, 0, 0 };
                Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_AMBIENT, light1_AMBIENT);
                Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_DIFFUSE, light1_AMBIENT);
                Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_POSITION, light1_location);
                Gl.glLightf(Gl.GL_LIGHT1, Gl.GL_SPECULAR, 250);

                Gl.glBegin(Gl.GL_QUADS);

                Gl.glEnd();

                simpleOpenGlControl1.Invalidate();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Gl.glEnable(Gl.GL_DEPTH_TEST);

            Gl.glEnable(Gl.GL_LIGHTING);

            Gl.glEnable(Gl.GL_ALPHA_TEST);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

            quadr = Glu.gluNewQuadric();
            Glu.gluQuadricTexture(quadr, Gl.GL_TRUE);
           LoadTexture(@"image.gif", 0);
        }


        private void SimpleOpenGlControl1_KeyUp(object sender, KeyEventArgs e)
        {
            if (moves == 0)
            {
                if (e.KeyCode == Keys.A)
                {

                    if (player_way > 1)
                    {
                        ++moves;
                        dx = -speed;
                        --player_way;
                    }
                }

                if (e.KeyCode == Keys.D)
                {

                    if (player_way < 3)
                    {
                        ++moves;
                        dx = speed;
                        ++player_way;
                    }
                }
            }

            if (e.KeyCode == Keys.Space)
            {
                timer1.Enabled = !timer1.Enabled;
            }

            if (e.KeyCode == Keys.Escape)
            {
                Application.Exit();
            }
        }

    }
}
