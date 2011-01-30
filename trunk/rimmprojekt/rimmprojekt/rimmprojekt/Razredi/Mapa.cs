using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;

using Xen;
using Xen.Camera;
using Xen.Graphics;
using Xen.Ex.Geometry;
using Xen.Ex.Material;

using JigLibX.Math;
using JigLibX.Physics;
using JigLibX.Geometry;
using JigLibX.Collision;

namespace rimmprojekt.Razredi
{
    public class Mapa : IDraw
    {
        public ArrayList zidovi;
        private Tla tla;

        public Mapa(string pot, ContentRegister content)
        {
            using (StreamReader sr = new StreamReader(pot))
            {
                Point velikost = new Point();
                velikost.X = Int32.Parse(sr.ReadLine());
                velikost.Y = Int32.Parse(sr.ReadLine());
                tla = new Tla(velikost, content);

                zidovi = new ArrayList();
                for (int i = 0; i < velikost.X; i++)
                {
                    for (int j = 0; j < velikost.Y; j++)
                    {
                        if (sr.Read() - 48 == 0)
                            zidovi.Add(new Kocka((float)j * 20.0f, 0.0f, (float)i * 20.0f, content));
                    }
                    sr.Read();
                    sr.Read();
                }
            }

            //foreach (Kocka k in zidovi)
            //    tla.skin.NonCollidables.Add(k.skin);
        }

        public void Draw(DrawState state)
        {
            tla.Draw(state);

            foreach (Kocka k in zidovi)
                k.Draw(state);
        }

        public bool CullTest(ICuller culler)
        {
            return true;
        }
    }
}
