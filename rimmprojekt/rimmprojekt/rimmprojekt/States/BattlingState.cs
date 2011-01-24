﻿using System;
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
    class BattlingState : IGameState, IContentOwner
    {
        private const String presledki = "                         ";
        private float PlayingTime;
        private float ActionTime;
        private float tempActionTime;
        private Boolean canDoAction;
        private Boolean gameOver;
        private Boolean gameFinished;


        #region risanje
        public Boolean isCharSelected;

        //risanje
        private Texture2D winForeground;
        private Texture2D backgroundPicture;
        private Texture2D pointer;
        private Texture2D leftBar;
        private Texture2D rightBar;
        private Texture2D hpBar;    //naredi sliko
        private Texture2D mpBar;    //naredi sliko
        private Texture2D timeBar;  //naredi sliko
        private Texture2D empty;  //naredi sliko
        private Texture2D gameOverTexture;  //naredi sliko
        private TexturedElement backgroundElement;
        private TexturedElement leftBarElement;
        private TexturedElement rightBarElement;
        private TexturedElement actionBarElement;
        private TexturedElement GameOverElement;
        private TexturedElement GameFinishedElement;
        private TexturedElement[] barArray;
        private TexturedElement[] emptybarArray;

        //risanje teksta
        private Int32 actionPointer;
        private String[] actionSelected;
        private TexturedElement[] actionTable;
        private SpriteFont smallFont;
        private SpriteFont bigFont;
        private TextElementRect txtEleRectLeftBar;
        private TextElement HealthTxtElement;
        private TextElement ManaTxtElement;
        private TextElement TimeTxtElement;
        private TextElement AttackTxtElement;
        private TextElement BlockTxtElement;
        private TextElement MagicTxtElement;
        private TextElement ItemsTxtElement;

        private TextElement debug;
        #endregion
        
        private Razredi.Mapa mapa;
        private DrawTargetScreen drawToScreen;
        private IGameStateManager stateManager;
        private Razredi.Tezej tezej;
        private Razredi.Minotaver minotaver;

        private Razredi.Character tezejChar;
        private Razredi.Character minotaverChar;

        public BattlingState(Razredi.Tezej theseus, Razredi.Minotaver minek,Application application)
        {
            drawToScreen = new DrawTargetScreen(new Camera3D());
            this.tezej = theseus;
            this.minotaver = minek;
        }

        public void Initalise(IGameStateManager stateManager)
        {
            this.stateManager = stateManager;

            gameFinished = false;
            gameOver = false;
            tempActionTime = 7.0f;
            actionTable = new TexturedElement[4];
            actionSelected = new String[4];
            isCharSelected = false;
            actionPointer = 3;
            barArray = new TexturedElement[3];
            emptybarArray = new TexturedElement[3];

            #region mapa tezej minotaver
            mapa = new Razredi.Mapa("../../../../rimmprojektContent/battleMap.txt", stateManager.Application.Content, stateManager.Application.UpdateManager);

            tezejChar = new Razredi.Character(10.0f, 0.0f, 40.0f, stateManager.Application.UpdateManager, stateManager.Application.Content, "tezej");
            minotaverChar = new Razredi.Character(80.0f, 0.0f, 40.0f, stateManager.Application.UpdateManager, stateManager.Application.Content, "minotaver");
            #endregion

            setFunctionalSettings();
            stateManager.Application.Content.Add(this);
        }

        public void DrawScreen(DrawState state)
        {
            ///Vector3 target = new Vector3(tezej.polozaj.X, tezej.polozaj.Y - 35.0f, tezej.polozaj.Z - 35.0f);
            if (tezejChar.isAttacking)
            {
                Vector3 target = new Vector3(tezejChar.polozaj.X, 0.0f, tezejChar.polozaj.Z);
                Vector3 position = new Vector3(tezejChar.polozaj.X - 15.0f, tezejChar.polozaj.Y + 25.0f, tezejChar.polozaj.Z + 20.0f);
                Camera3D camera = new Camera3D();
                camera.LookAt(target, position, Vector3.UnitY);
                state.Camera.SetCamera(camera);
            }
            else
            {
                Vector3 target = new Vector3(30.0f, 0.0f, 60.0f);
                Vector3 position = new Vector3(20, 22.0f, 90.0f);
                Camera3D camera = new Camera3D();
                camera.LookAt(target, position, Vector3.UnitY);
                state.Camera.SetCamera(camera);
            }

            backgroundElement.Draw(state);
            mapa.Draw(state);
            minotaverChar.Draw(state);
            tezejChar.Draw(state);

            leftBarElement.Draw(state);
            rightBarElement.Draw(state);

            foreach (TexturedElement i in emptybarArray)
                i.Draw(state);

            foreach (TexturedElement i in barArray)
                i.Draw(state);

            if (isCharSelected)
            {
                actionBarElement.Draw(state);
                actionTable[actionPointer].Texture = pointer;
                actionTable[actionPointer].Draw(state);
            }
            txtEleRectLeftBar.Draw(state);

            if (gameOver)
            {
                GameOverElement.Draw(state);
            }

            if (gameFinished)
            {
                GameFinishedElement.Draw(state);
            }
        }

        void IContentOwner.LoadContent(ContentState state)
        {
            pointer = state.Load<Texture2D>(@"Battle/pointer");
            smallFont = state.Load<SpriteFont>("Arial");
            bigFont = state.Load<SpriteFont>("ArialBattle");
            leftBar = state.Load<Texture2D>(@"Battle/leftBar");
            rightBar = state.Load<Texture2D>(@"Battle/rightBar");
            backgroundPicture = state.Load<Texture2D>(@"backBattle");
            hpBar = state.Load<Texture2D>(@"Tezej/hpbar");
            mpBar = state.Load<Texture2D>(@"Tezej/manabar");
            timeBar = state.Load<Texture2D>(@"Tezej/timebar");
            empty = state.Load<Texture2D>(@"Tezej/emptybar");
            gameOverTexture = state.Load<Texture2D>(@"Battle/gameOver");
            winForeground = state.Load<Texture2D>(@"Battle/winDisplay");

            setVisualSettings();
        }

        public void Update(UpdateState state)
        {
            PlayingTime += state.DeltaTimeSeconds;

            if (tezejChar.isDead)
            {
                gameOver = true;
                if (state.KeyboardState.KeyState.Space.OnPressed)
                {
                    stateManager.SetState(new MenuState());
                }
            }

            if (minotaverChar.isDead)
            {
                gameFinished = true;
                if (state.KeyboardState.KeyState.Space.OnPressed)
                {
                    //change state to playingstate
                }
            }

            if(!canDoAction)
                ActionTime += state.DeltaTimeSeconds;

            if (!gameOver)
            {
                if (minotaverChar.Health > 0)
                {
                    #region AI
                    if (Math.Round(PlayingTime) == tempActionTime || Math.Round(PlayingTime) > tempActionTime)
                    {
                        minotaverChar.goAttackEnemy(tezejChar.polozaj.X, 0);
                        tezejChar.damage = getAttackDamage(minotaverChar.Strength, tezejChar.Strength);
                        tezejChar.isTakingDamage = true;
                        tempActionTime = PlayingTime + 8.5f;
                    }
                    #endregion

                    #region notAI
                    if (canDoAction)
                    {
                        #region choose character
                        if (state.KeyboardState.KeyState.Enter.OnPressed || state.KeyboardState.KeyState.Space.OnPressed)
                        {
                            if (isCharSelected)
                            {
                                isCharSelected = false;
                                setActionBox(isCharSelected);
                                if (actionSelected[actionPointer].Equals("Attack"))
                                {
                                    tezejChar.goAttackEnemy(minotaverChar.polozaj.X, 0);
                                    minotaverChar.damage = getAttackDamage(tezejChar.Strength, minotaverChar.Strength);
                                    minotaverChar.isTakingDamage = true;
                                    actionDone(actionSelected[actionPointer]);
                                    tempActionTime += 1.9f;
                                }
                                else if (actionSelected[actionPointer].Equals("Block"))
                                {
                                    tezejChar.goBlock();
                                    actionDone(actionSelected[actionPointer]);
                                }

                            }
                            else
                            {
                                isCharSelected = true;
                                setActionBox(isCharSelected);
                            }
                        }
                        #endregion

                        if (isCharSelected)
                        {
                            #region keyboard input
                            if (state.KeyboardState.KeyState.W.OnPressed || state.KeyboardState.KeyState.D.OnPressed || state.KeyboardState.KeyState.Up.OnPressed
                                || state.KeyboardState.KeyState.Right.OnPressed)
                            {
                                if (actionPointer == 3)
                                    actionPointer = 0;
                                else
                                    actionPointer++;
                            }

                            if (state.KeyboardState.KeyState.S.OnPressed || state.KeyboardState.KeyState.A.OnPressed || state.KeyboardState.KeyState.Down.OnPressed
                                || state.KeyboardState.KeyState.Left.OnPressed)
                            {
                                if (actionPointer == 0)
                                    actionPointer = 3;
                                else
                                    actionPointer--;
                            }
                            #endregion
                        }
                    }
                    #endregion
                }
            }

            #region text output (hp and mana)
            changeStatusBars();
            HealthTxtElement.Text.SetText("HP:   " + tezejChar.Health + " / " + tezejChar.maxHealth);
            ManaTxtElement.Text.SetText("MP:   " + tezejChar.Mana + " / " + tezejChar.maxMana);
            debug.Text.SetText(Int32.Parse(Math.Round(PlayingTime).ToString()) + "     HP: " + minotaverChar.Health.ToString());
            #endregion
        }

        private void setVisualSettings()
        {
            #region PrimaryBars
            leftBarElement = new TexturedElement(leftBar, new Vector2(230, 190));
            leftBarElement.AlphaBlendState = Xen.Graphics.AlphaBlendState.Alpha;
            leftBarElement.VerticalAlignment = VerticalAlignment.Bottom;
            leftBarElement.HorizontalAlignment = HorizontalAlignment.Left;
            rightBarElement = new TexturedElement(rightBar, new Vector2(600, 190));
            rightBarElement.Position = new Vector2(230, 0);
            #endregion

            actionBarElement = new TexturedElement(leftBar, new Vector2(190, 190));
            actionBarElement.Position = new Vector2(110, 14);

            #region Text on bars
            txtEleRectLeftBar = new TextElementRect(new Vector2(700, 50));
            txtEleRectLeftBar.Font = bigFont;
            txtEleRectLeftBar.AlphaBlendState = Xen.Graphics.AlphaBlendState.Alpha;
            txtEleRectLeftBar.VerticalAlignment = VerticalAlignment.Bottom;
            txtEleRectLeftBar.HorizontalAlignment = HorizontalAlignment.Left;
            txtEleRectLeftBar.TextHorizontalAlignment = TextHorizontalAlignment.Left;
            txtEleRectLeftBar.Colour = Color.White;
            txtEleRectLeftBar.Position = new Vector2(40, 110);
            txtEleRectLeftBar.Text.SetText("Tezej");

            HealthTxtElement = new TextElement("HP: ");
            HealthTxtElement.Font = smallFont;
            HealthTxtElement.Text.SetText("HP: ");
            HealthTxtElement.Colour = Color.White;
            HealthTxtElement.Position = new Vector2(280, 4);

            ManaTxtElement = new TextElement("MP: ");
            ManaTxtElement.Font = smallFont;
            ManaTxtElement.Text.SetText("MP: ");
            ManaTxtElement.Colour = Color.White;
            ManaTxtElement.Position = new Vector2(430, 4);

            TimeTxtElement = new TextElement("TIME: ");
            TimeTxtElement.Font = smallFont;
            TimeTxtElement.Text.SetText("TIME: ");
            TimeTxtElement.Colour = Color.White;
            TimeTxtElement.Position = new Vector2(570, 0);

            txtEleRectLeftBar.Add(HealthTxtElement);
            txtEleRectLeftBar.Add(ManaTxtElement);
            txtEleRectLeftBar.Add(TimeTxtElement);

            #region actionBar
            AttackTxtElement = new TextElement("Attack");
            AttackTxtElement.Font = bigFont;
            AttackTxtElement.Text.SetText("Attack");
            AttackTxtElement.Colour = Color.White;
            AttackTxtElement.Position = new Vector2(120, 20);

            BlockTxtElement = new TextElement("Block");
            BlockTxtElement.Font = bigFont;
            BlockTxtElement.Text.SetText("Block");
            BlockTxtElement.Colour = Color.White;
            BlockTxtElement.Position = new Vector2(120, -20);

            MagicTxtElement = new TextElement("Magic");
            MagicTxtElement.Font = bigFont;
            MagicTxtElement.Text.SetText("Magic");
            MagicTxtElement.Colour = Color.White;
            MagicTxtElement.Position = new Vector2(120, -60);

            ItemsTxtElement = new TextElement("Items");
            ItemsTxtElement.Font = bigFont;
            ItemsTxtElement.Text.SetText("Items");
            ItemsTxtElement.Colour = Color.White;
            ItemsTxtElement.Position = new Vector2(120, -100);
            #endregion

            #endregion

            #region health, mana and time bars
            barArray[0] = new TexturedElement(new Vector2(100, 4));
            barArray[0].Texture = hpBar;
            barArray[0].Position = new Vector2(320, 145);
            barArray[1] = new TexturedElement(new Vector2(100, 4));
            barArray[1].Texture = mpBar;
            barArray[1].Position = new Vector2(470, 145);
            barArray[2] = new TexturedElement(new Vector2(100, 15));
            barArray[2].Texture = timeBar;
            barArray[2].Position = new Vector2(650, 145);

            emptybarArray[0] = new TexturedElement(new Vector2(100, 4));
            emptybarArray[0].Texture = empty;
            emptybarArray[0].Position = new Vector2(320, 145);
            emptybarArray[1] = new TexturedElement(new Vector2(100, 4));
            emptybarArray[1].Texture = empty;
            emptybarArray[1].Position = new Vector2(470, 145);
            emptybarArray[2] = new TexturedElement(new Vector2(100, 15));
            emptybarArray[2].Texture = empty;
            emptybarArray[2].Position = new Vector2(650, 145);
            #endregion


            Vector2 pointerSize = new Vector2(20, 20);
            actionTable[0] = new TexturedElement(pointerSize);
            actionTable[0].Position = new Vector2( 130, 34);
            actionTable[1] = new TexturedElement(pointerSize);
            actionTable[1].Position = new Vector2( 130, 74);
            actionTable[2] = new TexturedElement(pointerSize);
            actionTable[2].Position = new Vector2( 130, 114);
            actionTable[3] = new TexturedElement(pointerSize);
            actionTable[3].Position = new Vector2( 130, 154);

            backgroundElement = new TexturedElement(new Vector2(1280, 400));
            backgroundElement.Position = new Vector2(0,400);
            backgroundElement.Texture = backgroundPicture;

            debug = new TextElement();
            debug.Font = smallFont;
            debug.Position = new Vector2(400, 500);

            txtEleRectLeftBar.Add(debug);

            GameOverElement = new TexturedElement(new Vector2(1280, 720));
            GameOverElement.Texture = gameOverTexture;

            GameFinishedElement = new TexturedElement(new Vector2(700, 502));
            GameFinishedElement.Position = new Vector2(260, 140);
            GameFinishedElement.Texture = winForeground;
        }

        private void setFunctionalSettings()
        {
            actionSelected[3] = "Attack";
            actionSelected[2] = "Block";
            actionSelected[1] = "Magic";
            actionSelected[0] = "Items";

            tezej.isInBattle = true;
            tezejChar.changeCharacterOrientation("down", "right");
            tezejChar.Strength = tezej.strength+1000;
            tezejChar.Agility = tezej.agility;
            tezejChar.Intelligence = tezej.intelligence;
            tezejChar.Vitality = tezej.vitality;
            tezejChar.setStats();

            minotaverChar.changeCharacterOrientation("down", "left");
            minotaverChar.Strength = 100000;
            minotaverChar.Agility = 5;
            minotaverChar.Intelligence = 0;
            minotaverChar.Vitality = 20;
            minotaverChar.setStats();
            
        }

        private void setActionBox(Boolean value)
        {
            if (value)
            {
                actionBarElement.AlphaBlendState = Xen.Graphics.AlphaBlendState.Alpha;
                actionBarElement.VerticalAlignment = VerticalAlignment.Bottom;
                actionBarElement.HorizontalAlignment = HorizontalAlignment.Left;

                txtEleRectLeftBar.Add(AttackTxtElement);
                txtEleRectLeftBar.Add(BlockTxtElement);
                txtEleRectLeftBar.Add(MagicTxtElement);
                txtEleRectLeftBar.Add(ItemsTxtElement);
            }
            else
            {
                txtEleRectLeftBar.Remove(AttackTxtElement);
                txtEleRectLeftBar.Remove(BlockTxtElement);
                txtEleRectLeftBar.Remove(MagicTxtElement);
                txtEleRectLeftBar.Remove(ItemsTxtElement);
            }
        }

        private void changeStatusBars()
        {
            double timeSize = 0;

            double hpSize = (tezejChar.Health * 100) / tezejChar.maxHealth;
            barArray[0] = new TexturedElement(new Vector2(Int32.Parse(hpSize.ToString()), 4));
            barArray[0].Texture = hpBar;
            barArray[0].Position = new Vector2(320, 145);

            double mpSize = (tezejChar.Mana * 100) / tezejChar.maxMana;
            barArray[1] = new TexturedElement(new Vector2(Int32.Parse(mpSize.ToString()), 4));
            barArray[1].Texture = mpBar;
            barArray[1].Position = new Vector2(470, 145);

            if (!canDoAction)
            {
                timeSize = Math.Round((ActionTime * 100) / 5);
                barArray[2] = new TexturedElement(new Vector2(Int32.Parse(timeSize.ToString()), 15));
                barArray[2].Texture = timeBar;
                barArray[2].Position = new Vector2(650, 145);
            }

            if (timeSize == 100 || timeSize > 100)
            {
                canDoAction = true;
            }
        }

        private void actionDone(String action)
        {
            ActionTime = 0.0f;
            canDoAction = false;
        }

        private Int32 getAttackDamage(Int32 atkStr, Int32 defStr)
        {
            Int32 result = 0;

            Random random = new Random();
            if(defStr>atkStr)
                result = random.Next(atkStr, atkStr+atkStr);
            else
                result = random.Next(defStr, atkStr+100);

            return result;
        }
    }
}