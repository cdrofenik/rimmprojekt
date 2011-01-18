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
    class GalleryState : IContentOwner, IGameState
    {
        private List<Texture2D> textureScreenShotov;
        private List<TexturedElement> seznamElementov;
        private Int32 counter;

        private DrawTargetScreen drawToScreen;
        private IGameStateManager stateManager;

        public GalleryState(Application application)
        {
            drawToScreen = new DrawTargetScreen(new Camera3D());
        }

        public void Initalise(IGameStateManager stateManager)
        {
            this.stateManager = stateManager;
            counter = 0;
            textureScreenShotov = new List<Texture2D>();
            seznamElementov = new List<TexturedElement>();
        }

        public void DrawScreen(DrawState state)
        {
            using (state.Shader.Push())
            {
                if (CullTest(state))
                {
                    if (seznamElementov.Count != 0)
                    {
                        seznamElementov.ElementAt(counter).Draw(state);
                    }
                }
            }
        }

        void IContentOwner.LoadContent(ContentState state)
        {
            string[] filePaths = Directory.GetFiles("C://Screenshots", "*.png");
            foreach (string file in filePaths)
            {
                string path = "\"" + file + "\"";
                textureScreenShotov.Add(state.Load<Texture2D>(@path));
            }
            setFrames();
            int steviloEl = seznamElementov.Count;
        }

        public void Update(UpdateState state)
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
