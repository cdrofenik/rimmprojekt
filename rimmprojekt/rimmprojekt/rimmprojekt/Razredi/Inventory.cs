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
        #region parameters
        //parameters
        private Texture2D inventoryTexture;
        private Texture2D hpPotionTexture;
        private Texture2D mpPotionTexture;
        private TexturedElement element;
        private Vector2 sizeOfElement;
        private Vector2 beginVectorFirstInventoryRow;
        private Vector2 beginVectorSecondInventoryRow;
        private Tezej tezej;

        private List<TexturedElement> inventoryList = new List<TexturedElement>();
        private List<String> typeList = new List<String>();
        private List<Int32> valueList = new List<Int32>();

        private Boolean isVisable = false;
        #endregion

        public Inventory(UpdateManager manager, ContentRegister content, Tezej theseus)
		{
            this.beginVectorFirstInventoryRow = new Vector2(946, 480);
            this.beginVectorSecondInventoryRow = new Vector2(946, 440);
            this.tezej = theseus;
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

                        foreach (TexturedElement el in inventoryList)
                        {
                            el.Draw(state);
                        }
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

            if (state.KeyboardState.KeyState.F.OnPressed)
            {
                addHpPotion();
            }

            if (state.KeyboardState.KeyState.G.OnPressed)
            {
                addManaPotion();
            }

            #region number buttons
            if (state.KeyboardState.KeyState.D1.OnPressed)
            {
                useItem(0);
            }
            if (state.KeyboardState.KeyState.D2.OnPressed)
            {
                useItem(1);
            }
            if (state.KeyboardState.KeyState.D3.OnPressed)
            {
                useItem(2);
            }
            if (state.KeyboardState.KeyState.D4.OnPressed)
            {
                useItem(3);
            }
            if (state.KeyboardState.KeyState.D5.OnPressed)
            {
                useItem(4);
            }
            if (state.KeyboardState.KeyState.D6.OnPressed)
            {
                useItem(5);
            }
            if (state.KeyboardState.KeyState.D7.OnPressed)
            {
                useItem(6);
            }
            if (state.KeyboardState.KeyState.D8.OnPressed)
            {
                useItem(7);
            }
            if (state.KeyboardState.KeyState.D9.OnPressed)
            {
                useItem(8);
            }
            #endregion

            sortInventoryItems();

            element = new TexturedElement(inventoryTexture, sizeOfElement);
            return UpdateFrequency.FullUpdate60hz;
        }

        public void LoadContent(ContentState state)
        {
            inventoryTexture = state.Load<Texture2D>(@"inventory");
            hpPotionTexture = state.Load<Texture2D>(@"Textures/hPotion");
            mpPotionTexture = state.Load<Texture2D>(@"Textures/mPotion");
        }

        //custom methods
        public void addHpPotion()
        {
            inventoryList.Add(new TexturedElement(hpPotionTexture, new Vector2(37, 36)));
            typeList.Add("hp");
            valueList.Add(20);
        }

        public void addManaPotion()
        {
            inventoryList.Add(new TexturedElement(mpPotionTexture, new Vector2(37, 36)));
            typeList.Add("mp");
            valueList.Add(20);
        }

        public void removeItem(int index)
        {
            inventoryList.RemoveAt(index);
            typeList.RemoveAt(index);
            valueList.RemoveAt(index);
        }

        private void sortInventoryItems()
        {
            int counter = 0;
            int odmikX = 0;
            foreach(TexturedElement tmpElement in inventoryList)
            {
                if (counter == 0)
                {
                    tmpElement.Position = new Vector2(beginVectorFirstInventoryRow.X, beginVectorFirstInventoryRow.Y);
                }
                else if (counter > 0 && counter < 6)
                {
                    odmikX = counter * 40;
                    //odmikY = counter * 36;
                    tmpElement.Position = new Vector2(beginVectorFirstInventoryRow.X + odmikX, beginVectorFirstInventoryRow.Y);
                }
                else
                {
                    odmikX = (counter-6) * 40;
                    //odmikY = counter * 36;
                    tmpElement.Position = new Vector2(beginVectorSecondInventoryRow.X + odmikX, beginVectorSecondInventoryRow.Y);
                }
                counter++;
            }
        }

        private void useItem(Int32 index)
        {
            if (inventoryList.Count > 0)
            {
                if (typeList.IndexOf("hp") == index)
                {
                    tezej.healthPoints = tezej.healthPoints + 20;
                    removeItem(0);
                }
                else
                {
                    tezej.manaPoints = tezej.manaPoints + 20;
                    removeItem(0);
                }
            }
        }
    }
}
