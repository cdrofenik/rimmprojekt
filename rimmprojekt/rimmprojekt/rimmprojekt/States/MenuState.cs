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
        private bool IsActive;

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

            //set the text font (using global content)
            stateManager.Application.Content.Add(this);
        }

        bool IGameState.isActive
        {
            get { return IsActive; }
            set { IsActive = value; }
        }

        public void DrawScreen(DrawState state)
        {
            //display the 'menu' :-)
            menuText[0].Draw(state);
            menuText[1].Draw(state);
            menuText[2].Draw(state);
        }

        public void Update(UpdateState state)
        {
            //when a button is pressed, load the game..
            //Note the player index selected in the startup screen is used here...
            Xen.Input.State.InputState input = state.PlayerInput[this.stateManager.PlayerIndex].InputState;

            if (state.KeyboardState.KeyState.Enter.OnReleased)
            {
                //we want to start playing the game!

                //create a new game to play.
                PlayingState gameState = new PlayingState(this.stateManager.Application);

                //go to the loading state.
                this.stateManager.SetState(gameState);
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
        }
    }
}
