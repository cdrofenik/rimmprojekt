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
        

        private Razredi.Minotaver minotaver;

        private Razredi.Tezej tezej;
        private Razredi.Mapa mapa;
        private Razredi.Inventory inventory;
        private Razredi.Enemy minotavek;
        private List<Razredi.Enemy> sovarzniki;

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

            
            //minotaver = new Razredi.Minotaver(35.0f, 0.0f, 20.0f, stateManager.Application.UpdateManager, stateManager.Application.Content, bodies);
            sovarzniki = generateEnemys(stateManager.Application.UpdateManager, stateManager.Application.Content, bodies);
            minotavek = new Razredi.Enemy(20.0f, 0.0f, 20.0f, stateManager.Application.UpdateManager, stateManager.Application.Content, bodies, "");

            tezej = new Razredi.Tezej(30.0f, 0.0f, 20.0f, stateManager.Application.UpdateManager, stateManager.Application.Content, bodies);
            inventory = new Razredi.Inventory(stateManager.Application.UpdateManager, stateManager.Application.Content,tezej);
           

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
            foreach (Razredi.Enemy goblin in sovarzniki)
                goblin.Draw(state);

            minotavek.Draw(state);
            //minotaver.Draw(state);
            tezej.Draw(state);
            inventory.Draw(state);
            this.debugText.Draw(state);
        }

        public void Update(UpdateState state)
        {
            if (!backgroundSongStart)
            {
                //MediaPlayer.Play(backgroundSong);
                MediaPlayer.Volume = 0.6f;
                backgroundSongStart = true;
            }

            if (state.KeyboardState.KeyState.K.OnPressed)
            {
                BattlingState bs = new BattlingState(tezej, minotaver, inventory,this.stateManager.Application);
                this.stateManager.SetState(bs);
            }

            if (state.KeyboardState.KeyState.Escape.OnReleased)
            {
                stateManager.SetState(new MenuState());
                MediaPlayer.Stop();
            }

            if (state.KeyboardState.KeyState.F.OnPressed)
            {
                inventory.addPotion("hp", 20);
            }

            debugText.Text.SetText(tezej.polozaj.ToString() + " " + tezej.collisions.Count.ToString());
        }

        void IContentOwner.LoadContent(ContentState state)
        {
            this.debugText.Font = state.Load<SpriteFont>("Arial");
            backgroundSong = state.Load<Song>(@"backgroundSong");
            MediaPlayer.IsRepeating = true;
        }

        private List<Razredi.Enemy> generateEnemys(UpdateManager updateMng, ContentRegister contntrg, List<Body> telesa)
        {
            String skin_ = "goblin";
            List<Razredi.Enemy> result = new List<Razredi.Enemy>();
            for(int i=0;i<6;i++)
            {
                Vector3 pozition = getEnemyPozition(i);
                result.Add(new Razredi.Enemy(pozition.X, pozition.Y, pozition.Z, updateMng, contntrg, telesa,skin_));
            }

            return result;
        }

        private Vector3 getEnemyPozition(Int32 i)
        {
            Random rndm = new Random();
            float[] x = new float[6];
            x[0] = float.Parse("21");
            x[1] = float.Parse("181");
            x[2] = float.Parse(rndm.Next(222,380).ToString());
            x[3] = float.Parse(rndm.Next(184, 376).ToString());
            x[4] = float.Parse("459");
            x[5] = float.Parse("459");

            float[] z = new float[6];
            z[0] = float.Parse(rndm.Next(150, 455).ToString());
            z[1] = float.Parse(rndm.Next(220, 455).ToString());
            z[2] = float.Parse("299");
            z[3] = float.Parse("99");
            z[4] = float.Parse(rndm.Next(23, 140).ToString());
            z[5] = float.Parse(rndm.Next(180, 360).ToString());

            Vector3 result = new Vector3(x[i],0.0f,z[i]);

            return result;
        }
    }
}
