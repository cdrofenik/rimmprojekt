using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;

using Xen;
using Xen.Camera;
using Xen.Graphics;
using Xen.Ex.Geometry;
using Xen.Ex.Material;

using JigLibX.Math;
using JigLibX.Physics;
using JigLibX.Geometry;
using JigLibX.Collision;

namespace rimmprojekt.Razredi
{
    class Tla : IDraw, IContentOwner, IUpdate
    {
        private Point velikost;
        private IVertices tla;
        private IIndices indices;
        private MaterialShader material;

        private Vector3 polozaj;

        private Body body;
        public CollisionSkin skin;

        public Tla(Point v, ContentRegister content)
        {
            velikost = v;

            polozaj = new Vector3(0.0f, 0.0f, 0.0f);

            Vector2 topLeft = new Vector2(0.0f, 0.0f);
            Vector2 topRight = new Vector2(10.0f, 0.0f);
            Vector2 bottomLeft = new Vector2(0.0f, 10.0f);
            Vector2 bottomRight = new Vector2(10.0f, 10.0f);

            VertexPositionNormalTexture[] tempTla = new VertexPositionNormalTexture[]{
                    new VertexPositionNormalTexture(new Vector3(0.0f, -10.0f, (float)velikost.X*20.0f), Vector3.Up, topLeft),
                    new VertexPositionNormalTexture(new Vector3(0.0f, -10.0f, 0.0f), Vector3.Up, bottomLeft),
                    new VertexPositionNormalTexture(new Vector3((float)velikost.Y*20.0f, -10.0f, (float)velikost.X*20.0f), Vector3.Up, topRight),
                    new VertexPositionNormalTexture(new Vector3((float)velikost.Y*20.0f, -10.0f, 0.0f), Vector3.Up, bottomRight)
                };
            tla = new Vertices<VertexPositionNormalTexture>(tempTla);
            tempTla = null;

            ushort[] inds =
			    {
				    0,1,2,
				    1,3,2
			    };
            indices = new Indices<ushort>(inds);

            material = new MaterialShader();
            material.SpecularColour = Color.LightYellow.ToVector3();//with a nice sheen

            Vector3 lightDirection = new Vector3(0.5f, 1, -0.5f); //a less dramatic direction

            MaterialLightCollection lights = new MaterialLightCollection();
            lights.AmbientLightColour = Color.White.ToVector3() * 0.5f;
            lights.CreateDirectionalLight(lightDirection, Color.White);

            material.LightCollection = lights;

            material.Textures = new MaterialTextures();
            material.Textures.TextureMapSampler = TextureSamplerState.PointFiltering;

            body = new Body();
            skin = new CollisionSkin(body);
            body.CollisionSkin = skin;

            JigLibX.Geometry.Plane plane = new JigLibX.Geometry.Plane(Vector3.UnitY, 0f);
            skin.AddPrimitive(plane, new MaterialProperties(0.0f, 100.0f, 100.0f));

            Vector3 com = SetMass(2.0f);

            body.MoveTo(polozaj, Matrix.Identity);
            skin.ApplyLocalTransform(new JigLibX.Math.Transform(-com, Matrix.Identity));
            body.EnableBody();
            body.Immovable = true;

            content.Add(this);
        }
        
        public void Draw(DrawState state)
        {
            Matrix matrix = Matrix.CreateTranslation(polozaj);
            using (state.WorldMatrix.PushMultiply(ref matrix))
                if (CullTest(state))
                    using (state.Shader.Push(material))
                        tla.Draw(state, indices, Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList);
        }

        public bool CullTest(ICuller culler)
        {
            return true;
        }

        void IContentOwner.LoadContent(ContentState state)
        {
            material.Textures.TextureMap = state.Load<Texture2D>(@"Textures/tla2");
        }

        public UpdateFrequency Update(UpdateState state)
        {
            //polozaj = body.Position;
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
    }
}
