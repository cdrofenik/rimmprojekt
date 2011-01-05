using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Xen;
using Xen.Camera;
using Xen.Graphics;
using Xen.Ex.Graphics;
using Xen.Ex.Graphics2D;
using Xen.Ex.Geometry;
using Xen.Ex.Graphics.Content;
using Xen.Ex.Material;

using Microsoft.Xna.Framework.Content;

namespace rimmprojekt.Razredi
{
    class Tezej : IDraw, IContentOwner, IUpdate
    {
        private Texture2D tezejTexture;
        private TexturedElement sideElement;
        private Vector2 sizeOfSideElement;

        public Vector3 polozaj;
        private Matrix matrix;
        private IShader shader;
        private ModelInstance model;

        public Tezej(float x, float y, float z, UpdateManager manager, ContentRegister content)
        {
            manager.Add(this);
            model = new ModelInstance();
            content.Add(this);

            //inicializacija stranskega elementa
            sizeOfSideElement = new Vector2(649, 160);
            sideElement = new TexturedElement(tezejTexture, sizeOfSideElement);
            sideElement.AlphaBlendState = Xen.Graphics.AlphaBlendState.Alpha;


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
        }

        public void Draw(DrawState state)
        {
            //draw the vertices as a triangle list, with the indices

            using (state.Shader.Push())
            {
                if (CullTest(state))
                {
                        sideElement.Draw(state);
                }
            }


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
                        sideElement.Draw(state);
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
            model.ModelData = state.Load<Xen.Ex.Graphics.Content.ModelData>(@"tiny_4anim");
            tezejTexture = state.Load<Texture2D>(@"tezejus");
        }

        public UpdateFrequency Update(UpdateState state)
        {
            if (state.KeyboardState.KeyState.S.IsDown)
                polozaj.Z += 1.0f;
            if (state.KeyboardState.KeyState.W.IsDown)
                polozaj.Z -= 1.0f;
            if (state.KeyboardState.KeyState.D.IsDown)
                polozaj.X += 1.0f;
            if (state.KeyboardState.KeyState.A.IsDown)
                polozaj.X -= 1.0f;
            matrix = Matrix.CreateScale(0.04f, 0.04f, 0.04f) * Matrix.CreateRotationZ((float)Math.PI) *Matrix.CreateRotationX((float)Math.PI/2);
            matrix *= Matrix.CreateTranslation(polozaj);
            return UpdateFrequency.FullUpdate60hz;
        }
    }
}
