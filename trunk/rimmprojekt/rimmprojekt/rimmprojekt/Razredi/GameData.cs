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
using Microsoft.Xna.Framework.Media;

using JigLibX.Math;
using JigLibX.Physics;
using JigLibX.Geometry;
using JigLibX.Collision;

namespace rimmprojekt.Razredi
{
    class GameData
    {
        private Razredi.Tezej tezej;
        private Razredi.Inventory inventory;
        private Razredi.Enemy minotaver;
        private List<Razredi.Enemy> sovrazniki;
        private Razredi.Mapa mapa;

        #region get/set metode
        public Razredi.Tezej Tezej
        {
            get { return tezej; }
            set { tezej = value; }
        }

        public Razredi.Inventory Inventory
        {
            get { return inventory; }
            set { inventory = value; }
        }

        public Razredi.Enemy Minotaver
        {
            get { return minotaver; }
            set { minotaver = value; }
        }

        public List<Razredi.Enemy> Sovrazniki
        {
            get { return sovrazniki; }
            set { sovrazniki = value; }
        }

        public Razredi.Mapa Mapa
        {
            get { return mapa; }
            set { mapa = value; }
        }
        #endregion

        public GameData(States.IGameStateManager stateManager)
        {
            mapa = new Razredi.Mapa("../../../../rimmprojektContent/labirint1.txt", stateManager.Application.Content);

            List<Body> bodies = new List<Body>();
            foreach (Razredi.Kocka k in mapa.zidovi)
                bodies.Add(k.body);

            sovrazniki = generateEnemys(stateManager.Application.UpdateManager, stateManager.Application.Content, 7);
            minotaver = new Razredi.Enemy(71.0f, 0.0f, 80.0f, stateManager.Application.Content, "");

            tezej = new Razredi.Tezej(69.0f, 0.0f, 80.0f, stateManager.Application.UpdateManager, stateManager.Application.Content, bodies);
            inventory = new Razredi.Inventory(stateManager.Application.UpdateManager, stateManager.Application.Content, tezej);
        }

        public GameData(States.IGameStateManager stateManager, GameData gameData)
        {
            mapa = new Razredi.Mapa("../../../../rimmprojektContent/labirint1.txt", stateManager.Application.Content);

            List<Body> bodies = new List<Body>();
            foreach (Razredi.Kocka k in mapa.zidovi)
                bodies.Add(k.body);

            sovrazniki = gameData.Sovrazniki;
            minotaver = new Razredi.Enemy(20.0f, 0.0f, 20.0f, stateManager.Application.Content, "");

            tezej = new Razredi.Tezej(gameData.Tezej.polozaj.X, gameData.Tezej.polozaj.Y, gameData.Tezej.polozaj.Z, stateManager.Application.UpdateManager, stateManager.Application.Content, bodies);
            tezej.maxHealthPoints = gameData.Tezej.maxHealthPoints;
            tezej.maxManaPoints = gameData.Tezej.maxManaPoints;
            tezej.agility = gameData.Tezej.agility;
            tezej.expPoints = gameData.Tezej.expPoints;
            tezej.intelligence = gameData.Tezej.intelligence;
            tezej.strength = gameData.Tezej.strength;
            tezej.vitality = gameData.Tezej.vitality;
            inventory = gameData.Inventory;
        }

        public void Draw(DrawState state)
        {
            mapa.Draw(state);
            minotaver.Draw(state);
            foreach (Razredi.Enemy goblin in sovrazniki)
                goblin.Draw(state);
            tezej.Draw(state);
            inventory.Draw(state);
        }

        public void Update(UpdateState state)
        {
            tezej.Update(state);
            minotaver.Update(state);
            foreach (Razredi.Enemy goblin in sovrazniki)
                goblin.Update(state);
        }

        #region generateEnemies
        private List<Razredi.Enemy> generateEnemys(UpdateManager updateMng, ContentRegister contntrg, int count)
        {
            String skin_ = "goblin";
            List<Razredi.Enemy> result = new List<Razredi.Enemy>();
            for (int i = 0; i < count; i++)
            {
                Vector3 pozition = getEnemyPosition(i);
                Razredi.Enemy goblin = new Razredi.Enemy(pozition.X, pozition.Y, pozition.Z, contntrg, skin_);
                goblin.skin_value = "goblin";
                result.Add(goblin);
            }

            return result;
        }

        private Vector3 getEnemyPosition(Int32 i)
        {
            Random rndm = new Random();
            float[] x = new float[7];
            x[0] = float.Parse("143");
            x[1] = float.Parse("300");
            x[2] = float.Parse(rndm.Next(300, 498).ToString());
            x[3] = float.Parse("541");
            x[4] = float.Parse("581");
            x[5] = float.Parse(rndm.Next(422, 499).ToString());
            x[6] = float.Parse(rndm.Next(341, 388).ToString());

            float[] z = new float[7];
            z[0] = float.Parse(rndm.Next(380, 575).ToString());
            z[1] = float.Parse(rndm.Next(341, 575).ToString());
            z[2] = float.Parse("220");
            z[3] = float.Parse(rndm.Next(140, 299).ToString());
            z[4] = float.Parse(rndm.Next(303, 501).ToString());
            z[5] = float.Parse("501");
            z[6] = float.Parse("418");

            Vector3 result = new Vector3(x[i], 0.0f, z[i]);

            return result;
        }

        private Vector3 getRandomMinotaverPosition()
        {
            Random rndm = new Random();

            float[] x = new float[3];
            x[0] = float.Parse(rndm.Next(502, 577).ToString());
            x[1] = float.Parse(rndm.Next(501, 580).ToString());
            x[2] = float.Parse(rndm.Next(384, 581).ToString());

            float[] z = new float[3];
            z[0] = float.Parse("575");
            z[1] = float.Parse("504");
            z[2] = float.Parse("338");


            int tableIndex = rndm.Next(1, 3);

            return new Vector3(x[tableIndex], 0.0f, z[tableIndex]);
        }
        #endregion
    }
}
