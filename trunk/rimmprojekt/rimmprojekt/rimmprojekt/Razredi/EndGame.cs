using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
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
        private SolidColourElement back = new SolidColourElement(Color.Black,new Vector2(1280, 720));
        private Int32 counter;

        public EndGame(UpdateManager manager, ContentRegister content) 
        {
            manager.Add(this);
            content.Add(this);
        }

        public void Draw(DrawState state)
        {
            back.Draw(state);
            background.Draw(state);
            textBox.Draw(state);
        }

        public void LoadContent(ContentState state)
        {
            backPic = state.Load<Texture2D>(@"the_end");
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
            background = new TexturedElement(new Vector2(400, 299));
            background.AlphaBlendState = AlphaBlendState.Alpha;
            background.HorizontalAlignment = HorizontalAlignment.Left;
            background.VerticalAlignment = VerticalAlignment.Centre;
            background.Texture = backPic;

            textBox = new TextElementRect(new Vector2(500, 400));
            textBox.Font = bigFont;
            textBox.Colour = Color.White;
            textBox.AlphaBlendState = AlphaBlendState.Additive;
            textBox.HorizontalAlignment = HorizontalAlignment.Right;
            textBox.VerticalAlignment = VerticalAlignment.Centre;
            textBox.TextHorizontalAlignment = TextHorizontalAlignment.Left;
            textBox.Position = new Vector2(-150, 100);

            String pot = "../../../../rimmprojektContent/credits.txt";
            using (StreamReader sr = new StreamReader(pot))
            {
                textBox.Text.AppendLine(sr.ReadToEnd());
            }
            
        }
    }
}
