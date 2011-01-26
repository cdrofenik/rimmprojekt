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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace rimmprojekt.States
{
    interface IGameStateManager
    {
        Application Application { get; }
        PlayerIndex PlayerIndex { get; set; }

        //change to a different state
        void SetState(IGameState state);
    }

    interface IGameState
    {
        void Initalise(IGameStateManager stateManager);

        //simplified IDraw/IUpdate
        //NOTE:
        //For simplicity this only provides drawing directly to the screen.
        void DrawScreen(DrawState state);
        void Update(UpdateState state);
    }
}
