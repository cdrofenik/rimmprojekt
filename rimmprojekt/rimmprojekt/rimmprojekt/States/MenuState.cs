using System;
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

namespace rimmprojekt.States
{
    class MenuState : IGameState, IContentOwner
    {
        private List<TextElementRect> menuText = new List<TextElementRect>();
        private IGameStateManager stateManager;

        #region loading and background pic.
        private Boolean isLoadingElement;
        private float PlayTime;
        private float ActionTime;
        private Texture2D loadingTex;
        private TexturedElement loadingElement;
        private Texture2D alistarLol;
        private SolidColourElement solidColElement;
        private TexturedElement background;
        #endregion

        public void Initalise(IGameStateManager stateManager)
        {
            this.stateManager = stateManager;

            //display an incredibly complex line of text
            this.menuText.Add(new TextElementRect(new Vector2(400, 250)));
            this.menuText[0].Text.SetText("Nova igra [ENTER]");
            this.menuText[0].VerticalAlignment = VerticalAlignment.Bottom;
            this.menuText[0].HorizontalAlignment = HorizontalAlignment.Left;
            this.menuText[0].TextHorizontalAlignment = TextHorizontalAlignment.Left;

            this.menuText.Add(new TextElementRect(new Vector2(400, 100)));
            this.menuText[1].Text.SetText("Izhod [ESC]");
            this.menuText[1].VerticalAlignment = VerticalAlignment.Bottom;
            this.menuText[1].HorizontalAlignment = HorizontalAlignment.Left;
            this.menuText[1].TextHorizontalAlignment = TextHorizontalAlignment.Left;

            this.menuText.Add(new TextElementRect(new Vector2(400, 170)));
            this.menuText[2].Text.SetText("Galerija [Gs]");
            this.menuText[2].VerticalAlignment = VerticalAlignment.Bottom;
            this.menuText[2].HorizontalAlignment = HorizontalAlignment.Left;
            this.menuText[2].TextHorizontalAlignment = TextHorizontalAlignment.Left;

            #region background and loading
            loadingElement = new TexturedElement(new Vector2(718, 69));
            loadingElement.VerticalAlignment = VerticalAlignment.Bottom;
            loadingElement.HorizontalAlignment = HorizontalAlignment.Centre;
            background = new TexturedElement(new Vector2(1280, 720));
            solidColElement = new SolidColourElement(Color.Yellow, new Vector2(2, 2));
            isLoadingElement = false;
            PlayTime = 0.0f;
            ActionTime = 0.0f;
            #endregion

            //set the text font (using global content)
            stateManager.Application.Content.Add(this);
        }

        public void DrawScreen(DrawState state)
        {
            background.Draw(state);
            //display the 'menu' :-)
            if (isLoadingElement)
            {
                loadingElement.Draw(state);
                solidColElement.Draw(state);
            }
            else
            {
                menuText[0].Draw(state);
                menuText[1].Draw(state);
                menuText[2].Draw(state);
            }
        }

        public void Update(UpdateState state)
        {
            PlayTime += state.DeltaTimeSeconds;

            #region loading
            if (isLoadingElement)
            {
                ActionTime++;
                drawLoadingBar();
                if (Math.Round(ActionTime) == 339)
                {
                    PlayingState gameState = new PlayingState(this.stateManager.Application);
                    this.stateManager.SetState(gameState);
                }
            }
            #endregion

            Xen.Input.State.InputState input = state.PlayerInput[this.stateManager.PlayerIndex].InputState;

            if (state.KeyboardState.KeyState.Enter.OnReleased)
            {
                isLoadingElement = true;
                return;
            }

            if (state.KeyboardState.KeyState.G.OnReleased)
            {
                //start gallery display
                Gallery galleryState = new Gallery(this.stateManager.Application);

                //go to the loading state.
                this.stateManager.SetState(galleryState);
                return;
            }

            if (input.Buttons.Back.OnPressed)
            {
                //quit when back is pressed in the menu
                this.stateManager.Application.Shutdown();
                return;
            }
        }

        void IContentOwner.LoadContent(ContentState state)
        {
            //load the text font.
            this.menuText[0].Font = state.Load<SpriteFont>("MenuFont");
            this.menuText[1].Font = state.Load<SpriteFont>("MenuFont");
            this.menuText[2].Font = state.Load<SpriteFont>("MenuFont");
            alistarLol = state.Load<Texture2D>("Textures/Lol_Alistar");
            loadingTex = state.Load<Texture2D>("Textures/Loading");
            background.Texture = alistarLol;
            loadingElement.Texture = loadingTex;
        }

        private void drawLoadingBar()
        {
            int timeSize = Int32.Parse(Math.Round((ActionTime/10 * 100) / 5).ToString());
            solidColElement= new SolidColourElement(Color.Yellow,new Vector2(timeSize, 10));
            solidColElement.Position = new Vector2(296, 42);
        }
    }
}
