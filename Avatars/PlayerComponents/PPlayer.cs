using Avatars.AvatarComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avatars.PlayerComponents
{
    public class PPlayer : Player
    {
        public const int MaxAvatars = 4;

        private List<Avatar> battleAvatars = new List<Avatar>();
        private int currentAvatar;

        public override Avatar CurrentAvatar
        {
            get { return battleAvatars[currentAvatar]; }
        }

        public PPlayer(Game game, string name, bool gender, Texture2D texture)
            : base(game, name, gender, texture)
        {
        }

        public void SetCurrentAvatar(int index)
        {
            if (index < 0 || index > MaxAvatars)
                throw new IndexOutOfRangeException();

            currentAvatar = index;
        }

        public Avatar GetBattleAvatar(int index)
        {
            if (index < 0 || index > MaxAvatars)
                throw new IndexOutOfRangeException();

            return battleAvatars[index];
        }

        public void AddBattleAvatar(Avatar avatar)
        {
            if (battleAvatars.Count >= MaxAvatars - 1)
                throw new OverflowException();

            battleAvatars.Add(avatar);
        }

        public void RemoveBattleAvatar(int index)
        {
            if (index >= battleAvatars.Count)
                throw new IndexOutOfRangeException();

            battleAvatars.RemoveAt(index);
        }
    }
}
