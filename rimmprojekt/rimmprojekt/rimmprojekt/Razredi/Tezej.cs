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

namespace rimmprojekt.Razredi
{
    class Tezej : Kocka, IUpdate
    {
        public Vector3 polozaj;
        //private IShader shader;

        public Tezej(float x, float y, float z, UpdateManager manager)
            : base(x, y, z)
        {
            manager.Add(this);
            polozaj = new Vector3(x, y, z);
            MaterialShader material = new MaterialShader();
            material.SpecularColour = Color.LightYellow.ToVector3();//with a nice sheen

            Vector3 lightDirection = new Vector3(0.5f, 1, -0.5f); //a less dramatic direction

            MaterialLightCollection lights = new MaterialLightCollection();
            lights.AmbientLightColour = Color.Blue.ToVector3() * 0.5f;
            //lights.CreateDirectionalLight(lightDirection, Color.Red);

            material.LightCollection = lights;

            shader = material;
        }

        override public void Draw(DrawState state)
        {
            //draw the vertices as a triangle list, with the indices
            using (state.WorldMatrix.PushMultiply(ref matrix))
            {
                //cull test the custom geometry
                if (CullTest(state))
                {
                    //bind the shader
                    using (state.Shader.Push(shader))
                    {
                    //draw the custom geometry
                        this.vertices.Draw(state, this.indices, PrimitiveType.TriangleList);
                    }
                }
            }
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
            matrix = Matrix.CreateTranslation(polozaj);
            return UpdateFrequency.FullUpdate60hz;
        }
    }
}
