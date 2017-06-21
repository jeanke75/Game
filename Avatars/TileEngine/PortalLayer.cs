using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Avatars.TileEngine
{
    public class PortalLayer
    {
        #region Field Region

        private Dictionary<Rectangle, Portal> portals;

        #endregion

        #region Property Region

        [ContentSerializer]
        public Dictionary<Rectangle, Portal> Portals
        {
            get { return portals; }
            private set { portals = value; }
        }

        #endregion

        #region Constructor Region

        public PortalLayer()
        {
            portals = new Dictionary<Rectangle, Portal>();
        }

        #endregion
    }
}
