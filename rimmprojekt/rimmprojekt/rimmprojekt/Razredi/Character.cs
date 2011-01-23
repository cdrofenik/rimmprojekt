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

//using Microsoft.Xna.Framework.Content;

namespace rimmprojekt.Razredi
{
    class Character : IDraw, IContentOwner, IUpdate
    {
        public Boolean isHitting;
        public Boolean isTakingDamage;

        private Boolean firstTimeAnimationSet;
        private Boolean thisModelIsTezej;
        public Boolean isAttacking;
        public Boolean isBlocking;
        
        private Vector2 enemyLocation;
        private Boolean isAI;

        #region kontrole modelov in animacij
        private AnimationController animationController;
        private AnimationInstance animation;
        private string orientiranModel;
        private Boolean isRunning;
        private float PlayingTime;
        private float actionTime;
        private float actionTimeSecond;
        private Vector3 startingPosition;
        #endregion

        //displaying properties
        public Vector3 polozaj;
        private Matrix matrix;
        private IShader shader;
        private ModelInstance model;
        private ModelInstance sword;

        public Character(float x, float y, float z, UpdateManager manager, ContentRegister content, String character)
        {
            model = new ModelInstance();
            sword = new ModelInstance();

            if (character.Equals("tezej"))
            {
                thisModelIsTezej = true;
                isAI = false;
            }
            else
            {
                isAI = true;
            }   

            manager.Add(this);
            content.Add(this);

            orientiranModel = "down";
            actionTime = 0.0f;
            actionTimeSecond = 999.0f;
            startingPosition = new Vector3(x, y, z);
            isAttacking = false;
            isBlocking = false;
            isRunning = false;
            firstTimeAnimationSet = false;
            enemyLocation = new Vector2();

            animationController = model.GetAnimationController();
            animation = animationController.PlayLoopingAnimation(3);

            polozaj = new Vector3(x, y, z);
            matrix = Matrix.CreateTranslation(polozaj);
            MaterialShader material = new MaterialShader();
            material.SpecularColour = Color.LightYellow.ToVector3();//with a nice sheen
            Vector3 lightDirection = new Vector3(0.5f, 1, -0.5f); //a less dramatic direction
            MaterialLightCollection lights = new MaterialLightCollection();
            lights.AmbientLightColour = Color.White.ToVector3() * 0.5f;
            lights.CreateDirectionalLight(lightDirection, Color.Red);
            material.LightCollection = lights;
            shader = material;

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
                        //sword.Draw(state);
                        model.Draw(state);
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
            if(thisModelIsTezej)
                model.ModelData = state.Load<Xen.Ex.Graphics.Content.ModelData>(@"Models/player");
            else
                model.ModelData = state.Load<Xen.Ex.Graphics.Content.ModelData>(@"Models/minotaur");
        }

        public UpdateFrequency Update(UpdateState state)
        {
            Vector3 premik = new Vector3(0f, 0f, 0f);
            PlayingTime += state.DeltaTimeSeconds;

            if (!isAI)
            {
                #region Attack
                if (isAttacking)
                {
                    if ((polozaj.X != enemyLocation.X) && (polozaj.X < enemyLocation.X))
                    {
                        runToEnemy();
                        changeCharacterOrientation("down", orientiranModel);
                        premik.X += 1.0f;
                        isHitting = true;
                    }
                    else
                    {
                        hitEnemy(state);
                        if (PlayingTime > actionTime)
                        {
                            returnToPosition();
                            isAttacking = false;
                            isRunning = false;
                            isHitting = false;
                        }
                    }
                }
                #endregion

                #region Block
                if (isBlocking)
                {
                    setToBlock();
                    if (PlayingTime > actionTime)
                    {
                        animation.StopAnimation();
                        animation = animationController.PlayLoopingAnimation(3);
                        isBlocking = false;
                    }
                }
                #endregion

                #region Damaging
                if (isTakingDamage)
                {
                    takeDamage();
                }
                #endregion
            }
            else if (isAI)
            {
                #region Attack
                if (isAttacking)
                {
                    if ((polozaj.X != enemyLocation.X) && (polozaj.X > enemyLocation.X))
                    {
                        runToEnemy();
                        changeCharacterOrientation("down", "left");
                        premik.X -= 1.0f;
                        isHitting = true;
                    }
                    else
                    {
                        hitEnemy(state);
                        if (PlayingTime > actionTime)
                        {
                            returnToPosition();
                            isAttacking = false;
                            isRunning = false;
                            isHitting = false;
                        }
                    }
                }
                #endregion

                #region Damaging
                if (isTakingDamage)
                {
                    takeDamage();
                }
                #endregion
            }

            polozaj += premik;
            return UpdateFrequency.FullUpdate60hz;
        }

        public void changeCharacterOrientation(string prvotnaSmer, string zeljenaSmer)
        {
            orientiranModel = zeljenaSmer;

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

        #region Attacking
        public void goAttackEnemy(float x, float y)
        {
            isAttacking = true;
            if(!isAI)
                enemyLocation.X = x - 13;
            else
                enemyLocation.X = x + 13;

            enemyLocation.Y = y;
        }

        private void runToEnemy()
        {
            if (!isRunning)
            {
                animation.StopAnimation();
                animation = animationController.PlayLoopingAnimation(0);
                isRunning = true;
            }
        }

        private void hitEnemy(UpdateState state)
        {
            if (isHitting)
            {
                actionTime = PlayingTime + 0.6f;
                animation.StopAnimation();
                animation = animationController.PlayAnimation(4);
                isHitting = false;
            }
        }

        private void returnToPosition()
        {
            polozaj = startingPosition;
            changeCharacterOrientation("down", orientiranModel);
            animation.StopAnimation();
            animation = animationController.PlayLoopingAnimation(3);
        }
        #endregion

        #region Blocking
        public void goBlock()
        {
            isBlocking = true;
            firstTimeAnimationSet = false;
        }

        private void setToBlock()
        {
            if (!firstTimeAnimationSet)
            {
                actionTime = PlayingTime + 5.0f;
                firstTimeAnimationSet = true;
                animation.StopAnimation();
                animation = animationController.PlayLoopingAnimation(8);
            }
        }
        #endregion

        #region Take Damage
        private void takeDamage()
        {
            if (!firstTimeAnimationSet)
            {
                actionTime = PlayingTime + 1.3f;        //spremeni za pravi timing
                firstTimeAnimationSet = true;
            }

            if (PlayingTime > actionTime)
            {
                actionTimeSecond = PlayingTime+0.3f;
                animation.StopAnimation();
                animation = animationController.PlayAnimation(10);
                firstTimeAnimationSet = false;
            }

            if (PlayingTime > actionTimeSecond)
            {
                actionTime = 0.0f;
                actionTimeSecond = 999.0f;
                animation.StopAnimation();
                animation = animationController.PlayLoopingAnimation(3);
                isTakingDamage = false;
                firstTimeAnimationSet = false;
            }
        }
        #endregion
    }
}
