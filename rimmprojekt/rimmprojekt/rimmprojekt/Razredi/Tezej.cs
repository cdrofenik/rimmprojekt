using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
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
    public class Tezej : IDraw, IContentOwner
    {
        #region animacije za sablo
        //private Quaternion itemRotateAnim;
        //private Xen.Transform helperTransformStatic, helperTransformAnim;
        //private int boneIndex;
        //private Matrix helperMatrix;
        //private Matrix itemRot;
        #endregion

        public Boolean isInBattle;
        private TextElement debug;

        #region parameters
        private Boolean hasPlayed;

        #region stats
        private Int32 lvlUpMaxPoints = 5;       //tocke za razporeditev skillov
        private Int32 pointsCounter = 0;        //stevec razporejenih tock
        public Int32 maxHealthPoints;     //zacetni maksimalni hp
        public Int32 maxManaPoints;       //zacetni maksimalni mp
        private Int32 maxExp = 400;             //meja za napredovanje levela


        public Int32 healthPoints;
        public Int32 manaPoints;
        public Int32 expPoints;
        public Int32 strength;
        public Int32 agility;
        public Int32 intelligence;
        public Int32 vitality;
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

        //leveling
        private TextElementRect levelUpText;
        private TextElement strengthText;
        private TextElement agilityText;
        private TextElement intelligenceText;
        private TextElement vitalityText;
        private SpriteFont trueFont;
        private Texture2D tezejLevelUpTexture;
        private TexturedElement lvlUpElement;
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

        #region get/set metode
        public CollisionSystem CollisionSystem
        {
            get { return collisionSystem; }
            set { collisionSystem = value; }
        }
        #endregion

        public Tezej()
        {

        }

        public Tezej(float x, float y, float z, UpdateManager manager, ContentRegister content, List<Body> bodies)
        {
            model = new ModelInstance();
            sword = new ModelInstance();

            content.Add(this);

            #region animacije
            isInBattle = false;
            isBlocking = false;
            isAttacking = false;
            isRunning = false;
            isIdle = true;
            orientiranModel = "down";

            animationController = model.GetAnimationController();
            animation = animationController.PlayLoopingAnimation(3);

            #endregion

            #region zacetni statsi
            hasPlayed = false;

            strength = 15;
            agility = 12;
            intelligence = 13;
            vitality = 10;

            maxHealthPoints = 20 * vitality;
            healthPoints = maxHealthPoints;

            maxManaPoints = 10 * intelligence;
            manaPoints = maxManaPoints;
            expPoints = 0;
            #endregion

            #region leveling up elements
            //inicializacija leveling elementa
            lvlUpElement = new TexturedElement(tezejLevelUpTexture, new Vector2(403, 401));
            lvlUpElement.Position = new Vector2(400, 220);


            Color textColor = Color.Black;

            levelUpText = new TextElementRect(new Vector2(150, 300));
            levelUpText.Position = new Vector2(130, 0);
            levelUpText.Font = trueFont;
            levelUpText.Colour = Color.Black;
            levelUpText.AlphaBlendState = Xen.Graphics.AlphaBlendState.Alpha;
            levelUpText.VerticalAlignment = VerticalAlignment.Centre;
            levelUpText.HorizontalAlignment = HorizontalAlignment.Centre;
            levelUpText.TextHorizontalAlignment = TextHorizontalAlignment.Centre;

            strengthText = new TextElement("Strength");
            strengthText.Font = trueFont;
            strengthText.Colour = textColor;
            strengthText.Position = new Vector2(0, -43);

            agilityText= new TextElement("Agility");
            agilityText.Font = trueFont;
            agilityText.Colour = textColor;
            agilityText.Position = new Vector2(0, -88);

            intelligenceText = new TextElement("Intelligence");
            intelligenceText.Font = trueFont;
            intelligenceText.Colour = textColor;
            intelligenceText.Position = new Vector2(0, -132);

            vitalityText = new TextElement("Vitality");
            vitalityText.Font = trueFont;
            vitalityText.Colour = textColor;
            vitalityText.Position = new Vector2(0, -179);

            levelUpText.Add(strengthText);
            levelUpText.Add(agilityText);
            levelUpText.Add(intelligenceText);
            levelUpText.Add(vitalityText);
            #endregion

            #region collision
            polozaj = new Vector3(x, y, z);
            matrix = Matrix.CreateTranslation(polozaj);

            //helperMatrix = Matrix.CreateTranslation(new Vector3(-40, y, z));

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


            debug = new TextElement("debug");
            debug.Font = trueFont;
            debug.Colour = Color.Yellow;

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
                        model.Draw(state);
                    }
                }
            }

            //using (state.WorldMatrix.PushMultiply(ref helperMatrix))
            //{
            //    if (CullTest(state))
            //    {
            //        using (state.Shader.Push(shader))
            //        {
            //            //updateItemPosition(state, "asdf");
            //            //sword.Draw(state);
            //        }
            //    }
            //}

            if (hasLeveledUp)
            {
                setCharacterStatusValuseOnLevelUp();
                lvlUpElement.Draw(state);
                levelUpText.Draw(state);
            }

            //debug.Draw(state);
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
            levelUpSoundEffect = state.Load<SoundEffect>(@"Tezej/lvlupEffect");
            trueFont = state.Load<SpriteFont>("Arial");
        }

        public void Update(UpdateState state)
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

                if (pointsCounter+1 == lvlUpMaxPoints)
                {
                    expPoints = 0;
                    hasLeveledUp = false;
                    pointsCounter = 0;
                }

                if (hasLeveledUp)
                {
                    if (state.KeyboardState.KeyState.D1.OnPressed)
                    {
                        strength += 1;
                        pointsCounter++;
                    }
                    if (state.KeyboardState.KeyState.D2.OnPressed)
                    {
                        agility += 1;
                        pointsCounter++;
                    }
                    if (state.KeyboardState.KeyState.D3.OnPressed)
                    {
                        intelligence += 1;
                        pointsCounter++;
                    }
                    if (state.KeyboardState.KeyState.D4.OnPressed)
                    {
                        vitality += 1;
                        pointsCounter++;
                    }
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
                        body.Velocity = Vector3.Transform(polozaj, body.Orientation);
                    }
                    #endregion
                }

                checkKeysPressed(state);

                if ((expPoints == maxExp) || (expPoints > maxExp))
                    hasLeveledUp = true;

                polozaj += premik;
                body.MoveTo(polozaj + new Vector3(7.5f, 7.5f, 7.5f), Matrix.Identity);
                collisionSystem.DetectCollisions(body, collisionFunctor, null, 0.05f);
                if (collisions.Count > 0)
                    polozaj -= premik;        
            }
            //matrix = Matrix.CreateTranslation(polozaj);
            //collisionSystem.DetectAllCollisions(this.bodies, collisionFunctor, null, 0.05f);
            //matrix=Matrix.CreateScale(0.04f,0.04f,0.04f) *
            //    skin.GetPrimitiveLocal(0).Transform.Orientation *
            //    body.Orientation *
            //    Matrix.CreateTranslation(body.Position);

            debug.Text.SetText(polozaj.X + " , " + polozaj.Y + " , " + polozaj.Z + "\n"+body.Orientation.ToString());
            //helperMatrix = Matrix.CreateTranslation(polozaj);
        }

        private void setCharacterStatusValuseOnLevelUp()
        {
            strengthText.Text.SetText(strength.ToString());
            agilityText.Text.SetText(agility.ToString());
            intelligenceText.Text.SetText(intelligence.ToString());
            vitalityText.Text.SetText(vitality.ToString());

            maxHealthPoints = vitality * 20;
            healthPoints = maxHealthPoints;

            maxManaPoints = intelligence * 10;
            manaPoints = maxManaPoints;
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

        //mouse input
        #region mouse input
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
        #endregion

        #region risanje sable
        //private void updateItemPosition(DrawState state, String item)
        //{
        //    boneIndex = model.ModelData.Skeleton.GetBoneIndexByName("helperR");

        //    itemRotateAnim = animationController.GetTransformedBones(state)[boneIndex].Rotation;

        //    itemRot = Matrix.Identity;
        //    itemRot *= body.Orientation;

        //    helperTransformAnim = animationController.GetTransformedBones(state)[boneIndex];
        //    helperTransformStatic = model.ModelData.Skeleton.BoneWorldTransforms[boneIndex];
        //    helperTransformStatic.Translation -= model.ModelData.Skeleton.BoneWorldTransforms[0].Translation;

        //    helperTransformStatic *= helperTransformAnim;
        //    helperTransformStatic.GetMatrix(out helperMatrix);
        //    helperMatrix *= body.Orientation;

        //    body.MoveTo(polozaj + new Vector3(3.0f,3.0f,3.0f), Matrix.CreateFromQuaternion(itemRotateAnim)*itemRot);
        //    //helperMatrix *= boxObject.Body.Orientation;
        //    //model.Body.MoveTo(boxObject.Body.Position + helperMatrix.Translation, Matrix.CreateFromQuaternion(itemRotateAnim) * itemRot);
        //}

        #endregion
    }
}
