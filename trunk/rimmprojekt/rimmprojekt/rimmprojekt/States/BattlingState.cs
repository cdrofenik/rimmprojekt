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
using Microsoft.Xna.Framework.Audio;

using JigLibX.Math;
using JigLibX.Physics;
//using JigLibX.Geometry;
using JigLibX.Collision;

namespace rimmprojekt.States
{
    class BattlingState : IGameState, IContentOwner
    {
        Song backgroundSong;
        Boolean backgroundSongStart = false;
        SoundEffect selectSoundEffect;

        #region intro
        private float cameraTargetcounter = 140.0f;
        private Boolean intro;
        private float StartBattleTimer;
        #endregion

        private int TestDamage;

        private Razredi.EndGame endGameClass;
        private Boolean endGame;
        private TextElementRect awardText;
        private Int32 expGained;
        private Int32 hpPotionsGained;
        private Int32 mpPotionsGained;
        private const String presledki = "                                  ";
        private float PlayingTime;
        private float ActionTime;
        private float tempActionTime;
        private Boolean canDoAction;
        private Boolean gameOver;
        private Boolean gameFinished;

        private Razredi.GameData gameData;

        #region Razredi
        private Razredi.Enemy enemy;
        private Razredi.Character tezejChar;
        private Razredi.Character enemyChar;
        #endregion

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
        private SpriteFont mediumfonT;
        private SpriteFont bigFont;
        private TextElementRect txtEleRectLeftBar;
        private TextElement HealthTxtElement;
        private TextElement ManaTxtElement;
        private TextElement TimeTxtElement;
        private TextElement AttackTxtElement;
        private TextElement BlockTxtElement;
        private TextElement MagicTxtElement;
        private TextElement ItemsTxtElement;

        private TextElement winTxTelement;
        private TextElement looseTxTelement;

        private TextElement debug;
        #endregion

        private Razredi.Mapa mapa;
        //private Razredi.Inventory inventory;

        private IGameStateManager stateManager;


        public BattlingState(Razredi.GameData gameData)
        {
            
            this.gameData = gameData;
            //this.inventory = gameData.Inventory;
            foreach (Razredi.Enemy e in this.gameData.Sovrazniki)
            {
                if ((Math.Abs(this.gameData.Tezej.polozaj.X - e.polozaj.X) + Math.Abs(this.gameData.Tezej.polozaj.Z - e.polozaj.Z)) < 15f)
                {
                    this.enemy = e;
                    break;
                }
            }
        }

        public void Initalise(IGameStateManager stateManager)
        {
            this.stateManager = stateManager;

            #region parameter inicialization
            this.endGameClass = new Razredi.EndGame(stateManager.Application.UpdateManager, stateManager.Application.Content);
            intro = true;
            gameFinished = false;
            gameOver = false;
            tempActionTime = 7.0f;
            actionTable = new TexturedElement[3];
            actionSelected = new String[3];
            isCharSelected = false;
            actionPointer = 2;
            barArray = new TexturedElement[3];
            emptybarArray = new TexturedElement[3];
            #endregion

            #region gameData.Tezej minotaver
            mapa = new Razredi.Mapa("../../../../rimmprojektContent/battleMap.txt", stateManager.Application.Content);

            tezejChar = new Razredi.Character(40.0f, 0.0f, 60.0f, stateManager.Application.UpdateManager, stateManager.Application.Content, "tezej");
            enemyChar = new Razredi.Character(110.0f, 0.0f, 60.0f, stateManager.Application.UpdateManager, stateManager.Application.Content, enemy.skin_value);
            #endregion

            setFunctionalSettings();
            stateManager.Application.Content.Add(this);
        }

        public void DrawScreen(DrawState state)
        {
            //Vector3 target = new Vector3(gameData.Tezej.polozaj.X, gameData.Tezej.polozaj.Y - 35.0f, gameData.Tezej.polozaj.Z - 35.0f);
            Vector3 target = new Vector3();
            Vector3 position = new Vector3(30.0f, 9.0f, 90.0f);

            if (intro)
            {
                target = new Vector3(tezejChar.polozaj.X + 30, tezejChar.polozaj.Y + Int32.Parse(Math.Round(cameraTargetcounter).ToString()), tezejChar.polozaj.Z - 35.0f);
            }
            else
            {
                target = new Vector3(tezejChar.polozaj.X + 30, tezejChar.polozaj.Y + 2, tezejChar.polozaj.Z - 35.0f);
            }

            Camera3D camera = new Camera3D();
            camera.LookAt(target, position, Vector3.UnitY);
            state.Camera.SetCamera(camera);

            mapa.Draw(state);
            enemyChar.Draw(state);
            tezejChar.Draw(state);

            #region drawing elements (sprites and textures and text)
            if (!intro)
            {
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
            }
            #endregion

            if (gameOver)
            {
                GameOverElement.Draw(state);
                looseTxTelement.Draw(state);
            }

            if (gameFinished)
            {
                GameFinishedElement.Draw(state);
                winTxTelement.Draw(state);
                awardText.Draw(state);
            }

            gameData.Inventory.Draw(state);
            //debug.Draw(state);

            if (endGame)
            {
                endGameClass.Draw(state);
            }
        }

        void IContentOwner.LoadContent(ContentState state)
        {
            selectSoundEffect = state.Load<SoundEffect>(@"Battle/battle_select_sound");
            backgroundSong = state.Load<Song>(@"Battle/battle_background_song");
            pointer = state.Load<Texture2D>(@"Battle/pointer");
            smallFont = state.Load<SpriteFont>("Arial");
            bigFont = state.Load<SpriteFont>("ArialBattle");
            mediumfonT = state.Load<SpriteFont>("ArialMedium");
            leftBar = state.Load<Texture2D>(@"Battle/leftBar");
            rightBar = state.Load<Texture2D>(@"Battle/rightBar");
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
            if (!backgroundSongStart)
            {
                MediaPlayer.Play(backgroundSong);
                MediaPlayer.Volume = 0.6f;
                backgroundSongStart = true;
            }


            if (endGame &&  state.KeyboardState.KeyState.Space)
            {
                stateManager.SetState(new MenuState());
            }


            if (intro)
            {
                StartBattleTimer += state.DeltaTimeSeconds;
                cameraTargetcounter -= 0.41f;
            }

            if (Math.Round(StartBattleTimer) == 6.0f)
                intro = false;

            #region after intro
            if (!intro)
            {
                PlayingTime += state.DeltaTimeSeconds;

                #region tezej dies
                if (tezejChar.isDead)
                {
                    gameOver = true;
                    if (state.KeyboardState.KeyState.Space.OnPressed)
                    {
                        MediaPlayer.Stop();
                        stateManager.SetState(new MenuState());
                    }
                }
                #endregion

                #region enemy dies
                if (enemyChar.isDead)
                {
                    if(enemy.skin_value.Equals(""))
                    {
                        endGame = true;
                    } else
                    {
                        awardGenerator();
                        gameFinished = true;
                        if (state.KeyboardState.KeyState.Space.OnPressed)
                        {
                            MediaPlayer.Stop();
                            gameData.Sovrazniki.Remove(enemy);
                            stateManager.SetState(new PlayingState(gameData));
                        }
                    }
                }
                #endregion

                if (!canDoAction)
                    ActionTime += state.DeltaTimeSeconds;

                if (!gameOver)
                {
                    if (enemyChar.Health > 0)
                    {
                        #region AI
                        if (Math.Round(PlayingTime) == tempActionTime || Math.Round(PlayingTime) > tempActionTime)
                        {
                            if(checkIfcanAttack(PlayingTime,ActionTime))
                            {
                                enemyChar.goAttackEnemy(tezejChar.polozaj.X, 0);
                                tezejChar.damage = getAttackDamage(enemyChar, tezejChar);
                                tezejChar.doDamageAnimation(1.3f);
                                tempActionTime = PlayingTime + 7.0f;
                            }
                        }
                        #endregion

                        #region notAI
                        if (canDoAction && !gameData.Inventory.isInBattle)
                        {
                            #region choose character
                            if (state.KeyboardState.KeyState.Enter.OnPressed || state.KeyboardState.KeyState.Space.OnPressed)
                            {
                                if (isCharSelected)
                                {
                                    if(checkIfcanAttack(PlayingTime,tempActionTime))
                                    {
                                        if (actionSelected[actionPointer].Equals("Attack"))
                                        {
                                            tezejChar.goAttackEnemy(enemyChar.polozaj.X, 0);
                                            enemyChar.damage = getAttackDamage(tezejChar, enemyChar);
                                            enemyChar.doDamageAnimation(1.3f);
                                            actionDone(actionSelected[actionPointer]);
                                            tempActionTime += 1.9f;
                                            isCharSelected = false;
                                            setActionBox(isCharSelected);
                                        }
                                        else if (actionSelected[actionPointer].Equals("Block"))
                                        {
                                            tezejChar.goBlock();
                                            actionDone(actionSelected[actionPointer]);
                                            isCharSelected = false;
                                            setActionBox(isCharSelected);
                                        }
                                        else if (actionSelected[actionPointer].Equals("Items"))
                                        {
                                            gameData.Inventory.isInBattle = true;
                                            //actionDone(actionSelected[actionPointer]);
                                        }
                                    }
                                }
                            }
                            #endregion

                            if (isCharSelected)
                            {
                                #region keyboard input
                                if (state.KeyboardState.KeyState.W.OnPressed || state.KeyboardState.KeyState.D.OnPressed || state.KeyboardState.KeyState.Up.OnPressed
                                    || state.KeyboardState.KeyState.Right.OnPressed)
                                {
                                    selectSoundEffect.Play();
                                    if (actionPointer == 2)
                                        actionPointer = 0;
                                    else
                                        actionPointer++;
                                }

                                if (state.KeyboardState.KeyState.S.OnPressed || state.KeyboardState.KeyState.A.OnPressed || state.KeyboardState.KeyState.Down.OnPressed
                                    || state.KeyboardState.KeyState.Left.OnPressed)
                                {
                                    selectSoundEffect.Play();
                                    if (actionPointer == 0)
                                        actionPointer = 2;
                                    else
                                        actionPointer--;
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            if (state.KeyboardState.KeyState.A.OnPressed || state.KeyboardState.KeyState.Escape.OnPressed)
                            {
                                gameData.Inventory.isInBattle = false;
                            }

                            if (gameData.Inventory.isItemUsed)
                            {
                                actionDone(actionSelected[actionPointer]);
                                isCharSelected = false;
                                setActionBox(isCharSelected);
                                gameData.Inventory.isInBattle = false;
                                gameData.Inventory.isItemUsed = false;
                            }
                        }
                        #endregion
                    }
                }

                #region text output (hp and mana)
                changeStatusBars();
                HealthTxtElement.Text.SetText("    HP:   " + tezejChar.Health + " / " + tezejChar.maxHealth);
                ManaTxtElement.Text.SetText("    MP:   " + tezejChar.Mana + " / " + tezejChar.maxMana);
                //debug.Text.SetText(Int32.Parse(Math.Round(PlayingTime).ToString()) + "     HP: " + enemyChar.Health.ToString() + "\n" + TestDamage.ToString()
                //    + "\n");
                #endregion
            }
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
            actionBarElement.Position = new Vector2(120, 14);

            #region Text on bars
            txtEleRectLeftBar = new TextElementRect(new Vector2(700, 50));
            txtEleRectLeftBar.Font = bigFont;
            txtEleRectLeftBar.AlphaBlendState = Xen.Graphics.AlphaBlendState.Alpha;
            txtEleRectLeftBar.VerticalAlignment = VerticalAlignment.Bottom;
            txtEleRectLeftBar.HorizontalAlignment = HorizontalAlignment.Left;
            txtEleRectLeftBar.TextHorizontalAlignment = TextHorizontalAlignment.Left;
            txtEleRectLeftBar.Colour = Color.White;
            txtEleRectLeftBar.Position = new Vector2(25, 110);
            txtEleRectLeftBar.Text.SetText("Theseus");

            HealthTxtElement = new TextElement("HP: ");
            HealthTxtElement.Font = mediumfonT;
            HealthTxtElement.Text.SetText("  HP: ");
            HealthTxtElement.Colour = Color.White;
            HealthTxtElement.Position = new Vector2(280, 4);

            ManaTxtElement = new TextElement("MP: ");
            ManaTxtElement.Font = mediumfonT;
            ManaTxtElement.Text.SetText("MP: ");
            ManaTxtElement.Colour = Color.White;
            ManaTxtElement.Position = new Vector2(430, 4);

            TimeTxtElement = new TextElement("TIME: ");
            TimeTxtElement.Font = mediumfonT;
            TimeTxtElement.Text.SetText("   TIME: ");
            TimeTxtElement.Colour = Color.White;
            TimeTxtElement.Position = new Vector2(568, 0);

            txtEleRectLeftBar.Add(HealthTxtElement);
            txtEleRectLeftBar.Add(ManaTxtElement);
            txtEleRectLeftBar.Add(TimeTxtElement);

            #region actionBar
            AttackTxtElement = new TextElement("Attack");
            AttackTxtElement.Font = bigFont;
            AttackTxtElement.Text.SetText("Attack");
            AttackTxtElement.Colour = Color.White;
            AttackTxtElement.Position = new Vector2(150, 20);

            BlockTxtElement = new TextElement("Block");
            BlockTxtElement.Font = bigFont;
            BlockTxtElement.Text.SetText("Block");
            BlockTxtElement.Colour = Color.White;
            BlockTxtElement.Position = new Vector2(150, -20);

            //MagicTxtElement = new TextElement("Magic");
            //MagicTxtElement.Font = bigFont;
            //MagicTxtElement.Text.SetText("Magic");
            //MagicTxtElement.Colour = Color.White;
            //MagicTxtElement.Position = new Vector2(150, -60);

            ItemsTxtElement = new TextElement("Items");
            ItemsTxtElement.Font = bigFont;
            ItemsTxtElement.Text.SetText("Items");
            ItemsTxtElement.Colour = Color.White;
            ItemsTxtElement.Position = new Vector2(150, -60);
            #endregion


            winTxTelement = new TextElement("You Win");
            winTxTelement.Font = bigFont;
            winTxTelement.Colour = Color.Yellow;
            winTxTelement.Text.SetText("You Win!");
            winTxTelement.Position = new Vector2(600, -180);

            looseTxTelement = new TextElement("You Loose");
            looseTxTelement.Font = bigFont;
            looseTxTelement.Colour = Color.White;
            looseTxTelement.Text.SetText("[Press SPACE to play game again!]");
            looseTxTelement.Position = new Vector2(450, -600);
            #endregion

            #region health, mana and time bars
            emptybarArray[0] = new TexturedElement(new Vector2(105, 4));
            emptybarArray[0].Texture = empty;
            emptybarArray[0].Position = new Vector2(320, 141);
            emptybarArray[1] = new TexturedElement(new Vector2(107, 4));
            emptybarArray[1].Texture = empty;
            emptybarArray[1].Position = new Vector2(470, 141);
            emptybarArray[2] = new TexturedElement(new Vector2(100, 15));
            emptybarArray[2].Texture = empty;
            emptybarArray[2].Position = new Vector2(650, 145);
            #endregion


            Vector2 pointerSize = new Vector2(20, 20);
            //actionTable[0] = new TexturedElement(pointerSize);
            //actionTable[0].Position = new Vector2( 143, 34);
            actionTable[0] = new TexturedElement(pointerSize);
            actionTable[0].Position = new Vector2( 143, 74);
            actionTable[1] = new TexturedElement(pointerSize);
            actionTable[1].Position = new Vector2( 143, 114);
            actionTable[2] = new TexturedElement(pointerSize);
            actionTable[2].Position = new Vector2(143, 154);

            debug = new TextElement();
            debug.Font = smallFont;
            debug.Position = new Vector2(400, 500);

            txtEleRectLeftBar.Add(debug);

            GameOverElement = new TexturedElement(new Vector2(1280, 720));
            GameOverElement.Texture = gameOverTexture;

            GameFinishedElement = new TexturedElement(new Vector2(505, 503));
            GameFinishedElement.Position = new Vector2(390, 140);
            GameFinishedElement.Texture = winForeground;

            //awardText
            awardText = new TextElementRect(new Vector2(700, 80));
            awardText.Font = mediumfonT;
            awardText.AlphaBlendState = Xen.Graphics.AlphaBlendState.Alpha;
            awardText.VerticalAlignment = VerticalAlignment.Centre;
            awardText.HorizontalAlignment = HorizontalAlignment.Left;
            awardText.TextHorizontalAlignment = TextHorizontalAlignment.Centre;
            awardText.Colour = Color.White;
            awardText.Position = new Vector2(270, 20);
        }

        private void setFunctionalSettings()
        {
            //actionSelected[3] = "Attack";
            actionSelected[2] = "Attack";
            actionSelected[1] = "Block";
            actionSelected[0] = "Items";

            //gameData.Tezej.isInBattle = true;
            gameData.Tezej.isInBattle = true;
            tezejChar.changeCharacterOrientation("down", "right");
            tezejChar.Strength = gameData.Tezej.strength+11111;
            tezejChar.Agility = gameData.Tezej.agility;
            tezejChar.Intelligence = gameData.Tezej.intelligence;
            tezejChar.Vitality = gameData.Tezej.vitality;
            tezejChar.setStats();

            enemyChar.changeCharacterOrientation("down", "left");
            enemyChar.Strength = enemy.strength;
            enemyChar.Agility = enemy.agility;
            enemyChar.Intelligence = enemy.intelligence;
            enemyChar.Vitality = enemy.vitality;
            enemyChar.setStats();

            gameData.Inventory.setCharacter(tezejChar);
            
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
                //txtEleRectLeftBar.Add(MagicTxtElement);
                txtEleRectLeftBar.Add(ItemsTxtElement);
            }
            else
            {
                txtEleRectLeftBar.Remove(AttackTxtElement);
                txtEleRectLeftBar.Remove(BlockTxtElement);
                //txtEleRectLeftBar.Remove(MagicTxtElement);
                txtEleRectLeftBar.Remove(ItemsTxtElement);
            }
        }

        private void changeStatusBars()
        {
            double timeSize = 0;

            double hpSize = (tezejChar.Health * 105) / tezejChar.maxHealth;
            barArray[0] = new TexturedElement(new Vector2(Int32.Parse(hpSize.ToString()), 4));
            barArray[0].Texture = hpBar;
            barArray[0].Position = new Vector2(320, 141);

            double mpSize = (tezejChar.Mana * 107) / tezejChar.maxMana;
            barArray[1] = new TexturedElement(new Vector2(Int32.Parse(mpSize.ToString()), 4));
            barArray[1].Texture = mpBar;
            barArray[1].Position = new Vector2(470, 141);

            if (!canDoAction)
            {
                timeSize = Math.Round((ActionTime * 100) / 5);
                barArray[2] = new TexturedElement(new Vector2(Int32.Parse(timeSize.ToString()), 15));
                barArray[2].Texture = timeBar;
                barArray[2].Position = new Vector2(650, 145);
            }

            if (timeSize == 100 || timeSize > 100)
            {
                isCharSelected = true;
                setActionBox(true);
                canDoAction = true;
            }
        }

        private void actionDone(String action)
        {
            ActionTime = 0.0f;
            canDoAction = false;
        }

        private Int32 getAttackDamage(Razredi.Character prviChar, Razredi.Character drugiChar)
        {
            Int32 result = 0;
            int atk = prviChar.Strength;
            int def = drugiChar.Strength;

            if (drugiChar == tezejChar && tezejChar.isBlocking)
            {
                def = drugiChar.Strength * 5;
            }

            Random random = new Random();

            if(def>atk)
                result = random.Next(10, 20);
            else
                result = random.Next(def, atk);

            TestDamage = result;
            return result;
        }

        private void awardGenerator()
        {
            if (expGained == 0)
            {
                Random rndm = new Random();
                String addText = "";

                expGained = 500;
                gameData.Tezej.expPoints += expGained;

                hpPotionsGained = rndm.Next(0, 2);
                for (int i = 0; i < hpPotionsGained; i++)
                {
                    addText += hpPotionsGained.ToString()+"x Health potions gained\n";
                    gameData.Inventory.addPotion("hp", 20);
                }

                mpPotionsGained = rndm.Next(0, 2);
                for (int i = 0; i < mpPotionsGained; i++)
                {
                    addText += mpPotionsGained+"x Mana potions gained\n";
                    gameData.Inventory.addPotion("mp", 20);
                }

                gameData.Tezej.healthPoints = tezejChar.Health;
                gameData.Tezej.manaPoints = tezejChar.Mana;

                awardText.Text.AppendLine("Experience gained : " + expGained.ToString());
                awardText.Text.AppendLine("\n"+addText);
            }
        }

        private Boolean checkIfcanAttack(float time, float action_Time)
        {
            Boolean result = false;

            if (Math.Round(time) > action_Time - 3.0f || Math.Round(time) > action_Time - 4.0f || Math.Round(time) > action_Time - 5.0f)
            {
                result = true;
            }

            return result;
        }
    }
}
