using System;
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
        Boolean hasPlayed;
        //leveling up info
        private Int32 lvlUpMaxPoints = 5;
        private Int32 pointsCounter = 0;
        private Int32 maxExp = 404;
        public Int32 healthPoints;
        public Int32 manaPoints;
        public Int32 expPoints;
        public Int32 damage;
        public Int32 durability;
        public Int32 luck;
        private Boolean hasLeveledUp = false;

        List<Body> bodies;
        private CollisionSystem collisionSystem;
        private Body body;
        private CollisionSkin skin;
        public List<CollisionInfo> collisions;
        private BasicCollisionFunctor collisionFunctor;

        #region drawing elements
        //avatar,leveling and status parameters
        private TextElementRect damageBarText;
        private TextElementRect durabilityBarText;
        private TextElementRect luckBarText;
        private SpriteFont trueFont;
        private Texture2D sideBarTexture;
        private Texture2D tezejHpTexture;
        private Texture2D tezejMpTexture;
        private Texture2D tezejExpTexture;
        private Texture2D tezejLevelUpTexture;
        private Texture2D tezejLevelUpButtonTexture;
        private TexturedElement sideElement;
        private TexturedElement hpElement;
        private TexturedElement manaElement;
        private TexturedElement expElement;
        private TexturedElement lvlUpElement;
        private TexturedElement lvlUpFirstButtonElement;
        private TexturedElement lvlUpSecondButtonElement;
        private TexturedElement lvlUpThirdButtonElement;
        private Vector2 sizeOfSideElement;
        #endregion


        //audio
        SoundEffect levelUpSoundEffect;

        //displaying properties

        public Vector3 polozaj;
        private Matrix matrix;
        private IShader shader;
        private ModelInstance model;

        #region kontrole modelov in animacij
        private AnimationController animationController;
        private AnimationInstance animation;
        private string orientiranModel;
        private Boolean isRunning;
        private Boolean isIdle;
        #endregion

        public Tezej(float x, float y, float z, UpdateManager manager, ContentRegister content, List<Body> bodies)
        {
            isRunning = false;
            isIdle = true;
            orientiranModel = "down";
            manager.Add(this);
            model = new ModelInstance();
            animationController = model.GetAnimationController();

            content.Add(this);

            animation = animationController.PlayLoopingAnimation(1);

            hasPlayed = false;

            healthPoints = 361;
            manaPoints = 361;
            expPoints = 0;

            damage = 1;
            durability = 1;
            luck = 1;

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

            #region side elements
            //inicializacija stranskega elementa
            sizeOfSideElement = new Vector2(649, 160);
            sideElement = new TexturedElement(sideBarTexture, sizeOfSideElement);
            sideElement.AlphaBlendState = Xen.Graphics.AlphaBlendState.Alpha;
            setDisplayBars();                 //narise hp in mana bars
            #endregion

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
            content.Add(this);
        }

        public void Draw(DrawState state)
        {
            using (state.WorldMatrix.PushMultiply(ref matrix))
            {
                //cull test the custom geometry
                if (CullTest(state))
                {
                    //bind the shader
                    using (state.Shader.Push(shader))
                    {
                        //draw the custom geometry
                        model.Draw(state);
                    }
                }
            }

            using (state.Shader.Push())
            {
                if (CullTest(state))
                {
                    sideElement.Remove(manaElement);
                    sideElement.Remove(hpElement);
                    sideElement.Remove(expElement);
                    setDisplayBars();

                    sideElement.Draw(state);
                    if (hasLeveledUp)
                    {
                        lvlUpElement.Remove(damageBarText);
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
            sideBarTexture = state.Load<Texture2D>(@"Tezej/tezejus");
            tezejHpTexture = state.Load<Texture2D>(@"Tezej/hpbar");
            tezejMpTexture = state.Load<Texture2D>(@"Tezej/manabar");
            tezejExpTexture = state.Load<Texture2D>(@"Tezej/expbar");
            tezejLevelUpTexture = state.Load<Texture2D>(@"Tezej/LvlUpTexture");
            tezejLevelUpButtonTexture = state.Load<Texture2D>(@"Tezej/lvlUpButton");
            levelUpSoundEffect = state.Load<SoundEffect>(@"Tezej/lvlupEffect");
            trueFont = state.Load<SpriteFont>("Arial");
        }

        public UpdateFrequency Update(UpdateState state)
        {
            collisions.Clear();

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
                    sideElement.Remove(expElement);
                }
                #endregion
            }
            else
            {
                hasPlayed = false;
                Vector3 premik = new Vector3(0f, 0f, 0f);
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
                checkKeysPressed(state);

                if (state.KeyboardState.KeyState.H.IsDown)
                    healthPoints -= 20;
                if (state.KeyboardState.KeyState.M.IsDown)
                    manaPoints -= 20;
                if (state.KeyboardState.KeyState.E.IsDown)
                    expPoints += 20;
                if ((expPoints == maxExp) || (expPoints > maxExp))
                    hasLeveledUp = true;

                polozaj += premik;
                body.MoveTo(polozaj + new Vector3(7.5f, 7.5f, 7.5f), Matrix.Identity);
                collisionSystem.DetectCollisions(body, collisionFunctor, null, 0.05f);
                if (collisions.Count > 0)
                    polozaj -= premik;
                //matrix = Matrix.CreateTranslation(polozaj);
                //collisionSystem.DetectAllCollisions(this.bodies, collisionFunctor, null, 0.05f);
            }
            //matrix=Matrix.CreateScale(0.04f,0.04f,0.04f) *
            //    skin.GetPrimitiveLocal(0).Transform.Orientation *
            //    body.Orientation *
            //    Matrix.CreateTranslation(body.Position);
            return UpdateFrequency.FullUpdate60hz;
        }

        private Vector3 SetMass(float mass)
        {
            PrimitiveProperties primitiveProperties = new PrimitiveProperties(
                PrimitiveProperties.MassDistributionEnum.Solid,
                PrimitiveProperties.MassTypeEnum.Mass, mass);

            float junk;
            Vector3 com;
            Matrix it;
            Matrix itCoM;

            skin.GetMassProperties(primitiveProperties, out junk, out com, out it, out itCoM);

            body.BodyInertia = itCoM;
            body.Mass = junk;

            return com;
        }

        private void setDisplayBars()
        {
            //risanje hpja
            this.hpElement = new TexturedElement(tezejHpTexture, new Vector2(healthPoints, 15));
            this.hpElement.Position = new Vector2(170, 106);
            this.sideElement.Add(hpElement);

            //risanje mane
            this.manaElement = new TexturedElement(tezejMpTexture, new Vector2(manaPoints, 15));
            this.manaElement.Position = new Vector2(170, 62);
            this.sideElement.Add(manaElement);

            //risanje exp
            this.expElement = new TexturedElement(tezejExpTexture, new Vector2(expPoints, 7));
            this.expElement.Position = new Vector2(67, 17);
            this.sideElement.Add(expElement);
        }

        private void setCharacterStatusValuseOnLevelUp()
        {
            damageBarText = new TextElementRect(new Vector2(40, 40));
            damageBarText.Position = new Vector2(134, 20);
            damageBarText.Font = trueFont;
            damageBarText.Colour = Color.DarkBlue;
            damageBarText.Text.AppendLine(damage.ToString());
            damageBarText.Text.AppendLine();
            damageBarText.Text.AppendLine();
            damageBarText.Text.AppendLine(durability.ToString());
            damageBarText.Text.AppendLine();
            damageBarText.Text.AppendLine();
            damageBarText.Text.AppendLine(luck.ToString());
            damageBarText.VerticalAlignment = VerticalAlignment.Centre;
            damageBarText.HorizontalAlignment = HorizontalAlignment.Centre;
            damageBarText.TextHorizontalAlignment = TextHorizontalAlignment.Centre;

            lvlUpElement.Add(damageBarText);
        }

        private void changeAngele(string prvotnaSmer, string zeljenaSmer)
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

        private void isHeRunning()
        {
            if (!isRunning)
            {
                animation.StopAnimation();
                animation = animationController.PlayLoopingAnimation(0);
                isRunning = true;
                isIdle = false;
            }
        }

        private void isHeIdle()
        {
            if (!isIdle)
            {
                animation.StopAnimation();
                animation = animationController.PlayLoopingAnimation(1);
                isIdle = true;
                isRunning = false;
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

        private void heIsDead()
        {
            animation.StopAnimation();
            animation = animationController.PlayLoopingAnimation(12);
        }
    }
}
