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
    class Minotaver : IDraw, IContentOwner, IUpdate
    {
        public Int32 healthPoints;
        public Int32 damage;
        public Int32 durability;

        //displaying properties
        public Vector3 polozaj;
        private Matrix matrix;
        private IShader shader;
        private ModelInstance model;

        public Minotaver(float x, float y, float z, UpdateManager manager, ContentRegister content)
        {
            manager.Add(this);
            model = new ModelInstance();
            content.Add(this);

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
            //model.ModelData = state.Load<Xen.Ex.Graphics.Content.ModelData>(@"Minotaver/minotaur");
        }

        public UpdateFrequency Update(UpdateState state)
        {
            if (state.KeyboardState.KeyState.P.OnPressed)
                polozaj.X += 10;
            return UpdateFrequency.FullUpdate60hz;
        }
    }
}
