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
        private Texture2D emptyTexture;
        private TexturedElement element;
        private Vector2 sizeOfElement;
        private Vector2 beginVectorFirstInventoryRow;
        private Vector2 beginVectorSecondInventoryRow;
        private Tezej tezej;
        private TexturedElement[] inventoryTextureArray = new TexturedElement[12];
        private Potion[] inventoryPotionArray = new Potion[12];
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
            inicializeInventoryItems();
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

                        foreach (TexturedElement el in inventoryTextureArray)
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

            if (isVisable)
            {
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

                if (state.KeyboardState.KeyState.F.OnPressed)
                {
                    addPotion("mp", 20);
                }
            }

            element = new TexturedElement(inventoryTexture, sizeOfElement);
            return UpdateFrequency.FullUpdate60hz;
        }

        public void LoadContent(ContentState state)
        {
            inventoryTexture = state.Load<Texture2D>(@"inventory");
            hpPotionTexture = state.Load<Texture2D>(@"Textures/hPotion");
            mpPotionTexture = state.Load<Texture2D>(@"Textures/mPotion");
            emptyTexture = state.Load<Texture2D>(@"Textures/emptyInventoryItem");
        }

        //custom methods
        private void useItem(Int32 index)
        {
            if (!inventoryPotionArray[index].type.Equals(""))
            {
                if (inventoryPotionArray[index].type.Equals("hp"))
                {
                    tezej.healthPoints = tezej.healthPoints + inventoryPotionArray[index].value;
                    inventoryTextureArray[index].Texture = emptyTexture;
                    inventoryPotionArray[index] = new Potion();
                }
                else
                {
                    tezej.manaPoints = tezej.manaPoints + inventoryPotionArray[index].value;
                    inventoryTextureArray[index].Texture = emptyTexture;
                    inventoryPotionArray[index] = new Potion();
                }
            }
        }

        private void inicializeInventoryItems()
        {
            int counter = 0;
            int odmikX = 0;
            for(int i = 0; i < 12;i++)
            {
                if (counter == 0)
                {
                    inventoryTextureArray[i] = new TexturedElement(new Vector2(37, 36));
                    inventoryTextureArray[i].Position = new Vector2(beginVectorFirstInventoryRow.X, beginVectorFirstInventoryRow.Y);
                    inventoryTextureArray[i].Texture = emptyTexture;
                    inventoryPotionArray[i] = new Potion();
                }
                else if (counter > 0 && counter < 6)
                {
                    odmikX = counter * 40;
                    inventoryTextureArray[i] = new TexturedElement(new Vector2(37, 36));
                    inventoryTextureArray[i].Position = new Vector2(beginVectorFirstInventoryRow.X + odmikX, beginVectorFirstInventoryRow.Y);
                    inventoryTextureArray[i].Texture = emptyTexture;
                    inventoryPotionArray[i] = new Potion();
                }
                else
                {
                    odmikX = (counter-6) * 40;
                    inventoryTextureArray[i] = new TexturedElement(new Vector2(37, 36));
                    inventoryTextureArray[i].Position = new Vector2(beginVectorSecondInventoryRow.X + odmikX, beginVectorSecondInventoryRow.Y);
                    inventoryTextureArray[i].Texture = emptyTexture;
                    inventoryPotionArray[i] = new Potion();
                }
                counter++;
            }
        }

        public void addPotion(String potionType, Int32 potionValue)
        {
            for(int i = 0; i < 12;i++)
            {
                if (inventoryPotionArray[i].type == "")
                {
                    inventoryPotionArray[i] = new Potion(potionType, potionValue);
                    if (potionType == "hp")
                    {
                        inventoryTextureArray[i].Texture = hpPotionTexture;
                    }
                    else
                    {
                        inventoryTextureArray[i].Texture = mpPotionTexture;
                    }
                    break;
                }
                else
                {
                    //inventory is full!
                }
            }
        }
    }
}
