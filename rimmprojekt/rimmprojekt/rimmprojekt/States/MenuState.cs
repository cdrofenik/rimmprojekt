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
        private TextElementRect menuText;
        private IGameStateManager stateManager;


        public void Initalise(IGameStateManager stateManager)
        {
            this.stateManager = stateManager;

            //display an incredibly complex line of text
            this.menuText = new TextElementRect(new Vector2(400, 100));
            this.menuText.Text.SetText("Pritisni [ENTER]");
            this.menuText.VerticalAlignment = VerticalAlignment.Centre;
            this.menuText.HorizontalAlignment = HorizontalAlignment.Centre;
            this.menuText.TextHorizontalAlignment = TextHorizontalAlignment.Centre;

            //set the text font (using global content)
            stateManager.Application.Content.Add(this);
        }

        public void DrawScreen(DrawState state)
        {
            //display the 'menu' :-)
            menuText.Draw(state);
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
            this.menuText.Font = state.Load<SpriteFont>("Arial");
        }
    }
}
