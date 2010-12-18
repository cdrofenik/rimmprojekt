using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Xen;
using Xen.Camera;
using Xen.Graphics;
using Xen.Ex.Geometry;
using Xen.Ex.Material;
using Xen.Ex.Graphics2D;
using Xen.Ex.Graphics.Content;

namespace rimmprojekt
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    [DisplayName(Name = "RIMM")]
    public class Game1 : Application
    {
        private Camera3D camera;
        private DrawTargetScreen drawToScreen;
        private Xen.Ex.Graphics2D.Statistics.DrawStatisticsDisplay statisticsOverlay;


        //This method gets called just before the window is shown, and the device is created
        protected override void Initialise()
        {
            //create the draw target.
            Xen.Camera.FirstPersonControlledCamera3D camera = null;

            //it uses player input, so the UpdateManager must be passed in
            camera = new Xen.Camera.FirstPersonControlledCamera3D(this.UpdateManager);

            //in this case, we want the z-axis to be the up/down axis (otherwise it's the Y-axis)
            //camera.ZAxisUp = true;
            //also it's default is a bit too fast moving
            camera.MovementSensitivity *= 0.01f;
            camera.LookAt(new Vector3(24.0f, -20.0f, 0.0f), new Vector3(24.0f, 20.0f, 60.0f), Vector3.UnitY);

            this.camera = camera;
            drawToScreen = new DrawTargetScreen(camera);
            
            //Set the screen clear colour to blue
            //(Draw targets have a built in ClearBuffer object)
            drawToScreen.ClearBuffer.ClearColour = Color.CornflowerBlue;
            Window.Title = "RIMM";
            Window.AllowUserResizing = true;

            Razredi.Mapa mapa = new Razredi.Mapa("../../../../rimmprojektContent/labirint1.txt");
            drawToScreen.Add(mapa);

            statisticsOverlay = new Xen.Ex.Graphics2D.Statistics.DrawStatisticsDisplay(this.UpdateManager);
            drawToScreen.Add(statisticsOverlay);
        }

        //this is the default Update method.
        //Update() is called 60 times per second, which is the same rate that player input
        //is updated.
        //Note: Player input and Updating is explained in more detail in Tutorial 13
        protected override void Update(UpdateState state)
        {
            //quit when the back button is pressed (escape on the PC)
            if (state.PlayerInput[PlayerIndex.One].InputState.Buttons.Back.OnPressed)
                this.Shutdown();
        }

        //This is the main application draw method. All drawing code should go in here.
        //
        //Any drawing should be done through, or using the DrawState object.
        //Do not store a reference to a DrawState - if a method doesn't give access to it, you shouldn't be drawing in that method.
        //The most useful GraphicsDevice functionality is covered by the DrawState or other objects in Xen/Xen.Ex
        //The vast majority of applications shouldn't need to directly access the graphics device.
        protected override void Frame(FrameState state)
        {
            //perform the draw to the screen.
            drawToScreen.Draw(state);

            //at this point the screen has been drawn...
        }

        protected override void SetupGraphicsDeviceManager(GraphicsDeviceManager graphics, ref RenderTargetUsage presentation)
        {
            if (graphics != null) // graphics is null when starting within a WinForms host
            {
                graphics.PreferredBackBufferWidth = 800;
                graphics.PreferredBackBufferHeight = 450;
                graphics.PreferMultiSampling = true;
            }
        }

        protected override void InitialisePlayerInput(Xen.Input.PlayerInputCollection playerInput)
        {
            //if using keyboard/mouse, then centre the mouse each frame
            if (playerInput[PlayerIndex.One].ControlInput == Xen.Input.ControlInput.KeyboardMouse)
            {
                playerInput[PlayerIndex.One].InputMapper.CentreMouseToWindow = true;
            }
        }

        protected override void LoadContent(ContentState state)
        {
            //Load a normal XNA sprite font
            SpriteFont xnaSpriteFont = state.Load<SpriteFont>("Arial");

            //the statistics overlay requires the font is set
            this.statisticsOverlay.Font = xnaSpriteFont;
        }
    }
}
