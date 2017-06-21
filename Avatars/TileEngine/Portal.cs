using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Avatars.TileEngine
{
    public class Portal
    {
        #region Field Region

        Point sourceTile;
        Point destinationTile;
        string destinationLevel;

        #endregion

        #region Property Region

        [ContentSerializer]
        public Point SourceTile
        {
            get { return sourceTile; }
            private set { sourceTile = value; }
        }

        [ContentSerializer]
        public Point DestinationTile
        {
            get { return destinationTile; }
            private set { destinationTile = value; }
        }

        [ContentSerializer]
        public string DestinationLevel
        {
            get { return destinationLevel; }
            private set { destinationLevel = value; }
        }

        #endregion

        #region Constructor Region

        private Portal()
        {
        }

        public Portal(Point sourceTile, Point destinationTile, string destinationLevel)
        {
            SourceTile = sourceTile;
            DestinationTile = destinationTile;
            DestinationLevel = destinationLevel;
        }

        #endregion
    }
}
