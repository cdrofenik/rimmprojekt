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
        private Boolean endGame;
        Song backgroundSong;
        Boolean backgroundSongStart = false;
        private TextElementRect debugText;
        private TexturedElement backgroundPicture;

        private DrawTargetScreen drawToScreen;
        private IGameStateManager stateManager;

        private Razredi.EndGame endGameClass;
        private Razredi.GameData gameData;

        public PlayingState(Application application)
        {
            drawToScreen = new DrawTargetScreen(new Camera3D());
        }

        public PlayingState(Razredi.GameData gameData)
        {
            drawToScreen = new DrawTargetScreen(new Camera3D());
            this.gameData = gameData;
        }

        public void Initalise(IGameStateManager stateManager)
        {
            this.stateManager = stateManager;

            if (gameData != null)
                gameData = new Razredi.GameData(stateManager, gameData);
            else
                gameData = new Razredi.GameData(stateManager);

            endGame = false;
            battleStart = false;
            drawAnimationForBattle = false;
            battleStartTimer = 9999.0f;
            PlayingTime = 0.0f;
            battleAnimElement = new SolidColourElement[4];
            backgroundPicture = new TexturedElement(new Vector2(1280, 720));

            this.endGameClass = new Razredi.EndGame(stateManager.Application.UpdateManager,stateManager.Application.Content);
            this.debugText = new TextElementRect(new Vector2(400, 100));
            this.debugText.Position = new Vector2(340, 240);
            this.debugText.Colour = Color.Pink;
            this.debugText.Text.SetText("Debug Text");
            this.debugText.VerticalAlignment = VerticalAlignment.Centre;
            this.debugText.HorizontalAlignment = HorizontalAlignment.Centre;
            this.debugText.TextHorizontalAlignment = TextHorizontalAlignment.Centre;

            stateManager.Application.Content.Add(this);
        }

        //simplified IDraw/IUpdate
        //NOTE:
        //For simplicity this only provides drawing directly to the screen.
        public void DrawScreen(DrawState state)
        {
            //Vector3 target = new Vector3(gameData.Tezej.polozaj.X, gameData.Tezej.polozaj.Y - 35.0f, gameData.Tezej.polozaj.Z - 10.0f);

            Vector3 target = new Vector3(gameData.Tezej.polozaj.X, gameData.Tezej.polozaj.Y - 35.0f, gameData.Tezej.polozaj.Z - 13.0f);
            Vector3 position = new Vector3(gameData.Tezej.polozaj.X, gameData.Tezej.polozaj.Y + 28.0f, gameData.Tezej.polozaj.Z + 30.0f);

            Vector3 position2 = new Vector3(gameData.Tezej.polozaj.X, 35.0f, 35.0f);
            Camera3D camera = new Camera3D();
            camera.LookAt(target, position, Vector3.UnitY);
            state.Camera.SetCamera(camera);

            backgroundPicture.Draw(state);
            gameData.Draw(state);
            //this.debugText.Draw(state);

            if (drawAnimationForBattle)
            {
                for (int i = 0; i < 4; i++)
                {
                    battleAnimElement[i] = new SolidColourElement(new Color(20, 20, 40), new Vector2(Int32.Parse(Math.Round(battleAnimCounter * 2).ToString()),
                        Int32.Parse(Math.Round(battleAnimCounter).ToString())));
                    battleAnimElement[i].Position = postavitev[i];
                    battleAnimElement[i].Draw(state);
                }
            }

            if (endGame)
            {
                endGameClass.Draw(state);
            }

        }

        public void Update(UpdateState state)
        {
            PlayingTime += state.DeltaTimeSeconds;

            //endgame
            //if(minotaver.isDead) {
            //  endGame=true;
            //}

            #region draw battle animation
            if (drawAnimationForBattle)
                battleAnimCounter += 10.0f;

            if (PlayingTime > battleStartTimer)
            {
                MediaPlayer.Stop();
                this.stateManager.SetState(new BattlingState(gameData));
            }
            #endregion

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

            gameData.Update(state);

            foreach (Razredi.Enemy e in gameData.Sovrazniki)
            {
                if ((Math.Abs(gameData.Tezej.polozaj.X - e.polozaj.X) + Math.Abs(gameData.Tezej.polozaj.Z - e.polozaj.Z)) < 10f)
                {
                    gameData.Tezej.isInBattle = true;

                    if (!battleStart)
                    {
                        battleStartTimer = PlayingTime + 1.7f;
                        drawAnimationForBattle = true;
                        battleStart = true;
                        
                    }
                    //this.stateManager.SetState(new BattlingState(gameData));
                    break;
                }
            }

            if (state.KeyboardState.KeyState.LeftControl.IsDown && state.KeyboardState.KeyState.S.OnReleased)
            {
                gameData.shrani();
            }

            if (state.KeyboardState.KeyState.F.OnPressed)
            {
                endGame = true;
            }

            //debugText.Text.SetText(gameData.Tezej.polozaj.ToString() + " " + gameData.Tezej.collisions.Count.ToString());
        }

        void IContentOwner.LoadContent(ContentState state)
        {
            this.debugText.Font = state.Load<SpriteFont>("Arial");
            backgroundSong = state.Load<Song>(@"backgroundSong");
            backgroundPicture.Texture = state.Load<Texture2D>("Textures/tla2");
            MediaPlayer.IsRepeating = true;
        }
    }
}
