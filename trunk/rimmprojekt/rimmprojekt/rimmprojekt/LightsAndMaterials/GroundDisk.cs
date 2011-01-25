using System;
using System.Collections.Generic;
using System.Text;

using Xen;
using Xen.Camera;
using Xen.Graphics;
using Xen.Ex.Graphics2D;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Xen.Ex.Material;
using Xen.Ex.Graphics;
using Xen.Ex.Graphics.Content;
namespace rimmprojekt.LightsAndMaterials
{
    class GroundDisk : IDraw, IContentOwner
    {
        private Point velikost;
        private IVertices tla;
        private IIndices indices;
        private MaterialShader material;

        private Vector3 polozaj;

        public GroundDisk(ContentRegister content, MaterialLightCollection lights, Point v)
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
            
            ushort[] inds =
			    {
				    0,1,2,
				    1,3,2
			    };

            tla = new Vertices<VertexPositionNormalTexture>(tempTla);
            tempTla = null;
            indices = new Indices<ushort>(inds);


            //create the custom material for this geometry
            //the light collection has been passed into the constructor, although it
            //could easily be changed later (by changing material.Lights)
            this.material = new MaterialShader(lights);

            //give the disk really bright specular for effect
            this.material.SpecularColour = new Vector3(1, 1, 1);
            this.material.DiffuseColour = new Vector3(0.6f, 0.6f, 0.6f);
            this.material.SpecularPower = 64;

            //setup the texture samples to use high quality anisotropic filtering
            //the textures are assigned in LoadContent
            this.material.Textures = new MaterialTextures();
            this.material.Textures.TextureMapSampler = TextureSamplerState.AnisotropicHighFiltering;
            this.material.Textures.NormalMapSampler = TextureSamplerState.AnisotropicLowFiltering;

            //load the textures for this material
            content.Add(this);
        }

        public void LoadContent(ContentState state)
        {
            //load the box texture, and it's normal map.
            material.Textures.TextureMap = state.Load<Texture2D>(@"Battle/Ambient/box");
            //material.Textures.NormalMap = state.Load<Texture2D>(@"Battle/Ambient/box_normal");
        }

        //draw the ground plane..
        public void Draw(DrawState state)
        {
            //first bind the material shader
            using (state.Shader.Push(material)){
                //then draw the vertices
                tla.Draw(state, indices, PrimitiveType.TriangleList);
            }
        }

        public bool CullTest(ICuller culler)
        {
            return true;
        }
    }
}
