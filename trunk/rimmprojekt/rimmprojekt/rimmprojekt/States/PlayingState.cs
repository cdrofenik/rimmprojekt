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
    class PlayingState : IGameState, IContentOwner
    {
        private IGameStateManager stateManager;
        private Razredi.Tezej tezej;
        private Razredi.Mapa mapa;

        public PlayingState(Application application)
        {
            
        }

        public void Initalise(IGameStateManager stateManager)
        {
            this.stateManager = stateManager;
            mapa = new Razredi.Mapa("../../../../rimmprojektContent/labirint1.txt", stateManager.Application.Content);
            tezej = new Razredi.Tezej(30.0f, 0.0f, 20.0f, stateManager.Application.UpdateManager, stateManager.Application.Content);
        }

        //simplified IDraw/IUpdate
        //NOTE:
        //For simplicity this only provides drawing directly to the screen.
        public void DrawScreen(DrawState state)
        {
            Vector3 target = new Vector3(tezej.polozaj.X, tezej.polozaj.Y - 35.0f, tezej.polozaj.Z - 35.0f);
            Vector3 position = new Vector3(tezej.polozaj.X, tezej.polozaj.Y + 35.0f, tezej.polozaj.Z + 35.0f);
            Camera3D camera = new Camera3D();
            camera.LookAt(target, position, Vector3.UnitY);
            state.Camera.SetCamera(camera);
            mapa.Draw(state);
            tezej.Draw(state);
        }

        public void Update(UpdateState state)
        {
            if (state.KeyboardState.KeyState.Escape.OnReleased)
                stateManager.SetState(new MenuState());
        }

        void IContentOwner.LoadContent(ContentState state)
        {
        }
    }
}
