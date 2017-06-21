using Avatars.AvatarComponents;
using Avatars.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avatars.GameStates
{
    public interface ILevelUpState
    {
        void SetAvatar(Avatar playerAvatar);
    }

    public class LevelUpState : BaseGameState, ILevelUpState
    {
        #region Field Region

        private Rectangle destination;
        private int points;
        private int selected;
        private SpriteFont font;
        private Avatar player;
        private Dictionary<string, int> attributes = new Dictionary<string, int>();
        private Dictionary<string, int> assignedTo = new Dictionary<string, int>();
        private Texture2D levelUpBackground;

        #endregion

        #region Property Region
        #endregion

        #region Constructor Region

        public LevelUpState(Game game)
            : base(game)
        {
            attributes.Add("Attack", 0);
            attributes.Add("Defense", 0);
            attributes.Add("Speed", 0);
            attributes.Add("Health", 0);
            attributes.Add("Done", 0);

            foreach (string s in attributes.Keys)
                assignedTo.Add(s, 0);
        }

        #endregion

        #region Method Region

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            levelUpBackground = GameRef.Content.Load<Texture2D>(
                @"GameScreens\levelup-menu");

            font = GameRef.Content.Load<SpriteFont>(@"Fonts\scenefont");

            destination = new Rectangle(
                (Game1.ScreenRectangle.Width - levelUpBackground.Width) / 2, 
                (Game1.ScreenRectangle.Height - levelUpBackground.Height) / 2, 
                levelUpBackground.Width, 
                levelUpBackground.Height);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            int i = 0;
            string attribute = "";

            if (Xin.CheckKeyReleased(Keys.Down))
            {
                selected++;

                if (selected >= attributes.Count)
                    selected = attributes.Count - 1;
            }
            else if (Xin.CheckKeyReleased(Keys.Up))
            {
                selected--;

                if (selected < 0)
                    selected = 0;
            }

            if (Xin.CheckKeyReleased(Keys.Space) || Xin.CheckKeyReleased(Keys.Enter))
            {
                if (selected == 4 && points == 0)
                {
                    foreach (string s in assignedTo.Keys)
                    {
                        player.AssignPoint(s, assignedTo[s]);
                    }

                    manager.PopState();
                    manager.PopState();
                    manager.PopState();
                    return;
                }
            }

            int increment = 1;

            if (Xin.CheckKeyReleased(Keys.Right) && points > 0)
            {
                foreach (string s in assignedTo.Keys)
                {
                    if (s == "Done")
                        return;

                    if (i == selected)
                    {
                        attribute = s;
                        break;
                    }

                    i++;
                }


                if (attribute == "Health")
                    increment *= 5;

                points--;
                assignedTo[attribute] += increment;

                if (points == 0)
                    selected = 4;
            }
            else if (Xin.CheckKeyReleased(Keys.Left) && points <= 3)
            {
                foreach (string s in assignedTo.Keys)
                {
                    if (s == "Done")
                        return;

                    if (i == selected)
                    {
                        attribute = s;
                        break;
                    }

                    i++;
                }

                if (assignedTo[attribute] != attributes[attribute])
                {
                    if (attribute == "Health")
                        increment *= 5;

                    points++;
                    assignedTo[attribute] -= increment;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);


            GameRef.SpriteBatch.Begin();
            GameRef.SpriteBatch.Draw(levelUpBackground, destination, Color.White);

            Vector2 textPosition = new Vector2(destination.X + 5, destination.Y + 5);

            GameRef.SpriteBatch.DrawString(font, player.Name, textPosition, Color.Black);
            textPosition.Y += font.LineSpacing * 2;

            int i = 0;

            foreach (string s in attributes.Keys)
            {
                Color tint = Color.Black;

                if (i == selected)
                    tint = Color.Red;

                if (s != "Done")
                {
                    GameRef.SpriteBatch.DrawString(font, s + ":", textPosition, tint);
                    textPosition.X += 125;

                    GameRef.SpriteBatch.DrawString(font, attributes[s].ToString(), textPosition, tint);
                    textPosition.X += 40;

                    GameRef.SpriteBatch.DrawString(font, assignedTo[s].ToString(), textPosition, tint);
                    textPosition.X = destination.X + 5;

                    textPosition.Y += font.LineSpacing;
                }
                else
                {
                    GameRef.SpriteBatch.DrawString(font, "Done", textPosition, tint);
                    textPosition.Y += font.LineSpacing * 2;
                }
                i++;
            }

            GameRef.SpriteBatch.DrawString(font, points.ToString() + " point left.", textPosition, Color.Black);
            GameRef.SpriteBatch.End();
        }

        public void SetAvatar(Avatar playerAvatar)
        {
            player = playerAvatar;

            attributes["Attack"] = player.BaseAttack;
            attributes["Defense"] = player.BaseDefense;
            attributes["Speed"] = player.BaseSpeed;
            attributes["Health"] = player.BaseHealth;

            assignedTo["Attack"] = player.BaseAttack;
            assignedTo["Defense"] = player.BaseDefense;
            assignedTo["Speed"] = player.BaseSpeed;
            assignedTo["Health"] = player.BaseHealth;

            points = 3;
            selected = 0;
        }

        #endregion
    }
}
