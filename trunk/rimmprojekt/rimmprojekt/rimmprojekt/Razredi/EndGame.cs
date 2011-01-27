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
using Xen.Ex.Graphics;
using Xen.Ex.Graphics2D;
using Xen.Ex.Geometry;
using Xen.Ex.Graphics.Content;
using Xen.Ex.Material;

namespace rimmprojekt.Razredi
{
    class EndGame : IDraw, IContentOwner, IUpdate
    {
        private SpriteFont bigFont;
        private TextElementRect textBox;
        private Texture2D backPic;
        private TexturedElement background;
        private Int32 counter;

        public EndGame(UpdateManager manager, ContentRegister content) 
        {
            manager.Add(this);
            content.Add(this);
        }

        public void Draw(DrawState state)
        {
            //background.Draw(state);
            textBox.Draw(state);
        }

        public void LoadContent(ContentState state)
        {
            //backPic = state.Load<Texture2D>(@"backgroundEnd");
            bigFont = state.Load<SpriteFont>(@"ArialBattle");
            setVisuals();
        }

        public bool CullTest(ICuller culler)
        {
            return true;
        }

        public UpdateFrequency Update(UpdateState state)
        {
            return UpdateFrequency.FullUpdate60hz;
        }

        private void setVisuals()
        {
            background = new TexturedElement(new Vector2(1280, 720));
            //background.Texture = backPic;

            textBox = new TextElementRect(new Vector2(400, 400));
            textBox.Font = bigFont;
            textBox.Colour = Color.White;
            textBox.AlphaBlendState = AlphaBlendState.Additive;
            textBox.HorizontalAlignment = HorizontalAlignment.Right;
            textBox.VerticalAlignment = VerticalAlignment.Centre;
            textBox.TextHorizontalAlignment = TextHorizontalAlignment.Right;
            textBox.Position = new Vector2(-350, 100);

            String pot = "../../../../rimmprojektContent/credits.txt";
            using (StreamReader sr = new StreamReader(pot))
            {
                textBox.Text.AppendLine(sr.ReadLine());
            }
            
        }
    }
}
