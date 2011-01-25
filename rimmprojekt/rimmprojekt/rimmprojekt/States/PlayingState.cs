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
        #region battleIntro
        private float battleStartTimer;
        private Boolean battleStart;
        private Boolean drawAnimationForBattle;
        private SolidColourElement[] battleAnimElement;
        private Vector2[] postavitev = new Vector2[4] { new Vector2(0, 360), new Vector2(640, 360), new Vector2(0, 0), new Vector2(640, 0) };
        private float battleAnimCounter;
        #endregion

        private float PlayingTime;

        Song backgroundSong;
        Boolean backgroundSongStart = false;
        private TextElementRect debugText;
        private TexturedElement backgroundPicture;

        private DrawTargetScreen drawToScreen;
        private IGameStateManager stateManager;

        private Razredi.Tezej tezej;
        private Razredi.Mapa mapa;
        private Razredi.Inventory inventory;
        private Razredi.Enemy minotavek;
        private List<Razredi.Enemy> sovarzniki;

        private bool IsActive;

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

            battleStart = false;
            drawAnimationForBattle = false;
            battleStartTimer = 9999.0f;
            PlayingTime = 0.0f;
            battleAnimElement = new SolidColourElement[4];
            backgroundPicture = new TexturedElement(new Vector2(1280, 720));

            sovarzniki = generateEnemys(stateManager.Application.UpdateManager, stateManager.Application.Content, bodies, 7);
            minotavek = new Razredi.Enemy(20.0f, 0.0f, 20.0f, stateManager.Application.UpdateManager, stateManager.Application.Content, bodies, "");

            tezej = new Razredi.Tezej(0.0f, 0.0f, 20.0f, stateManager.Application.UpdateManager, stateManager.Application.Content, bodies);
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

        bool IGameState.isActive
        {
            get { return IsActive; }
            set { IsActive = value; }
        }

        private void InitialisePhysics()
        {

        }

        //simplified IDraw/IUpdate
        //NOTE:
        //For simplicity this only provides drawing directly to the screen.
        public void DrawScreen(DrawState state)
        {
            //Vector3 target = new Vector3(tezej.polozaj.X, tezej.polozaj.Y - 35.0f, tezej.polozaj.Z - 10.0f);
            if (IsActive)
            {
                Vector3 target = new Vector3(tezej.polozaj.X, tezej.polozaj.Y - 35.0f, tezej.polozaj.Z - 13.0f);
                Vector3 position = new Vector3(tezej.polozaj.X, tezej.polozaj.Y + 28.0f, tezej.polozaj.Z + 30.0f);
                Vector3 position2 = new Vector3(tezej.polozaj.X, 35.0f, 35.0f);
                Camera3D camera = new Camera3D();
                camera.LookAt(target, position, Vector3.UnitY);
                state.Camera.SetCamera(camera);

                backgroundPicture.Draw(state);
                mapa.Draw(state);
                foreach (Razredi.Enemy goblin in sovarzniki)
                    goblin.Draw(state);

                minotavek.Draw(state);
                //minotaver.Draw(state);
                tezej.Draw(state);
                inventory.Draw(state);
                this.debugText.Draw(state);

                if (drawAnimationForBattle)
                {
                    for(int i = 0;i<4;i++)
                    {
                        battleAnimElement[i] = new SolidColourElement(new Color(20, 20, 40), new Vector2(Int32.Parse(Math.Round(battleAnimCounter*2).ToString()),
                            Int32.Parse(Math.Round(battleAnimCounter).ToString())));
                        battleAnimElement[i].Position = postavitev[i];
                        battleAnimElement[i].Draw(state);
                    }
                }
            }
        }

        public void Update(UpdateState state)
        {
            PlayingTime += state.DeltaTimeSeconds;

            #region draw battle animation
            if (drawAnimationForBattle)
                battleAnimCounter += 10.0f;


            if (battleStart)
            {
                battleStartTimer = PlayingTime + 1.7f;
                battleStart = false;
            }

            if (PlayingTime > battleStartTimer)
            {
                BattlingState bs = new BattlingState(tezej, sovarzniki.ElementAt<Razredi.Enemy>(2), inventory, this.stateManager.Application);
                this.stateManager.SetState(bs);
            }
            #endregion

            if (IsActive)
            {
                if (!backgroundSongStart)
                {
                    //MediaPlayer.Play(backgroundSong);
                    MediaPlayer.Volume = 0.6f;
                    backgroundSongStart = true;
                }

                if (state.KeyboardState.KeyState.K.OnPressed)
                {
                    battleStart = true;
                    drawAnimationForBattle = true;
                }

                if (state.KeyboardState.KeyState.Escape.OnReleased)
                {
                    stateManager.SetState(new MenuState());
                    MediaPlayer.Stop();
                }
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

            backgroundPicture.Texture = state.Load<Texture2D>("Textures/tla2");
        }

        private List<Razredi.Enemy> generateEnemys(UpdateManager updateMng, ContentRegister contntrg, List<Body> telesa,int count)
        {
            String skin_ = "goblin";
            List<Razredi.Enemy> result = new List<Razredi.Enemy>();
            for (int i = 0; i < count; i++)
            {
                Vector3 pozition = getEnemyPosition(i);
                Razredi.Enemy goblin = new Razredi.Enemy(pozition.X, pozition.Y, pozition.Z, updateMng, contntrg, telesa,skin_);
                goblin.goblin_string = "goblin";
                result.Add(goblin);
            }

            return result;
        }

        private Vector3 getEnemyPosition(Int32 i)
        {
            Random rndm = new Random();
            float[] x = new float[7];
            x[0] = float.Parse("143");
            x[1] = float.Parse("300");
            x[2] = float.Parse(rndm.Next(300,498).ToString());
            x[3] = float.Parse("541");
            x[4] = float.Parse("581");
            x[5] = float.Parse(rndm.Next(422, 499).ToString());
            x[6] = float.Parse(rndm.Next(341, 388).ToString());

            float[] z = new float[7];
            z[0] = float.Parse(rndm.Next(380, 575).ToString());
            z[1] = float.Parse(rndm.Next(341, 575).ToString());
            z[2] = float.Parse("220");
            z[3] = float.Parse(rndm.Next(140, 299).ToString());
            z[4] = float.Parse(rndm.Next(303, 501).ToString());
            z[5] = float.Parse("501");
            z[6] = float.Parse("418");

            Vector3 result = new Vector3(x[i],0.0f,z[i]);

            return result;
        }

        private Vector3 getRandomMinotaverPosition()
        {
            Random rndm = new Random();

            float[] x = new float[3];
            x[0] = float.Parse(rndm.Next(502, 577).ToString());
            x[1] = float.Parse(rndm.Next(501, 580).ToString());
            x[2] = float.Parse(rndm.Next(384, 581).ToString());

            float[] z = new float[3];
            z[0] = float.Parse("575");
            z[1] = float.Parse("504");
            z[2] = float.Parse("338");


            int tableIndex = rndm.Next(1, 3);

            return new Vector3(x[tableIndex], 0.0f, z[tableIndex]);
        }
    }
}
