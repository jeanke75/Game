using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Avatars.TileEngine
{
    public class World
    {
        #region Field Region

        private Dictionary<string, TileMap> maps;
        private string currentMapName;

        #endregion

        #region Property region

        [ContentSerializer]
        public Dictionary<string, TileMap> Maps
        {
            get { return maps; }
            private set { maps = value; }
        }

        [ContentSerializer]
        public string CurrentMapName
        {
            get { return currentMapName; }
            private set { currentMapName = value; }
        }

        public TileMap CurrentMap
        {
            get { return maps[currentMapName]; }
        }

        #endregion

        #region Constructor Region

        public World()
        {
            maps = new Dictionary<string, TileMap>();
        }

        #endregion

        #region Method Region

        public void AddMap(string mapName, TileMap map)
        {
            if (!maps.ContainsKey(mapName))
                maps.Add(mapName, map);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            CurrentMap.Draw(gameTime, spriteBatch, camera);
        }

        public void ChangeMap(string mapName, Rectangle portalLocation)
        {
            if (maps.ContainsKey(mapName))
            {
                currentMapName = mapName;
                return;
            }

            throw new Exception("Map name or portal name not found.");
        }

        #endregion
    }
}
