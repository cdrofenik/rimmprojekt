using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Xen;
using Xen.Camera;
using Xen.Graphics;
using Xen.Ex.Geometry;
using Xen.Ex.Material;

namespace rimmprojekt.Razredi
{
    class Oprema
    {
        protected String imeOpreme;
        protected String tipOpreme;
        protected String damageOpreme;
        protected String armorOpreme;
        protected Vector2 velikostOpreme;
        protected Texture2D slikaOpreme;

        public Oprema(String ime, String tip, String damage, String armor, String imgReference)
        {
            this.imeOpreme = ime;
            this.tipOpreme = tip;
            this.damageOpreme = damage;
            this.armorOpreme = armor;
        }
    }
}
