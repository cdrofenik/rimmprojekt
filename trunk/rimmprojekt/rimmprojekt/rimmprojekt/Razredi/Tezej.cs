﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

using Xen;
using Xen.Camera;
using Xen.Graphics;
using Xen.Ex.Graphics;
using Xen.Ex.Graphics2D;
using Xen.Ex.Geometry;
using Xen.Ex.Graphics.Content;
using Xen.Ex.Material;

using JigLibX.Math;
using JigLibX.Physics;
using JigLibX.Geometry;
using JigLibX.Collision;

using Microsoft.Xna.Framework.Content;

namespace rimmprojekt.Razredi
{
    class Tezej : IDraw, IContentOwner, IUpdate
    {
        public Boolean isInBattle;

        #region parameters
        private Boolean hasPlayed;

        #region stats
        private Int32 lvlUpMaxPoints = 5;
        private Int32 pointsCounter = 0;
        private Int32 maxExp = 404;
        public Int32 healthPoints;
        public Int32 maxHealthPoints = 361;
        public Int32 maxManaPoints = 361;
        public Int32 manaPoints;
        public Int32 expPoints;
        public Int32 damage;
        public Int32 durability;
        public Int32 luck;
        private Boolean hasLeveledUp = false;
        #endregion

        List<Body> bodies;
        private CollisionSystem collisionSystem;
        private Body body;
        private CollisionSkin skin;
        public List<CollisionInfo> collisions;
        private BasicCollisionFunctor collisionFunctor;

        #region drawing elements
        //avatar,leveling and status parameters

        public Boolean senseCollision;


        //leveling
        private TextElementRect levelUpText;
        private TextElementRect durabilityBarText;
        private TextElementRect luckBarText;
        private SpriteFont trueFont;
        private Texture2D tezejLevelUpTexture;
        private Texture2D tezejLevelUpButtonTexture;
        private TexturedElement lvlUpElement;
        private TexturedElement lvlUpFirstButtonElement;
        private TexturedElement lvlUpSecondButtonElement;
        private TexturedElement lvlUpThirdButtonElement;
        #endregion

        //audio
        SoundEffect levelUpSoundEffect;

        //displaying properties
        public Vector3 polozaj;
        private Matrix matrix;
        private IShader shader;
        private ModelInstance model;
        private ModelInstance sword;

        #region kontrole modelov in animacij
        private AnimationController animationController;
        private AnimationInstance animation;
        private string orientiranModel;
        private Boolean isRunning;
        private Boolean isIdle;
        public Boolean isAttacking;
        public Boolean isBlocking;
        #endregion

        #endregion

        public Tezej(float x, float y, float z, UpdateManager manager, ContentRegister content, List<Body> bodies)
        {
            model = new ModelInstance();
            sword = new ModelInstance();

            manager.Add(this);
            content.Add(this);

            #region animacije
            senseCollision = true;
            isInBattle = false;
            isBlocking = false;
            isAttacking = false;
            isRunning = false;
            isIdle = true;
            orientiranModel = "down";

            animationController = model.GetAnimationController();
            animation = animationController.PlayLoopingAnimation(3);

            int boneR = model.ModelData.Skeleton.GetBoneIndexByName("helperR");
            int boneL = model.ModelData.Skeleton.GetBoneIndexByName("helperL");
            //ModelBone bone = new ModelBone();
            //bone = model.ModelData.Skeleton.BoneData.ElementAt(boneR)

            #endregion

            #region parametri
            hasPlayed = false;

            healthPoints = 361;
            manaPoints = 361;
            expPoints = 0;

            damage = 1;
            durability = 1;
            luck = 1;
            #endregion

            #region leveling up elements
            //inicializacija leveling elementa
            lvlUpFirstButtonElement = new TexturedElement(tezejLevelUpButtonTexture, new Vector2(98, 37));
            lvlUpFirstButtonElement.Position = new Vector2(280, 214);        // pozicija damage bara

            lvlUpSecondButtonElement = new TexturedElement(tezejLevelUpButtonTexture, new Vector2(98, 37));
            lvlUpSecondButtonElement.Position = new Vector2(280, 167);        // pozicija durability bara

            lvlUpThirdButtonElement = new TexturedElement(tezejLevelUpButtonTexture, new Vector2(98, 37));
            lvlUpThirdButtonElement.Position = new Vector2(280, 117);        // pozicija luck bara

            lvlUpElement = new TexturedElement(tezejLevelUpTexture, new Vector2(403, 401));
            lvlUpElement.Position = new Vector2(400, 220);
            lvlUpElement.Add(lvlUpFirstButtonElement);
            lvlUpElement.Add(lvlUpSecondButtonElement);
            lvlUpElement.Add(lvlUpThirdButtonElement);
            setCharacterStatusValuseOnLevelUp();
            #endregion

            #region collision
            polozaj = new Vector3(x, y, z);
            matrix = Matrix.CreateTranslation(polozaj);
            MaterialShader material = new MaterialShader();
            material.SpecularColour = Color.LightYellow.ToVector3();//with a nice sheen

            Vector3 lightDirection = new Vector3(0.5f, 1, -0.5f); //a less dramatic direction

            MaterialLightCollection lights = new MaterialLightCollection();
            lights.AmbientLightColour = Color.White.ToVector3() * 0.5f;
            //lights.CreateDirectionalLight(lightDirection, Color.Red);
            material.LightCollection = lights;
            shader = material;

            collisionSystem = new CollisionSystemSAP();
            collisionSystem.UseSweepTests = false;

            collisions = new List<CollisionInfo>();
            collisionFunctor = new BasicCollisionFunctor(collisions);

            this.bodies = new List<Body>(bodies);
            foreach (Body b in bodies)
                collisionSystem.AddCollisionSkin(b.CollisionSkin);

            body = new Body();
            skin = new CollisionSkin(body);
            body.CollisionSkin = skin;

            Box box = new Box(Vector3.Zero, Matrix.Identity, new Vector3(5.0f, 5.0f, 5.0f));
            skin.AddPrimitive(box, new MaterialProperties(0.5f, 0.5f, 0.5f));

            body.MoveTo(polozaj + new Vector3(15.5f, 15.5f, 15.5f), Matrix.Identity);
            //skin.ApplyLocalTransform(new JigLibX.Math.Transform(polozaj, Matrix.Identity));
            body.EnableBody();

            bodies.Add(body);
            collisionSystem.AddCollisionSkin(body.CollisionSkin);

            #endregion

            content.Add(this);
        }

        public void Draw(DrawState state)
        {
            using (state.WorldMatrix.PushMultiply(ref matrix))
            {
                if (CullTest(state))
                {
                    using (state.Shader.Push(shader))
                    {
                        sword.Draw(state);
                        model.Draw(state);
                    }
                }
            }

            using (state.Shader.Push())
            {
                if (CullTest(state))
                {
                    if (hasLeveledUp)
                    {
                        lvlUpElement.Remove(levelUpText);
                        setCharacterStatusValuseOnLevelUp();
                        lvlUpElement.Draw(state);
                    }
                }
            }
        }

        public bool CullTest(ICuller culler)
        {
            return true;
        }

        public void LoadContent(ContentState state)
        {
            //load the model data into the model instance
            model.ModelData = state.Load<Xen.Ex.Graphics.Content.ModelData>(@"Models/player");
            sword.ModelData = state.Load<Xen.Ex.Graphics.Content.ModelData>(@"Models/epic_sword");
            tezejLevelUpTexture = state.Load<Texture2D>(@"Tezej/LvlUpTexture");
            tezejLevelUpButtonTexture = state.Load<Texture2D>(@"Tezej/lvlUpButton");
            levelUpSoundEffect = state.Load<SoundEffect>(@"Tezej/lvlupEffect");
            trueFont = state.Load<SpriteFont>("Arial");
        }

        public UpdateFrequency Update(UpdateState state)
        {
            collisions.Clear();

            Vector3 premik = new Vector3(0f, 0f, 0f);

            if (hasLeveledUp)
            {
                #region level up
                if (!hasPlayed)
                {
                    levelUpSoundEffect.Play();
                    hasPlayed = true;
                }

                if (state.KeyboardState.KeyState.B.OnPressed)
                {
                    damage += 1;
                    pointsCounter++;
                }
                if (state.KeyboardState.KeyState.N.OnPressed)
                {
                    durability += 1;
                    pointsCounter++;
                }
                if (state.KeyboardState.KeyState.M.OnPressed)
                {
                    luck += 1;
                    pointsCounter++;
                }

                if (pointsCounter == lvlUpMaxPoints)
                {
                    expPoints = 0;
                    hasLeveledUp = false;
                    pointsCounter = 0;
                }
                #endregion
            }
            else
            {
                hasPlayed = false;
                if (!isInBattle)
                {
                    #region keyboard input for free roaming
                    if (state.KeyboardState.KeyState.S.IsDown)
                    {
                        isHeRunning();
                        changeAngele(orientiranModel, "down");
                        premik.Z += 1.0f;
                    }
                    if (state.KeyboardState.KeyState.W.IsDown)
                    {
                        isHeRunning();
                        changeAngele(orientiranModel, "up");
                        premik.Z -= 1.0f;
                    }
                    if (state.KeyboardState.KeyState.D.IsDown)
                    {
                        isHeRunning();
                        changeAngele(orientiranModel, "right");
                        premik.X += 1.0f;
                    }
                    if (state.KeyboardState.KeyState.A.IsDown)
                    {
                        isHeRunning();
                        changeAngele(orientiranModel, "left");
                        premik.X -= 1.0f;
                    }
                    #endregion
                }

                checkKeysPressed(state);

                if (state.KeyboardState.KeyState.D1.IsDown)
                    healthPoints -= 20;
                if (state.KeyboardState.KeyState.D2.IsDown)
                    manaPoints -= 20;
                if (state.KeyboardState.KeyState.E.IsDown)
                    expPoints += 20;
                if ((expPoints == maxExp) || (expPoints > maxExp))
                    hasLeveledUp = true;

                polozaj += premik;
                body.MoveTo(polozaj + new Vector3(7.5f, 7.5f, 7.5f), Matrix.Identity);
                if (senseCollision)
                {
                    collisionSystem.DetectCollisions(body, collisionFunctor, null, 0.05f);
                    if (collisions.Count > 0)
                        polozaj -= premik;
                }
                //matrix = Matrix.CreateTranslation(polozaj);
                //collisionSystem.DetectAllCollisions(this.bodies, collisionFunctor, null, 0.05f);
            }
            //matrix=Matrix.CreateScale(0.04f,0.04f,0.04f) *
            //    skin.GetPrimitiveLocal(0).Transform.Orientation *
            //    body.Orientation *
            //    Matrix.CreateTranslation(body.Position);
            return UpdateFrequency.FullUpdate60hz;
        }

        private void setCharacterStatusValuseOnLevelUp()
        {
            levelUpText = new TextElementRect(new Vector2(40, 40));
            levelUpText.Position = new Vector2(134, 20);
            levelUpText.Font = trueFont;
            levelUpText.Colour = Color.DarkBlue;
            levelUpText.Text.AppendLine(damage.ToString());
            levelUpText.Text.AppendLine();
            levelUpText.Text.AppendLine();
            levelUpText.Text.AppendLine(durability.ToString());
            levelUpText.Text.AppendLine();
            levelUpText.Text.AppendLine();
            levelUpText.Text.AppendLine(luck.ToString());
            levelUpText.VerticalAlignment = VerticalAlignment.Centre;
            levelUpText.HorizontalAlignment = HorizontalAlignment.Centre;
            levelUpText.TextHorizontalAlignment = TextHorizontalAlignment.Centre;

            lvlUpElement.Add(levelUpText);
        }

        public void changeAngele(string prvotnaSmer, string zeljenaSmer)
        {
            if (prvotnaSmer.Equals(zeljenaSmer))
            {
                matrix = Matrix.CreateTranslation(polozaj);
            }
            else
            {
                if (prvotnaSmer.Equals("down") && zeljenaSmer.Equals("up"))
                {
                    matrix = Matrix.CreateRotationZ((float)Math.PI) * Matrix.CreateRotationX((float)(Math.PI));
                    matrix *= Matrix.CreateTranslation(polozaj);
                }
                if (prvotnaSmer.Equals("down") && zeljenaSmer.Equals("left"))
                {
                    matrix = Matrix.CreateRotationZ((float)(Math.PI / 2)) * Matrix.CreateRotationX((float)(Math.PI / 2)) * Matrix.CreateRotationZ((float)(-Math.PI / 2));
                    matrix *= Matrix.CreateTranslation(polozaj);
                }
                if (prvotnaSmer.Equals("down") && zeljenaSmer.Equals("right"))
                {
                    matrix = Matrix.CreateRotationZ((float)(-Math.PI / 2)) * Matrix.CreateRotationX((float)(Math.PI / 2)) * Matrix.CreateRotationZ((float)(Math.PI / 2));
                    matrix *= Matrix.CreateTranslation(polozaj);
                }
            }
        }

        private void checkKeysPressed(UpdateState state)
        {
            if (!state.KeyboardState.KeyState.S.IsDown && !state.KeyboardState.KeyState.W.IsDown && !state.KeyboardState.KeyState.D.IsDown
                && !state.KeyboardState.KeyState.A.IsDown)
            {
                isHeIdle();
            }
        }

        private void isHeRunning()
        {
            if (!isRunning)
            {
                animation.StopAnimation();
                animation = animationController.PlayLoopingAnimation(0);
                isRunning = true;
                isIdle = false;
                isAttacking = false;
            }
        }

        private void isHeIdle()
        {
            if (!isIdle)
            {
                animation.StopAnimation();
                animation = animationController.PlayLoopingAnimation(3);
                isIdle = true;
                isRunning = false;
                isAttacking = false;
                isBlocking = false;
            }
        }

        private void heIsDead()
        {
            animation.StopAnimation();
            animation = animationController.PlayLoopingAnimation(12);
        }

        private void isHeFighting(UpdateState state)
        {
            if (state.MouseState.LeftButton.DownDuration > 0.9)
            {
                animation.StopAnimation();
                animation = animationController.PlayAnimation(3);
                isIdle = false;
                isRunning = false;
                isAttacking = true;
                isBlocking = false;
            }
            else
            {
                if (!isAttacking)
                {
                    animation.StopAnimation();
                    animation = animationController.PlayAnimation(4);
                    isIdle = false;
                    isRunning = false;
                    isAttacking = true;
                    isBlocking = false;
                }
            }
        }

        private void isHeBlocking(UpdateState state)
        {
            if (state.MouseState.RightButton.DownDuration > 0.1)
            {
                animation.StopAnimation();
                animation = animationController.PlayAnimation(8);
                isIdle = false;
                isRunning = false;
                isAttacking = false;
                isBlocking = true;
            }
        }
    }
}
