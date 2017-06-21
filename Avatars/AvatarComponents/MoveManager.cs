using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avatars.AvatarComponents
{
    public static class MoveManager
    {
        #region Field Region

        private static Dictionary<string, IMove> allMoves = new Dictionary<string, IMove>();
        private static Random random = new Random();

        #endregion

        #region Property Region

        public static Random Random
        {
            get { return random; }
        }

        #endregion

        #region Constructor Region
        #endregion

        #region Method Region

        public static void FillMoves()
        {
            AddMove(new Tackle());
            AddMove(new Block());
            AddMove(new Haste());
            AddMove(new Bless());
            AddMove(new Curse());
            AddMove(new Heal());
            AddMove(new Flare());
            AddMove(new Shock());
            AddMove(new Gust());
            AddMove(new Frostbite());
            AddMove(new Shade());
            AddMove(new Burst());
            AddMove(new RockThrow());          
        }

        public static IMove GetMove(string name)
        {
            if (allMoves.ContainsKey(name))
                return (IMove)allMoves[name].Clone();

            return null;
        }

        public static void AddMove(IMove move)
        {
            if (!allMoves.ContainsKey(move.Name))
                allMoves.Add(move.Name, move);
        }

        #endregion
    }
}
