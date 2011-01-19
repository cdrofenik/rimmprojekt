using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

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
    class Kocka : IDraw, IContentOwner, IUpdate
    {
        protected Matrix matrix;
        protected Vector3 polozaj;
        protected readonly IVertices vertices;
        protected readonly IIndices indices;
        protected MaterialShader material;

        public Body body;
        private CollisionSkin skin;

        public Kocka(float x, float y, float z, ContentRegister content, UpdateManager manager)
        {
            polozaj = new Vector3(x, y, z);
            matrix = Matrix.CreateTranslation(polozaj);

            manager.Add(this);

            Vector2 topLeft = new Vector2(0.0f, 0.0f);
            Vector2 topRight = new Vector2(1.0f, 0.0f);
            Vector2 bottomLeft = new Vector2(0.0f, 1.0f);
            Vector2 bottomRight = new Vector2(1.0f, 1.0f);

            VertexPositionNormalTexture[] ploskve = new VertexPositionNormalTexture[]
            {
                // Front Surface
                new VertexPositionNormalTexture(new Vector3(-10.0f, -10.0f, 10.0f), Vector3.Backward, bottomLeft),
                new VertexPositionNormalTexture(new Vector3(-10.0f, 10.0f, 10.0f), Vector3.Backward, topLeft), 
                new VertexPositionNormalTexture(new Vector3(10.0f, -10.0f, 10.0f), Vector3.Backward, bottomRight),
                new VertexPositionNormalTexture(new Vector3(10.0f, 10.0f, 10.0f), Vector3.Backward, topRight),  

                // Back Surface
                new VertexPositionNormalTexture(new Vector3(10.0f, -10.0f, -10.0f), Vector3.Forward, bottomLeft),
                new VertexPositionNormalTexture(new Vector3(10.0f, 10.0f, -10.0f), Vector3.Forward, topLeft), 
                new VertexPositionNormalTexture(new Vector3(-10.0f, -10.0f, -10.0f), Vector3.Forward, bottomRight),
                new VertexPositionNormalTexture(new Vector3(-10.0f, 10.0f, -10.0f), Vector3.Forward, topRight), 

                // Left Surface
                new VertexPositionNormalTexture(new Vector3(-10.0f, -10.0f, -10.0f), Vector3.Left, bottomLeft),
                new VertexPositionNormalTexture(new Vector3(-10.0f, 10.0f, -10.0f), Vector3.Left, topLeft),
                new VertexPositionNormalTexture(new Vector3(-10.0f, -10.0f, 10.0f), Vector3.Left, bottomRight),
                new VertexPositionNormalTexture(new Vector3(-10.0f, 10.0f, 10.0f), Vector3.Left, topRight),

                // Right Surface
                new VertexPositionNormalTexture(new Vector3(10.0f, -10.0f, 10.0f), Vector3.Right, bottomLeft),
                new VertexPositionNormalTexture(new Vector3(10.0f, 10.0f, 10.0f), Vector3.Right, topLeft),
                new VertexPositionNormalTexture(new Vector3(10.0f, -10.0f, -10.0f), Vector3.Right, bottomRight),
                new VertexPositionNormalTexture(new Vector3(10.0f, 10.0f, -10.0f), Vector3.Right, topRight),

                // Top Surface
                new VertexPositionNormalTexture(new Vector3(-10.0f, 10.0f, 10.0f), Vector3.Up, bottomLeft),
                new VertexPositionNormalTexture(new Vector3(-10.0f, 10.0f, -10.0f), Vector3.Up, topLeft),
                new VertexPositionNormalTexture(new Vector3(10.0f, 10.0f, 10.0f), Vector3.Up, bottomRight),
                new VertexPositionNormalTexture(new Vector3(10.0f, 10.0f, -10.0f), Vector3.Up, topRight),

                // Bottom Surface
                new VertexPositionNormalTexture(new Vector3(-10.0f, -10.0f, -10.0f), Vector3.Down, bottomLeft),
                new VertexPositionNormalTexture(new Vector3(-10.0f, -10.0f, 10.0f), Vector3.Down, topLeft),
                new VertexPositionNormalTexture(new Vector3(10.0f, -10.0f, -10.0f), Vector3.Down, bottomRight),
                new VertexPositionNormalTexture(new Vector3(10.0f, -10.0f, 10.0f), Vector3.Down, topRight)
            };

            short[] boxIndices = new short[]
            { 
	            0, 1, 2, 2, 1, 3,   
	            4, 5, 6, 6, 5, 7,
	            8, 9, 10, 10, 9, 11, 
	            12, 13, 14, 14, 13, 15, 
	            16, 17, 18, 18, 17, 19,
	            20, 21, 22, 22, 21, 23
            };

            this.vertices = new Vertices<VertexPositionNormalTexture>(ploskve);
            this.indices = new Indices<short>(boxIndices);

            ploskve = null;
            boxIndices = null;

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

            Box box = new Box(Vector3.Zero, Matrix.Identity, new Vector3(20.0f, 20.0f, 20.0f));
            skin.AddPrimitive(box, new MaterialProperties(0.0f, 1.0f, 1.0f));

            //Vector3 com = SetMass(1.0f);

            body.MoveTo(polozaj, Matrix.Identity);
            //skin.ApplyLocalTransform(new JigLibX.Math.Transform(polozaj, Matrix.Identity));
            body.EnableBody();
            //body.Immovable = true;

            content.Add(this);
        }

        virtual public void Draw(DrawState state)
        {
            //draw the vertices as a triangle list, with the indices
            using (state.WorldMatrix.PushMultiply(ref matrix))
            {
                //cull test the custom geometry
                if (CullTest(state))
                {
                    //bind the shader
                    using (state.Shader.Push(material))
                    {
                        //draw the custom geometry
                        this.vertices.Draw(state, this.indices, Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList);
                    }
                }
            }
        }

        public bool CullTest(ICuller culler)
        {
            //cull test with a bounding box...
            //the box is represented as 'min / max' positions.
            //the vertex positions range from -1,-1,0 to 1,1,0

            //If the camera were changed, and this quad were offscreen, the cull test
            //would return false, and it would not be drawn.
            //return culler.TestBox(new Vector3(-1, -1, 0), new Vector3(1, 1, 0));
            return true;
        }

        void IContentOwner.LoadContent(ContentState state)
        {
            material.Textures.TextureMap = state.Load<Texture2D>(@"Textures/zid");
        }

        public UpdateFrequency Update(UpdateState state)
        {
            body.MoveTo(polozaj, Matrix.Identity);
            return UpdateFrequency.FullUpdate60hz;
        }
    }
}
