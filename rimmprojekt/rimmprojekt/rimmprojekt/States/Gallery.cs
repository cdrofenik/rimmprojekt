using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

using Xen;
using Xen.Camera;
using Xen.Graphics;
using Xen.Ex.Graphics;
using Xen.Ex.Graphics2D;
using Xen.Ex.Geometry;
using Xen.Ex.Graphics.Content;
using Xen.Ex.Material;

using Microsoft.Xna.Framework.Content;

namespace rimmprojekt.States
{
    class Gallery : IDraw, IContentOwner, IUpdate
    {
        private List<Texture2D> textureScreenShotov;
        private List<TexturedElement> seznamElementov;
        private Int32 counter;

        private IShader shader;

        public Gallery(UpdateManager manager, ContentRegister content)
        {
            manager.Add(this);
            content.Add(this);

            counter = 0;
            textureScreenShotov = new List<Texture2D>();
            seznamElementov = new List<TexturedElement>();
        }

        public void Draw(DrawState state) {

            using (state.Shader.Push())
            {
                if (CullTest(state))
                {
                    seznamElementov.ElementAt(counter).Draw(state);
                }
            }
        }

        public void LoadContent(ContentState state) 
        {
            string[] filePaths = Directory.GetFiles(@"Screenshots", "*.jpg");
            foreach (string file in filePaths)
            {
                string path = "\""+file+"\"";
                textureScreenShotov.Add(state.Load<Texture2D>(@path));
            }
        }

        public UpdateFrequency Update(UpdateState state) 
        {
            if (state.KeyboardState.KeyState.Left)
            {
                if (counter != 0)
                {
                    counter--;
                }
            }

            if (state.KeyboardState.KeyState.Right)
            {
                counter++;
            }

            return UpdateFrequency.FullUpdate60hz; 
        }

        private void setFrames()
        {
            Int32 elementCounter = 0;
            foreach(TexturedElement element in seznamElementov)
            {
                element.Texture =  textureScreenShotov.ElementAt(elementCounter);
                element.Position = new Vector2(100, 200);
                elementCounter++;
            }
        }

        public bool CullTest(ICuller culler)
        {
            return true;
        }
    }
}
