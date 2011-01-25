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
    //this class simply draws the sphere representing the lights
    class LightSourceDrawer : IDraw
    {
		private IDraw geometry;
		private Vector3 position;
		private Color lightColour;

		public LightSourceDrawer(Vector3 position, IDraw geometry, Color lightColour)
		{
			this.position = position;
			this.geometry = geometry;
			this.lightColour = lightColour;
		}

		public void Draw(DrawState state)
		{
			using (state.WorldMatrix.PushTranslateMultiply(ref position))
			{
				DrawSphere(state);
			}
		}

		private void DrawSphere(DrawState state)
		{
			//draw the geometry with a solid colour shader
			if (geometry.CullTest(state))
			{
				Xen.Ex.Shaders.FillSolidColour shader = state.GetShader<Xen.Ex.Shaders.FillSolidColour>();

				shader.FillColour = lightColour.ToVector4();

				using (state.Shader.Push(shader))
				{
					geometry.Draw(state);
				}
			}
		}

		public bool CullTest(ICuller culler)
		{
			return true;
		}
	}
}
