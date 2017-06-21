using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avatars.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Avatars.ConversationComponents
{
    public class GameScene
    {
        #region Field Region

        protected Game game;
        protected string text;
        private List<SceneOption> options;
        private int selectedIndex;
        private Color highLight;
        private Color normal;
        private Vector2 textPosition;
        private static Texture2D selected;
        private bool isMouseOver;

        private Vector2 menuPosition = new Vector2(50, 475);

        #endregion

        #region Property Region

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public static Texture2D Selected
        {
            get { return selected; }
        }

        public List<SceneOption> Options
        {
            get { return options; }
            set { options = value; }
        }

        [ContentSerializerIgnore]
        public SceneAction OptionAction
        {
            get { return options[selectedIndex].OptionAction; }
        }

        public string OptionScene
        {
            get { return options[selectedIndex].OptionScene; }
        }

        public string OptionText
        {
            get { return options[selectedIndex].OptionText; }
        }

        public int SelectedIndex
        {
            get { return selectedIndex; }
        }

        public bool IsMouseOver
        {
            get { return isMouseOver; }
        }

        [ContentSerializerIgnore]
        public Color NormalColor
        {
            get { return normal; }
            set { normal = value; }
        }

        [ContentSerializerIgnore]
        public Color HighLightColor
        {
            get { return highLight; }
            set { highLight = value; }
        }

        public Vector2 MenuPosition
        {
            get { return menuPosition; }
        }

        #endregion

        #region Constructor Region
  
        private GameScene()
        {
            NormalColor = Color.Blue;
            HighLightColor = Color.Red;
        }

        public GameScene(string text, List<SceneOption> options)
            : this()
        {
            this.text = text;
            this.options = options;
            textPosition = Vector2.Zero;
        }

        public GameScene(Game game, string text, List<SceneOption> options)
            : this(text, options)
        {
            this.game = game;
        }

        #endregion

        #region Method Region

        public void SetText(string text, SpriteFont font)
        {
            textPosition = new Vector2(450, 50);

            StringBuilder sb = new StringBuilder();
            float currentLength = 0f;

            if (font == null)
            {
                this.text = text;
                return;
            }

            string[] parts = text.Split(' ');

            foreach (string s in parts)
            {
                Vector2 size = font.MeasureString(s);

                if (currentLength + size.X < 500f)
                {
                    sb.Append(s);
                    sb.Append(" ");
                    currentLength += size.X;
                }
                else
                {
                    sb.Append("\n\r");
                    sb.Append(s);
                    sb.Append(" ");
                    currentLength = 0;
                }
            }

            this.text = sb.ToString();
        }

        public void Initialize()
        {
        }

        public void Update(GameTime gameTime, PlayerIndex index)
        {
            if (Xin.CheckKeyReleased(Keys.Down))
            {
                selectedIndex--;
                if (selectedIndex < 0)
                    selectedIndex = options.Count - 1;
            }
            else if (Xin.CheckKeyReleased(Keys.Down))
            {
                selectedIndex++;
                if (selectedIndex > options.Count - 1)
                    selectedIndex = 0;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Texture2D background, SpriteFont font)
        {
            Vector2 selectedPosition = new Vector2();
            Rectangle portraitRect = new Rectangle(25, 25, 425, 425);
            Color myColor;

            if (selected == null)
                selected = game.Content.Load<Texture2D>(@"Misc\selected");

            if (textPosition == Vector2.Zero)
                SetText(text, font);

            if (background != null)
                spriteBatch.Draw(background, Vector2.Zero, Color.White);
            
            spriteBatch.DrawString(font,
                text,
                textPosition,
                Color.White);

            Vector2 position = menuPosition;

            Rectangle optionRect = new Rectangle(0, (int)position.Y, 1280, font.LineSpacing);
            isMouseOver = false;

            for (int i = 0; i < options.Count; i++)
            {
                if (optionRect.Contains(Xin.MouseState.Position))
                {
                    selectedIndex = i;
                    isMouseOver = true;
                }

                if (i == SelectedIndex)
                {
                    myColor = HighLightColor;
                    selectedPosition.X = position.X - 35;
                    selectedPosition.Y = position.Y;

                    spriteBatch.Draw(selected, selectedPosition, Color.White);
                }
                else
                    myColor = NormalColor;

                spriteBatch.DrawString(font,
                    options[i].OptionText,
                    position,
                    myColor);

                position.Y += font.LineSpacing + 5;
                optionRect.Y += font.LineSpacing + 5;
            }
        }

        #endregion
    }
}
