using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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
    class LoadGameState : IGameState, IContentOwner
    {
        IGameStateManager stateManager;
        SpriteFont selectedFont;
        SpriteFont nonSelectedFont;
        int selectedEntry;
        Vector2 startPosition;
        Color selected;
        Color nonSelected;

        List<string> menuEntries = new List<string>();
        List<TextElement> menuEntryRect = new List<TextElement>();

        private Texture2D alistarLol;
        private SolidColourElement solidColElement;
        private TexturedElement background;

        public void Initalise(IGameStateManager stateManager)
        {
            this.stateManager = stateManager;

            string[] filePaths = Directory.GetFiles(@"Content\SaveGames", "*.xml");
            foreach (string file in filePaths)
            {
                menuEntries.Add(file);
            }

            selectedEntry = 0;
            selected = Color.Yellow;
            nonSelected = Color.White;

            startPosition = new Vector2(-450, 200);

            for (int i = 0; i < menuEntries.Count; i++)
            {
                this.menuEntryRect.Add(new TextElement());
                this.menuEntryRect[i].Text.SetText(menuEntries[i]);
                this.menuEntryRect[i].Position = startPosition;
                this.menuEntryRect[i].VerticalAlignment = VerticalAlignment.Centre;
                this.menuEntryRect[i].HorizontalAlignment = HorizontalAlignment.Centre;
                startPosition.Y -= 60;
            }

            #region background and loading
            background = new TexturedElement(new Vector2(1280, 720));
            solidColElement = new SolidColourElement(Color.Yellow, new Vector2(2, 2));
            #endregion

            //set the text font (using global content)
            stateManager.Application.Content.Add(this);
        }

        void IContentOwner.LoadContent(ContentState state)
        {
            nonSelectedFont = state.Load<SpriteFont>("MenuFont");
            selectedFont = state.Load<SpriteFont>("MenuFont1");

            for (int i = 0; i < menuEntries.Count; i++)
            {
                //load the text font.
                this.menuEntryRect[i].Font = nonSelectedFont;
            }

            alistarLol = state.Load<Texture2D>("Textures/Lol_Alistar");
            background.Texture = alistarLol;
        }

        public void Update(UpdateState state)
        {
            if (state.KeyboardState.KeyState.Up.OnReleased)
            {
                selectedEntry--;
                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;
            }

            if (state.KeyboardState.KeyState.Down.OnReleased)
            {
                selectedEntry = (selectedEntry + 1) % menuEntries.Count;
            }

            if (state.KeyboardState.KeyState.Enter.OnPressed)
            {
                Razredi.GameData gd = new Razredi.GameData();
                stateManager.SetState(new PlayingState(gd.nalozi(menuEntries[selectedEntry])));
            }

            if (state.KeyboardState.KeyState.Escape.OnReleased)
                stateManager.SetState(new MenuState());
        }

        public void DrawScreen(DrawState state)
        {
            background.Draw(state);
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
}
