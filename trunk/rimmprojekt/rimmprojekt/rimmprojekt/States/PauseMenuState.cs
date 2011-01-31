using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xen;
using Xen.Camera;
using Xen.Graphics;
using Xen.Ex.Graphics;
using Xen.Ex.Graphics2D;
using Xen.Ex.Geometry;
using Xen.Ex.Graphics.Content;
using Xen.Ex.Material;
using Xen.Input.State;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace rimmprojekt.States
{
    class PauseMenuState : IGameState, IContentOwner
    {
        private IGameStateManager stateManager;

        #region loading and background pic.
        private Boolean isLoadingElement;
        private float PlayTime;
        private float ActionTime;
        private Texture2D loadingTex;
        private TexturedElement loadingElement;
        private Texture2D alistarLol;
        private SolidColourElement solidColElement;
        private TexturedElement background;
        #endregion

        #region menu entries, keyboard input
        SpriteFont selectedFont;
        SpriteFont nonSelectedFont;
        int selectedEntry;
        Vector2 startPosition;
        Color selected;
        Color nonSelected;

        List<string> menuEntries = new List<string>();
        List<TextElement> menuEntryRect = new List<TextElement>();
        TextElement menuTitle;

        KeyboardState currentKeyboardState = new KeyboardState();
        KeyboardState previousKeyboardState = new KeyboardState();
        #endregion

        private Razredi.GameData gameData;

        #region input methods and menu operations
        private bool MenuUp
        {
            get { return IsNewPressedKey(Keys.Up); }
        }

        private bool MenuDown
        {
            get { return IsNewPressedKey(Keys.Down); }
        }

        private bool MenuSelect
        {
            get { return IsNewPressedKey(Keys.Enter); }
        }

        private bool MenuCancel
        {
            get { return IsNewPressedKey(Keys.Escape); }
        }

        private bool IsNewPressedKey(Keys key)
        {
            return previousKeyboardState.IsKeyUp(key) && currentKeyboardState.IsKeyDown(key);
        }

        private void HandleInput()
        {
            if (MenuUp)
            {
                selectedEntry--;
                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;
            }

            if (MenuDown)
            {
                selectedEntry++;
                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;
            }

            if (MenuSelect)
            {
                MenuSelectExecute(selectedEntry);
            }

            if (MenuCancel)
            {
                MenuCancelExecute();
            }
        }

        private void MenuSelectExecute(int selectedItem)
        {
            switch (selectedItem)
            {
                case 0: ResumeGame(); break;
                case 1: NewGame(); break;
                case 2: SaveGame(); break;
                case 3: GoToMainMenu(); break;
                case 4: QuitGame(); break;
            }
        }

        private void MenuCancelExecute()
        {
            ResumeGame();
        }

        private void ResumeGame()
        {
            stateManager.SetState(new PlayingState(gameData));
        }

        private void NewGame()
        {
            isLoadingElement = true;
        }

        private void SaveGame()
        {
            gameData.shrani();
        }

        private void GoToMainMenu()
        {
            this.stateManager.SetState(new MenuState());
        }

        private void QuitGame()
        {
            this.stateManager.Application.Shutdown();
        }
        #endregion

        public PauseMenuState(Razredi.GameData gameData)
        {
            this.gameData = gameData;
        }

        public void Initalise(IGameStateManager stateManager)
        {
            this.stateManager = stateManager;

            this.menuTitle = new TextElement();
            this.menuTitle.Text.SetText("Pause Menu");
            this.menuTitle.Position = new Vector2(10, 200);
            this.menuTitle.VerticalAlignment = VerticalAlignment.Centre;
            this.menuTitle.HorizontalAlignment = HorizontalAlignment.Left;
            this.menuTitle.Colour = Color.Yellow;

            #region menu entries
            menuEntries.Add("Resume Game");
            menuEntries.Add("New Game");
            menuEntries.Add("Save Game");
            //menuEntries.Add("Go To Main Menu");
            menuEntries.Add("Quit Game");

            selectedEntry = 0;
            selected = Color.Yellow;
            nonSelected = Color.White;

            startPosition = new Vector2(10, 100);

            for (int i = 0; i < menuEntries.Count; i++)
            {
                this.menuEntryRect.Add(new TextElement());
                this.menuEntryRect[i].Text.SetText(menuEntries[i]);
                this.menuEntryRect[i].Position = startPosition;
                this.menuEntryRect[i].VerticalAlignment = VerticalAlignment.Centre;
                this.menuEntryRect[i].HorizontalAlignment = HorizontalAlignment.Left;
                startPosition.Y -= 60;
            }
            #endregion

            #region background and loading
            loadingElement = new TexturedElement(new Vector2(718, 69));
            loadingElement.AlphaBlendState = AlphaBlendState.Additive;
            loadingElement.VerticalAlignment = VerticalAlignment.Bottom;
            loadingElement.HorizontalAlignment = HorizontalAlignment.Centre;
            background = new TexturedElement(new Vector2(1280, 720));
            solidColElement = new SolidColourElement(Color.Yellow, new Vector2(2, 2));
            isLoadingElement = false;
            PlayTime = 0.0f;
            ActionTime = 0.0f;
            #endregion

            //set the text font (using global content)
            stateManager.Application.Content.Add(this);
        }

        public void DrawScreen(DrawState state)
        {
            background.Draw(state);
            //display the 'menu' :-)
            if (isLoadingElement)
            {
                loadingElement.Draw(state);
                solidColElement.Draw(state);
            }
            else
            {
                menuTitle.Draw(state);

                for (int i = 0; i < menuEntries.Count; i++)
                {
                    bool isSelected = (i == selectedEntry);
                    Color color = isSelected ? selected : nonSelected;
                    SpriteFont font = isSelected ? selectedFont : nonSelectedFont;

                    this.menuEntryRect[i].Colour = color;
                    this.menuEntryRect[i].Font = font;
                    menuEntryRect[i].Draw(state);
                }
            }
        }

        public void Update(UpdateState state)
        {
            PlayTime += state.DeltaTimeSeconds;

            #region loading
            if (isLoadingElement)
            {
                ActionTime++;
                drawLoadingBar();
                if (Math.Round(ActionTime) == 339)
                {
                    PlayingState gameState = new PlayingState(this.stateManager.Application);
                    this.stateManager.SetState(gameState);
                }
            }
            #endregion

            Xen.Input.State.InputState input = state.PlayerInput[this.stateManager.PlayerIndex].InputState;

            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = state.KeyboardState;

            HandleInput();

            //if (state.KeyboardState.KeyState.Enter.OnReleased)
            //{
            //    isLoadingElement = true;
            //    return;
            //}

            //if (state.KeyboardState.KeyState.G.OnReleased)
            //{
            //    //start gallery display
            //    Gallery galleryState = new Gallery(this.stateManager.Application);

            //    //go to the loading state.
            //    this.stateManager.SetState(galleryState);
            //    return;
            //}

            //if (input.Buttons.Back.OnPressed)
            //{
            //    //quit when back is pressed in the menu
            //    this.stateManager.Application.Shutdown();
            //    return;
            //}
        }

        void IContentOwner.LoadContent(ContentState state)
        {
            nonSelectedFont = state.Load<SpriteFont>("MenuFont");
            selectedFont = state.Load<SpriteFont>("MenuFont1");

            this.menuTitle.Font = selectedFont;

            for (int i = 0; i < menuEntries.Count; i++)
            {
                //load the text font.
                this.menuEntryRect[i].Font = nonSelectedFont;
            }

            alistarLol = state.Load<Texture2D>("Textures/Lol_Alistar");
            loadingTex = state.Load<Texture2D>("Textures/Loading");
            background.Texture = alistarLol;
            loadingElement.Texture = loadingTex;
        }

        private void drawLoadingBar()
        {
            int timeSize = Int32.Parse(Math.Round((ActionTime / 10 * 100) / 5).ToString());
            solidColElement = new SolidColourElement(Color.Yellow, new Vector2(timeSize, 10));
            solidColElement.Position = new Vector2(296, 42);
        }
    }
}
