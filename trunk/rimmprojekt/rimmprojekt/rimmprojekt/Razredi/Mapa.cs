using System;
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

namespace rimmprojekt.Razredi
{
    class Mapa : IDraw, IContentOwner
    {
        private Kocka[] zidovi;
        private IVertices tla;
        private IIndices indices;
        private Point velikost;
        private MaterialShader material;

        public Mapa(string pot, ContentRegister content)
        {
            using (StreamReader sr = new StreamReader(pot))
            {
                Vector2 topLeft = new Vector2(0.0f, 0.0f);
                Vector2 topRight = new Vector2(10.0f, 0.0f);
                Vector2 bottomLeft = new Vector2(0.0f, 10.0f);
                Vector2 bottomRight = new Vector2(10.0f, 10.0f);
                velikost.X = Int32.Parse(sr.ReadLine());
                velikost.Y = Int32.Parse(sr.ReadLine());
                VertexPositionNormalTexture[] tempTla = new VertexPositionNormalTexture[]{
                    new VertexPositionNormalTexture(new Vector3(-10.0f, -10.0f, (float)velikost.X*20.0f- 10.0f), Vector3.Up, topLeft),
                    new VertexPositionNormalTexture(new Vector3(-10.0f, -10.0f, -10.0f), Vector3.Up, bottomLeft),
                    new VertexPositionNormalTexture(new Vector3((float)velikost.Y*20.0f -10.0f, -10.0f, (float)velikost.X*20.0f - 10.0f), Vector3.Up, topRight),
                    new VertexPositionNormalTexture(new Vector3((float)velikost.Y*20.0f - 10.0f, -10.0f, -10.0f), Vector3.Up, bottomRight)
                };
                tla = new Vertices<VertexPositionNormalTexture>(tempTla);
                tempTla = null;

                ushort[] inds =
			    {
				    0,1,2,
				    1,3,2
			    };
                indices = new Indices<ushort>(inds);

                zidovi = new Kocka[velikost.X * velikost.Y];
                int stevec = 0;
                for (int i = 0; i < velikost.X; i++)
                {
                    for (int j = 0; j < velikost.Y; j++)
                    {
                        if (sr.Read() - 48 == 0)
                        {
                            zidovi[stevec] = new Kocka((float)j * 20.0f, 0.0f, (float)i * 20.0f, content);
                            stevec++;
                        }
                    }
                    sr.Read();
                    sr.Read();
                }
            }

            material = new MaterialShader();
            material.SpecularColour = Color.LightYellow.ToVector3();//with a nice sheen

            Vector3 lightDirection = new Vector3(0.5f, 1, -0.5f); //a less dramatic direction

            MaterialLightCollection lights = new MaterialLightCollection();
            lights.AmbientLightColour = Color.White.ToVector3() * 0.5f;
            lights.CreateDirectionalLight(lightDirection, Color.White);

            material.LightCollection = lights;

            material.Textures = new MaterialTextures();
            material.Textures.TextureMapSampler = TextureSamplerState.PointFiltering;

            content.Add(this);
        }

        public void Draw(DrawState state)
        {
            using (state.Shader.Push(material))
            {
                tla.Draw(state, indices, PrimitiveType.TriangleList);
                int stevec = 0;
                while (zidovi[stevec] != null)
                {
                    zidovi[stevec].Draw(state);
                    stevec++;
                }
            }
        }

        public bool CullTest(ICuller culler)
        {
            return true;
        }

        void IContentOwner.LoadContent(ContentState state)
        {
            material.Textures.TextureMap = state.Load<Texture2D>(@"Textures/tla");
        }
    }
}
