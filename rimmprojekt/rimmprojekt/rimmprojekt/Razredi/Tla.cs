﻿using System;
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
    class Tla : IDraw, IContentOwner
    {
        private Point velikost;
        private IVertices tla;
        private IIndices indices;
        private MaterialShader material;

        private Vector3 polozaj;

        public Tla(Point v, ContentRegister content)
        {
            velikost = v;

            polozaj = new Vector3(0.0f, 0.0f, 0.0f);

            Vector2 topLeft = new Vector2(0.0f, 0.0f);
            Vector2 topRight = new Vector2(10.0f, 0.0f);
            Vector2 bottomLeft = new Vector2(0.0f, 10.0f);
            Vector2 bottomRight = new Vector2(10.0f, 10.0f);

            VertexPositionNormalTexture[] tempTla = new VertexPositionNormalTexture[]{
                    new VertexPositionNormalTexture(new Vector3(-10.0f, -10.0f, (float)velikost.X*20.0f-10f), Vector3.Up, topLeft),
                    new VertexPositionNormalTexture(new Vector3(-10.0f, -10.0f, 0.0f), Vector3.Up, bottomLeft),
                    new VertexPositionNormalTexture(new Vector3((float)velikost.Y*20.0f-10f, -10.0f, (float)velikost.X*20.0f-10f), Vector3.Up, topRight),
                    new VertexPositionNormalTexture(new Vector3((float)velikost.Y*20.0f-10f, -10.0f, 0.0f), Vector3.Up, bottomRight)
                };
            tla = new Vertices<VertexPositionNormalTexture>(tempTla);
             //tempTla = null;

            ushort[] inds =
			    {
				    0,1,2,
				    1,3,2
			    };
            indices = new Indices<ushort>(inds);

            material = new MaterialShader();
            material.SpecularColour = Color.LightYellow.ToVector3();//with a nice sheen

            Vector3 lightDirection = new Vector3(0.7f, 0.1f, -0.5f); //a less dramatic direction

            MaterialLightCollection lights = new MaterialLightCollection();
            lights.AmbientLightColour = Color.White.ToVector3() * 0.2f;
            lights.CreateDirectionalLight(lightDirection, Color.White);

            material.LightCollection = lights;

            material.Textures = new MaterialTextures();
            material.Textures.TextureMapSampler = TextureSamplerState.PointFiltering;

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
    }
}