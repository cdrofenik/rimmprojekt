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
    class Enemy : IDraw, IContentOwner
    {
        public Boolean isInBattle;

        #region parameters

        #region stats
        public Int32 maxHealthPoints;     //zacetni maksimalni hp
        public Int32 maxManaPoints;       //zacetni maksimalni mp

        public Int32 healthPoints;
        public Int32 manaPoints;
        public Int32 expPoints;
        public Int32 strength;
        public Int32 agility;
        public Int32 intelligence;
        public Int32 vitality;
        #endregion

        #region collision
        List<Body> bodies;
        private CollisionSystem collisionSystem;
        private Body body;
        private CollisionSkin skin;
        public List<CollisionInfo> collisions;
        private BasicCollisionFunctor collisionFunctor;
        #endregion

        #region AI
        private String orientiranModel = "down";
        private Boolean moveUpDown;
        private Boolean moveLeftRight;
        private Int32 movementCounter;
        #endregion

        private Boolean skinGoblin;
        public String skin_value;
        public Boolean hasHitTezej;

        //displaying properties
        public Vector3 polozaj;
        private Matrix matrix;
        private IShader shader;
        private ModelInstance model;

        #region kontrole modelov in animacij
        private AnimationController animationController;
        private AnimationInstance animation;
        private Boolean isIdle;
        #endregion

        #endregion

        #region get/set metode
        public Body Body
        {
            get { return body; }
            set { body = value; }
        }
        #endregion

        public Enemy(float x, float y, float z, ContentRegister content, String skinChosen)
        {
            skinGoblin = true;
            skin_value = "";

            if (skinChosen.Equals(skin_value))
                skinGoblin = false;

            setStatsAs(skinChosen);

            model = new ModelInstance();
            animationController = model.GetAnimationController();
            
            content.Add(this);

            #region animacije
            isInBattle = false;
            isIdle = true;
            hasHitTezej = false;

            animation = animationController.PlayLoopingAnimation(3);

            #endregion

            #region zacetni statsi

            maxHealthPoints = 20 * vitality;
            healthPoints = maxHealthPoints;

            maxManaPoints = 10 * intelligence;
            manaPoints = maxManaPoints;

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
                        if(!hasHitTezej)
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
            //load the model data into the model instance
            if (skinGoblin)
                model.ModelData = state.Load<Xen.Ex.Graphics.Content.ModelData>(@"Models/goblin");
            else
                model.ModelData = state.Load<Xen.Ex.Graphics.Content.ModelData>(@"Models/minotaur");
        }

        public void Update(UpdateState state)
        {
            //collisions.Clear();

            Vector3 premik = new Vector3(0f, 0f, 0f);

            isHeIdle();

            //polozaj += premik;
            //body.MoveTo(polozaj + new Vector3(7.5f, 7.5f, 7.5f), Matrix.Identity);
            //if (!hasHitTezej)
            //    collisionSystem.DetectCollisions(body, collisionFunctor, null, 0.05f);

            //if (collisions.Count > 0)
            //{
            //    polozaj -= premik;
            //    hasHitTezej = true;
            //}
            //matrix = Matrix.CreateTranslation(polozaj);
            //collisionSystem.DetectAllCollisions(this.bodies, collisionFunctor, null, 0.05f);
            //matrix=Matrix.CreateScale(0.04f,0.04f,0.04f) *
            //    skin.GetPrimitiveLocal(0).Transform.Orientation *
            //    body.Orientation *
            //    Matrix.CreateTranslation(body.Position);
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

        private void isHeIdle()
        {
            if (!isIdle)
            {
                animation.StopAnimation();
                animation = animationController.PlayLoopingAnimation(3);
                isIdle = true;
            }
        }

        private void setStatsAs(String enemy)
        {
            if (enemy.Equals(skin_value))
            {
                strength = 40;
                agility = 12;
                intelligence = 0;
                vitality = 20;
            }
            else
            {
                strength = 13;
                agility = 12;
                intelligence = 1;
                vitality = 6;
            }
        }
    }
}