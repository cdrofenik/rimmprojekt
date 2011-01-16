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
using Microsoft.Xna.Framework.Media;

using JigLibX.Math;
using JigLibX.Physics;
using JigLibX.Geometry;
using JigLibX.Collision;

namespace rimmprojekt.States
{
    class PlayingState : IGameState, IContentOwner
    {
        Song backgroundSong;
        Boolean backgroundSongStart = false;
        private TextElementRect debugText;

        private DrawTargetScreen drawToScreen;
        private IGameStateManager stateManager;
        private Razredi.Tezej tezej;
        private Razredi.Mapa mapa;
        private Razredi.Inventory inventory;
        private Razredi.Minotaver minotaver;

        public PlayingState(Application application)
        {
            drawToScreen = new DrawTargetScreen(new Camera3D());
        }

        public void Initalise(IGameStateManager stateManager)
        {
            this.stateManager = stateManager;

            mapa = new Razredi.Mapa("../../../../rimmprojektContent/labirint1.txt", stateManager.Application.Content, stateManager.Application.UpdateManager);

            List<Body> bodies = new List<Body>();
            foreach (Razredi.Kocka k in mapa.zidovi)
                bodies.Add(k.body);
            tezej = new Razredi.Tezej(30.0f, 0.0f, 20.0f, stateManager.Application.UpdateManager, stateManager.Application.Content, bodies);
            inventory = new Razredi.Inventory(stateManager.Application.UpdateManager, stateManager.Application.Content);
            minotaver = new Razredi.Minotaver(35.0f, 0.0f, 20.0f, stateManager.Application.UpdateManager, stateManager.Application.Content, bodies);

            this.debugText = new TextElementRect(new Vector2(400, 100));
            this.debugText.Position = new Vector2(340, 240);
            this.debugText.Colour = Color.Pink;
            this.debugText.Text.SetText("Debug Text");
            this.debugText.VerticalAlignment = VerticalAlignment.Centre;
            this.debugText.HorizontalAlignment = HorizontalAlignment.Centre;
            this.debugText.TextHorizontalAlignment = TextHorizontalAlignment.Centre;

            stateManager.Application.Content.Add(this);
        }

        private void InitialisePhysics()
        {

        }

        //simplified IDraw/IUpdate
        //NOTE:
        //For simplicity this only provides drawing directly to the screen.
        public void DrawScreen(DrawState state)
        {
            Vector3 target = new Vector3(tezej.polozaj.X, tezej.polozaj.Y - 35.0f, tezej.polozaj.Z - 35.0f);
            Vector3 position = new Vector3(tezej.polozaj.X, tezej.polozaj.Y + 35.0f, tezej.polozaj.Z + 35.0f);
            Vector3 position2 = new Vector3(tezej.polozaj.X, 35.0f, 35.0f);
            Camera3D camera = new Camera3D();
            camera.LookAt(target, position, Vector3.UnitY);
            state.Camera.SetCamera(camera);

            mapa.Draw(state);
            minotaver.Draw(state);
            tezej.Draw(state);
            inventory.Draw(state);
            //this.debugText.Draw(state);
        }

        public void Update(UpdateState state)
        {
            if (!backgroundSongStart)
            {
                MediaPlayer.Play(backgroundSong);
                MediaPlayer.Volume = 0.6f;
                backgroundSongStart = true;
            }

            if (state.KeyboardState.KeyState.Escape.OnReleased)
            {
                stateManager.SetState(new MenuState());
                MediaPlayer.Stop();
            }

            //debugText.Text.SetText(tezej.polozaj.ToString() + " " + tezej.collisions.Count.ToString());
        }

        void IContentOwner.LoadContent(ContentState state)
        {
            this.debugText.Font = state.Load<SpriteFont>("Arial");
            backgroundSong = state.Load<Song>(@"backgroundSong");
            MediaPlayer.IsRepeating = true;
        }
    }
}
