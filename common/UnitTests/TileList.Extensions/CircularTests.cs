using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PA.TileList.Circular;
using System.Drawing;

namespace UnitTests.TileList.Extensions
{
    [TestClass]
    public class CircularTests
    {
        [TestMethod]
        public void Profile()
        {
            CircularProfile p = new CircularProfile(100);

            double step = 1f;
            double reso = 1f;

            p.AddProfileFlat(-Math.PI/2, 10, 10, step);
            p.AddProfileFlat(7 * Math.PI / 4, 10, 10, step);
            p.AddProfileFlat(0, 10, 10, step);
            p.AddProfileFlat(Math.PI / 3f, 20, 20, step, reso);
            p.AddProfileFlat(2f * Math.PI / 3f, 40, 40, step, reso);
           
            //p.AddProfileFlat(Math.PI, 50, 50, step, reso);


            ProfileToBmp("Profile", p, 50);
        }

        private void ProfileToBmp(string testname, CircularProfile p, int ppp = 1)
        {
            int maxsize = ppp * (int)p.GetMaxRadius() * 2;
            int minsize = ppp * (int)p.GetMinRadius() * 2;
            int midsize = ppp * (int)p.Radius * 2;
            int mid = maxsize / 2 + 3 * ppp;

            using (Bitmap b = new Bitmap(2 * mid, 2 * mid))
            {
                using (Graphics g = Graphics.FromImage(b))
                {
                    g.DrawEllipse(Pens.Blue, mid - maxsize / 2, mid - maxsize / 2, maxsize, maxsize);
                    g.DrawEllipse(Pens.Blue, mid - minsize / 2, mid - minsize / 2, minsize, minsize);
                    g.DrawEllipse(Pens.Black, mid - midsize / 2, mid - midsize / 2, midsize, midsize);

                    for (int f = 0; f < 8; f++)
                    {
                        double angle = f * Math.PI / 4f;
                        g.DrawLine(new Pen(Color.Yellow, 1), mid, mid, mid + (int)Math.Round(ppp * (p.Radius + 10) * Math.Cos(angle)), mid - (int)Math.Round(ppp * (p.Radius + 10) * Math.Sin(angle)));
                    }

                    Color[] c = new Color[] { Color.Transparent, Color.Transparent };

                    CircularProfile.ProfileStep s = p.GetFirst();

                    double a = s.Angle  ;
                    double r = s.Radius;

                    int[] pos = Plot(g, ppp, mid, c[0], a, r, "X");

                    int i = 1;

                    foreach (CircularProfile.ProfileStep ps in p.Profile)
                    {
                        int[] end = Plot(g, ppp, mid, Color.Transparent, ps.Angle, r, i % 10 == 0 ? i.ToString() : "");
                        int[] cur = Plot(g, ppp, mid, c[i % c.Length], ps.Angle, ps.Radius, i % 10 == 0 ? i.ToString() : "");

                        int ad = (int)Math.Round(180 * -a / Math.PI);
                        int sw = (int)Math.Round(180 * -(ps.Angle - a) / Math.PI);

                        g.DrawArc(new Pen(Color.Red, 3), (int)Math.Round(mid - r * ppp), (int)Math.Round(mid - r * ppp), (int)Math.Round(r * 2 * ppp), (int)Math.Round(r * 2 * ppp), ad, sw);
                        g.DrawLine(new Pen(Color.Red, 3), end[0], end[1], cur[0], cur[1]);


                        pos = cur;
                        a = ps.Angle;
                        r = ps.Radius;
                        i++;
                    }
                }

                b.Save(testname + ".png");
            }
        }

        private int[] Plot(Graphics g, int ppp, int mid, Color c, double a, double r, string s)
        {
            int plotsize = 3;

            int x0 = (int)Math.Round(mid + r * Math.Cos(a) * ppp);
            int y0 = (int)Math.Round(mid - r * Math.Sin(a) * ppp);

            g.FillRectangle(new SolidBrush(c), x0 - plotsize * ppp / 2, y0 - plotsize * ppp / 2, plotsize * ppp, plotsize * ppp);

            if (s != "")
            {
                int x1 = (int)Math.Round(mid + (r + plotsize + s.Length) * Math.Cos(a) * ppp);
                int y1 = (int)Math.Round(mid - (r + plotsize + s.Length) * Math.Sin(a) * ppp);
                g.DrawString(s, SystemFonts.DefaultFont, Brushes.Black, x1, y1);
            }
            return new int[] { x0, y0 };
        }
    }

}
