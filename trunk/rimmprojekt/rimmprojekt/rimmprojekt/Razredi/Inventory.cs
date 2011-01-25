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

        public Boolean isItemUsed;
        private Int32 inventoryPointer;
        public Boolean isInBattle;
        private Vector2 sizeOfElement;
        private Tezej tezej;
        private Razredi.Character tezejChar;
        private SpriteFont bigFont;

        private Texture2D pointer;
        private Texture2D inventoryTexture;
        private Texture2D hpPotionTexture;
        private Texture2D mpPotionTexture;
        
        private TexturedElement hpPoitionDisplay;
        private TexturedElement mpPoitionDisplay;
        private TexturedElement element;

        private TextElementRect textBox;
        private TextElement hpValue;
        private TextElement mpValue;

        private TexturedElement[] inventoryTable;
        private Int32 hpListCounter;
        private List<Potion> hpPotionList;
        private Int32 mpListCounter;
        private List<Potion> mpPotionList;
        #endregion

        public Inventory(UpdateManager manager, ContentRegister content, Tezej theseus)
		{
            isInBattle = false;
            isItemUsed = false;
            this.tezej = theseus;
            manager.Add(this);
            content.Add(this);

            inventoryPointer = 1;
            inventoryTable = new TexturedElement[2];
            hpPotionList = new List<Potion>();
            mpPotionList = new List<Potion>();

            sizeOfElement = new Vector2(380.0f, 210.0f);
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
                    if (isInBattle)
                    {
                        element.Position = new Vector2(state.Application.WindowWidth - 1000, state.Application.WindowHeight - 700);
                        element.Draw(state);
                        hpPoitionDisplay.Draw(state);
                        mpPoitionDisplay.Draw(state);
                        textBox.Draw(state);
                        inventoryTable[inventoryPointer].Texture = pointer;
                        inventoryTable[inventoryPointer].Draw(state);
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
            mpListCounter = mpPotionList.Count;
            hpListCounter = hpPotionList.Count;

            #region isInBattle
            if (isInBattle)
            {
                hpValue.Text.SetText(hpPotionList.Count);
                mpValue.Text.SetText(mpPotionList.Count);
                if (state.KeyboardState.KeyState.W.OnPressed)
                {
                    if (inventoryPointer == 1)
                        inventoryPointer = 0;
                    else
                        inventoryPointer++;
                }

                if (state.KeyboardState.KeyState.S.OnPressed)
                {
                    if (inventoryPointer == 0)
                        inventoryPointer = 1;
                    else
                        inventoryPointer--;
                }

                if (state.KeyboardState.KeyState.Space.OnPressed)
                {
                    if(inventoryPointer == 1)
                        useItem(inventoryPointer);
                    else
                        useItem(inventoryPointer);
                }
            }
            #endregion

            element = new TexturedElement(inventoryTexture, sizeOfElement);
            return UpdateFrequency.FullUpdate60hz;
        }

        public void LoadContent(ContentState state)
        {
            pointer = state.Load<Texture2D>(@"Battle/pointer");
            inventoryTexture = state.Load<Texture2D>(@"Battle/rightBar");
            hpPotionTexture = state.Load<Texture2D>(@"Inventory/potionHp");
            mpPotionTexture = state.Load<Texture2D>(@"Inventory/potionMp");
            bigFont = state.Load<SpriteFont>("ArialBattle");
        }

        //custom methods
        private void useItem(Int32 row)
        {
            if (row == 1)
            {
                if (hpPotionList.Count != 0)
                {
                    tezejChar.Health = tezejChar.Health + hpPotionList[hpPotionList.Count - 1].value;
                    hpPotionList.RemoveAt(hpPotionList.Count - 1);
                    isItemUsed = true;
                }
                
            }
            else
            {
                if (mpPotionList.Count != 0)
                {
                    tezejChar.Mana = tezejChar.Mana + mpPotionList[mpPotionList.Count - 1].value;
                    mpPotionList.RemoveAt(mpPotionList.Count - 1);
                    isItemUsed = true;
                }
            }
        }

        private void inicializeInventoryItems() 
        {
            hpPoitionDisplay = new TexturedElement(new Vector2(40, 38));
            hpPoitionDisplay.Texture = hpPotionTexture;
            hpPoitionDisplay.Position = new Vector2(350, 155);

            mpPoitionDisplay = new TexturedElement(new Vector2(40, 38));
            mpPoitionDisplay.Texture = mpPotionTexture;
            mpPoitionDisplay.Position = new Vector2(350, 100);

            textBox = new TextElementRect(new Vector2(400, 50));
            textBox.Font = bigFont;
            textBox.AlphaBlendState = Xen.Graphics.AlphaBlendState.Alpha;
            textBox.VerticalAlignment = VerticalAlignment.Bottom;
            textBox.HorizontalAlignment = HorizontalAlignment.Left;
            textBox.TextHorizontalAlignment = TextHorizontalAlignment.Left;
            textBox.Colour = Color.White;
            textBox.Position = new Vector2(395, 140);
            textBox.Text.AppendLine("Health potions x");
            textBox.Text.AppendLine();
            textBox.Text.AppendLine("Mana potions x");

            hpValue = new TextElement("hpValue");
            hpValue.Font = bigFont;
            hpValue.Colour = Color.Yellow;
            hpValue.Position = new Vector2(180, 0);
            hpValue.Text.SetText("VALUE");

            mpValue = new TextElement("mpValue");
            mpValue.Font = bigFont;
            mpValue.Colour = Color.Yellow;
            mpValue.Position = new Vector2(180, -57);
            mpValue.Text.SetText("VALUE");

            textBox.Add(hpValue);
            textBox.Add(mpValue);

            Vector2 pointerSize = new Vector2(20, 20);
            inventoryTable[0] = new TexturedElement(pointerSize);
            inventoryTable[0].Position = new Vector2(324, 110);             // mana potion pointer location
            inventoryTable[1] = new TexturedElement(pointerSize);
            inventoryTable[1].Position = new Vector2(324, 165);             //hp potion pointer location
        }

        public void addPotion(String potionType, Int32 potionValue)
        {
            if (potionType.Contains("hp"))
                hpPotionList.Add(new Potion(potionType, potionValue));
            else
            {
                mpPotionList.Add(new Potion(potionType, potionValue));
            }
        }

        public void setCharacter(Razredi.Character character)
        {
            this.tezejChar = character;
        }
    }
}
