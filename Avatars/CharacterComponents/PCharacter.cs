using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Avatars.AvatarComponents;
using Avatars.TileEngine;

namespace Avatars.CharacterComponents
{
    public class PCharacter : ICharacter
    {
        #region Constant

        public const float SpeakingRadius = 40f;
        public const int AvatarLimit = 6;

        #endregion

        #region Field Region

        private string name;
        private Avatar[] avatars = new Avatar[AvatarLimit];
        private int currentAvatar;
        private Avatar givingAvatar;
        private AnimatedSprite sprite;

        private string conversation;

        private static Game1 gameRef;
        private static Dictionary<AnimationKey, Animation> characterAnimations = new Dictionary<AnimationKey, Animation>();

        #endregion

        #region Property Region

        public string Name
        {
            get { return name; }
        }

        public AnimatedSprite Sprite
        {
            get { return sprite; }
        }

        public Avatar BattleAvatar
        {
            get { return avatars[currentAvatar]; }
        }

        public Avatar GiveAvatar
        {
            get { return givingAvatar; }
        }

        public string Conversation
        {
            get { return conversation; }
        }

        #endregion

        #region Constructor Region

        private PCharacter()
        {
        }

        #endregion

        #region Method Region

        private static void BuildAnimations()
        {
        }

        public static PCharacter FromString(Game game, string characterString)
        {
            if (gameRef == null)
                gameRef = (Game1)game;

            if (characterAnimations.Count == 0)
                BuildAnimations();

            PCharacter character = new PCharacter();
            string[] parts = characterString.Split(',');

            character.name = parts[0];
            Texture2D texture = game.Content.Load<Texture2D>(@"CharacterSprites\" + parts[1]);
            character.sprite = new AnimatedSprite(texture, gameRef.PlayerAnimations);

            AnimationKey key = AnimationKey.WalkDown;
            Enum.TryParse<AnimationKey>(parts[2], true, out key);

            character.sprite.CurrentAnimation = key;

            character.conversation = parts[3];

            for (int i = 4; i < 10 && i < parts.Length; i++)
                character.avatars[i - 4] = AvatarManager.GetAvatar(parts[i].ToLowerInvariant());

            return character;
        }

        public void ChangeAvatar(int index)
        {
            if (index < 0 || index >= AvatarLimit)
            {
                currentAvatar = index;
            }
        }

        public void SetConversation(string newConversation)
        {
            this.conversation = newConversation;
        }

        public static void Save(string characterName)
        {

        }

        public void Update(GameTime gameTime)
        {
            sprite.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            sprite.Draw(gameTime, spriteBatch);
        }

        #endregion


        public bool Battled
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
