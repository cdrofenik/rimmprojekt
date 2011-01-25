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
    class GameStateManager : IGameStateManager, IDraw, IUpdate
    {
        private readonly Application application;
        private PlayerIndex playerIndex;

        //the current state object
        private IGameState currentGameState;

        public GameStateManager(Application application)
        {
            this.playerIndex = PlayerIndex.One;
            this.application = application;

            this.SetState(new MenuState());
        }

        //interface members
        Application IGameStateManager.Application
        {
            get { return application; }
        }
        PlayerIndex IGameStateManager.PlayerIndex
        {
            get { return playerIndex; }
            set { playerIndex = value; }
        }

        //change to a new state.
        public void SetState(IGameState state)
        {
            //dispose the old state first (otherwise it's resources might stick around!)
            if (this.currentGameState is IDisposable)
                (this.currentGameState as IDisposable).Dispose();

            this.currentGameState = state;

            //call Initalise() on the new state
            state.Initalise(this);
        }

        public void Draw(DrawState state)
        {
            //draw the state
            this.currentGameState.DrawScreen(state);
        }

        bool ICullable.CullTest(ICuller culler)
        {
            return true;
        }

        public UpdateFrequency Update(UpdateState state)
        {
            //update the current state
            this.currentGameState.Update(state);

            return UpdateFrequency.FullUpdate60hz;
        }
    }
}
