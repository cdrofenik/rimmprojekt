using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xen;
using Xen.Camera;
using Xen.Graphics;
using Xen.Ex.Graphics;
using Xen.Ex.Graphics2D;
using Xen.Ex.Geometry;
using Xen.Ex.Graphics.Content;
using Xen.Ex.Material;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

using JigLibX.Math;
using JigLibX.Physics;
using JigLibX.Geometry;
using JigLibX.Collision;

namespace rimmprojekt.States
{
    class Gallery : IGameState, IContentOwner
    {
        #region parameters
        private SpriteFont trueFont;
        private Int32 counter;
        private DrawTargetScreen drawToScreen;
        private IGameStateManager stateManager;
        private GraphicsDevice graphics;
        private Texture2D frameTexture;
        private TexturedElement frame;
        
        private TextElementRect txtElement;

        private List<String> fileNameList;
        private List<Texture2D> textureScreenShotov;
        private List<TexturedElement> seznamElementov;
        #endregion

        public Gallery(Application application)
        {
            Type appType = application.GetType().BaseType;
            graphics = (GraphicsDevice)appType.GetField("graphics", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(application);
            drawToScreen = new DrawTargetScreen(new Camera3D());
        }

        public void Initalise(IGameStateManager stateManager)
        {
            this.stateManager = stateManager;
            counter = 0;
            textureScreenShotov = new List<Texture2D>();
            seznamElementov = new List<TexturedElement>();
            fileNameList = new List<String>();

            stateManager.Application.Content.Add(this);
        }

        public void DrawScreen(DrawState state)
        {
            using (state.Shader.Push())
            {
                if (CullTest(state))
                {
                    
                    seznamElementov.ElementAt(counter).Draw(state);
                    frame.Draw(state);

                    txtElement.Text.SetText("\n\n" + (counter + 1) + "/" + textureScreenShotov.Count + "\nFile: " + fileNameList.ElementAt(counter));
                    txtElement.Draw(state);
                }
            }
        }

        void IContentOwner.LoadContent(ContentState state)
        {
            frameTexture = state.Load<Texture2D>(@"Textures/galleryFrame");
            trueFont = state.Load<SpriteFont>("Arial");

            string[] filePaths = Directory.GetFiles(@"Content\Screenshots", "*.png");
            foreach (string file in filePaths)
            {
                string path = file;
                FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.OpenOrCreate);
                textureScreenShotov.Add(Texture2D.FromStream(graphics, fs));
                fileNameList.Add(path);
            }
            setFrames();
        }

        public void Update(UpdateState state)
        {
            if (state.KeyboardState.KeyState.Escape.OnReleased)
            {
                stateManager.SetState(new MenuState());
                return;
            }

            if (state.KeyboardState.KeyState.Left.OnPressed)
            {
                counter--;
                if (counter < 0)
                    counter = textureScreenShotov.Count - 1;
            }

            if (state.KeyboardState.KeyState.Right.OnPressed)
            {
                counter = (counter + 1) % textureScreenShotov.Count;
            }
        }

        public bool CullTest(ICuller culler)
        {
            return true;
        }

        //custom methods
        private void setFrames()
        {

            Int32 elementCounter = 0;
            for (int i = 0; i < textureScreenShotov.Count; i++)
            {
                TexturedElement element = new TexturedElement(new Vector2(800, 450));
                element.Texture = textureScreenShotov.ElementAt(elementCounter);
                element.Position = new Vector2(225, 135);
                elementCounter++;
                seznamElementov.Add(element);
            }

            frame = new TexturedElement(frameTexture, new Vector2(1000, 600));
            //frame.Position = new Vector2(100, 600);
            frame.AlphaBlendState = Xen.Graphics.AlphaBlendState.Alpha;
            frame.VerticalAlignment = VerticalAlignment.Centre;
            frame.HorizontalAlignment = HorizontalAlignment.Centre;

            setTxtElement();
        }

        private void setTxtElement()
        {
            txtElement = new TextElementRect(new Vector2(300, 100));
            //txtElement.Position = new Vector2(100, 40);
            txtElement.Colour = Color.White;
            txtElement.VerticalAlignment = VerticalAlignment.Top;
            txtElement.HorizontalAlignment = HorizontalAlignment.Centre;
            txtElement.TextHorizontalAlignment = TextHorizontalAlignment.Centre;
            txtElement.Font = trueFont;
        }
    }
}
