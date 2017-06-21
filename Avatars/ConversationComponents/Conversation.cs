using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Avatars.ConversationComponents
{
    public class Conversation
    {
        #region Field Region

        private string name;
        private string firstScene;
        private string currentScene;
        private Dictionary<string, GameScene> scenes;
        private string backgroundName;
        private Texture2D background;
        private string fontName;
        private SpriteFont spriteFont;

        #endregion

        #region Property Region

        public string Name
        {
            get { return name; }
        }

        public string FirstScene
        {
            get { return firstScene; }
        }

        public GameScene CurrentScene
        {
            get { return scenes[currentScene]; }
        }

        public Dictionary<string, GameScene> Scenes
        {
            get { return scenes; }
        }

        public string BackgroundName
        {
            get { return backgroundName; }
            set { backgroundName = value; }
        }

        public Texture2D Background
        {
            get { return background; }
        }

        public SpriteFont SpriteFont
        {
            get { return spriteFont; }
        }

        public string FontName
        {
            get { return fontName; }
            set { fontName = value; }
        }

        #endregion

        #region Constructor Region

        public Conversation(string name, string firstScene, Texture2D background, SpriteFont font)
        {
            this.scenes = new Dictionary<string, GameScene>();
            this.name = name;
            this.firstScene = firstScene;
            this.background = background;
            this.spriteFont = font;
        }

        #endregion

        #region Method Region

        public void Update(GameTime gameTime)
        {
            CurrentScene.Update(gameTime, PlayerIndex.One);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            CurrentScene.Draw(gameTime, spriteBatch, background, spriteFont);
        }

        public void AddScene(string sceneName, GameScene scene)
        {
            if (!scenes.ContainsKey(sceneName))
                scenes.Add(sceneName, scene);
        }

        public GameScene GetScene(string sceneName)
        {
            if (scenes.ContainsKey(sceneName))
                return scenes[sceneName];

            return null;
        }

        public void StartConversation()
        {
            currentScene = firstScene;
        }

        public void ChangeScene(string sceneName)
        {
            currentScene = sceneName;
        }

        #endregion
    }
}
