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
    class Minotaver : IDraw, IContentOwner, IUpdate
    {
        public Int32 healthPoints;
        public Int32 damage;
        public Int32 durability;

        List<Body> bodies;
        private CollisionSystem collisionSystem;
        private Body body;
        private CollisionSkin skin;
        public List<CollisionInfo> collisions;
        private BasicCollisionFunctor collisionFunctor;

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

        public Minotaver(float x, float y, float z, UpdateManager manager, ContentRegister content, List<Body> bodies)
        {
            isRunning = false;
            isIdle = true;
            orientiranModel = "down";
            manager.Add(this);
            model = new ModelInstance();
            animationController = model.GetAnimationController();

            content.Add(this);

            animation = animationController.PlayLoopingAnimation(1);

            healthPoints = 361;
            damage = 1;
            durability = 1;

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
        }

        public bool CullTest(ICuller culler)
        {
            return true;
        }

        public void LoadContent(ContentState state)
        {
            //load the model data into the model instance
            model.ModelData = state.Load<Xen.Ex.Graphics.Content.ModelData>(@"Models/minotaur");
        }

        public UpdateFrequency Update(UpdateState state)
        {
            collisions.Clear();

            Vector3 premik = new Vector3(0f, 0f, 0f);
            if (state.KeyboardState.KeyState.Down.IsDown)
            {
                isHeRunning();
                changeAngele(orientiranModel, "down");
                premik.Z += 1.0f;
            }
            if (state.KeyboardState.KeyState.Up.IsDown)
            {
                isHeRunning();
                changeAngele(orientiranModel, "up");
                premik.Z -= 1.0f;
            }
            if (state.KeyboardState.KeyState.Right.IsDown)
            {
                isHeRunning();
                changeAngele(orientiranModel, "right");
                premik.X += 1.0f;
            }
            if (state.KeyboardState.KeyState.Left.IsDown)
            {
                isHeRunning();
                changeAngele(orientiranModel, "left");
                premik.X -= 1.0f;
            }
            checkKeysPressed(state);

            polozaj += premik;
            body.MoveTo(polozaj + new Vector3(7.5f, 7.5f, 7.5f), Matrix.Identity);
            collisionSystem.DetectCollisions(body, collisionFunctor, null, 0.05f);
            if (collisions.Count > 0)
                polozaj -= premik;
            //matrix = Matrix.CreateTranslation(polozaj);
            //collisionSystem.DetectAllCollisions(this.bodies, collisionFunctor, null, 0.05f);
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
