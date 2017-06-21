using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Avatars.TileEngine;
using Avatars.AvatarComponents;

namespace Avatars.PlayerComponents
{
    public class Player : DrawableGameComponent
    {
        #region Field Region

        protected Game1 gameRef;
        protected string name;
        protected bool gender;
        protected string mapName;
        protected Point tile;
        protected AnimatedSprite sprite;
        protected Texture2D texture;
        protected float speed = 180f;

        protected Vector2 position;

        protected Dictionary<string, Avatar> avatars = new Dictionary<string, Avatar>();
        private string currentAvatar;

        #endregion

        #region Property Region

        public Vector2 Position
        {
            get { return sprite.Position; }
            set { sprite.Position = value; }
        }

        public AnimatedSprite Sprite
        {
            get { return sprite; }
        }

        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        public virtual Avatar CurrentAvatar
        {
            get { return avatars[currentAvatar]; }
        }

        #endregion

        #region Constructor Region

        private Player(Game game)
            : base(game)
        {
        }

        public Player(Game game, string name, bool gender, Texture2D texture)
            : base(game)
        {
            gameRef = (Game1)game;
            this.name = name;
            this.gender = gender;

            this.texture = texture;
            this.sprite = new AnimatedSprite(texture, gameRef.PlayerAnimations);
            this.sprite.CurrentAnimation = AnimationKey.WalkDown;
        }

        #endregion

        #region Method Region

        public virtual void AddAvatar(string avatarName, Avatar avatar)
        {
            if (!avatars.ContainsKey(avatarName))
                avatars.Add(avatarName, avatar);            
        }

        public virtual Avatar GetAvatar(string avatarName)
        {
            if (avatars.ContainsKey(avatarName))
                return avatars[avatarName];

            return null;
        }

        public virtual void SetAvatar(string avatarName)
        {
            if (avatars.ContainsKey(avatarName))
                currentAvatar = avatarName;
            else
                throw new IndexOutOfRangeException();
        }

        public void SavePlayer()
        {
        }

        public static Player Load(Game game)
        {
            Player player = new Player(game);

            return player;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            sprite.Draw(gameTime, gameRef.SpriteBatch);
        }

        #endregion
    }
}
