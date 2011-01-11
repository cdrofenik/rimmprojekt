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

namespace rimmprojekt.Razredi
{
    class Inventory : IDraw, IContentOwner, IUpdate
    {
        //parameters
        private Texture2D inventoryTexture;
        private TexturedElement element;
        private Vector2 sizeOfElement;

        private List<Oprema> inventoryList = new List<Oprema>();
        private Boolean isVisable = false;

        public Inventory(UpdateManager manager, ContentRegister content)
		{
            manager.Add(this);
            content.Add(this);
            sizeOfElement = new Vector2(330.0f, 332.0f);
            element = new TexturedElement(inventoryTexture,sizeOfElement);
            element.AlphaBlendState = Xen.Graphics.AlphaBlendState.Modulate;
		}

        public void Draw(DrawState state)
        {
            using (state.Shader.Push())
            {
                if (CullTest(state))
                {
                    if (isVisable)
                    {
                        element.Position = new Vector2(state.Application.WindowWidth - 380, state.Application.WindowHeight - 340);
                        element.Draw(state);
                    }
                }
            }
        }

        public bool CullTest(ICuller culler)
        {
            return true;
        }

        public UpdateFrequency Update(UpdateState state)
        {
            if (state.KeyboardState.KeyState.I.OnReleased)
            {
                if (isVisable)
                {
                    isVisable = false;
                }
                else
                {
                    isVisable = true;
                }
            }
            element = new TexturedElement(inventoryTexture, sizeOfElement);
            return UpdateFrequency.FullUpdate60hz;
        }

        public void LoadContent(ContentState state)
        {
            inventoryTexture = state.Load<Texture2D>(@"inventory");
        }

        //custom methods
        public void addItem(Oprema item)
        {
            inventoryList.Add(item);
        }
    }
}
