using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Avatars.AvatarComponents;
using Avatars.GameStates;
using Avatars.Components;

namespace Avatars.GameStates
{
    public interface IBattleOverState
    {
        void SetAvatars(Avatar player, Avatar enemy);
    }

    public class BattleOverState : BaseGameState, IBattleOverState
    {
        #region Field Region

        private Avatar player;
        private Avatar enemy;
        private Texture2D combatBackground;
        private Rectangle playerRect;
        private Rectangle enemyRect;
        private Rectangle playerBorderRect;
        private Rectangle enemyBorderRect;
        private Rectangle playerMiniRect;
        private Rectangle enemyMiniRect;
        private Rectangle playerHealthRect;
        private Rectangle enemyHealthRect;
        private Rectangle healthSourceRect;
        private Vector2 playerName;
        private Vector2 enemyName;
        private float playerHealth;
        private float enemyHealth;
        private Texture2D avatarBorder;
        private Texture2D avatarHealth;
        private SpriteFont avatarFont;
        private SpriteFont font;
        private string[] battleState;
        private Vector2 battlePosition;
        private bool levelUp;

        #endregion

        #region Property Region
        #endregion

        #region Constructor Region

        public BattleOverState(Game game)
            : base(game)
        {
            battleState = new string[3];

            battleState[0] = "The battle was won!";
            battleState[1] = " gained ";
            battleState[2] = "Continue";

            battlePosition = new Vector2(25, 475);

            playerRect = new Rectangle(10, 90, 300, 300);
            enemyRect = new Rectangle(Game1.ScreenRectangle.Width - 310, 10, 300, 300);

            playerBorderRect = new Rectangle(10, 10, 300, 75);
            enemyBorderRect = new Rectangle(Game1.ScreenRectangle.Width - 310, 320, 300, 75);

            healthSourceRect = new Rectangle(10, 50, 290, 20);
            playerHealthRect = new Rectangle(playerBorderRect.X + 12, playerBorderRect.Y + 52, 286, 16);
            enemyHealthRect = new Rectangle(enemyBorderRect.X + 12, enemyBorderRect.Y + 52, 286, 16);

            playerMiniRect = new Rectangle(playerBorderRect.X + 11, playerBorderRect.Y + 11, 28, 28);
            enemyMiniRect = new Rectangle(enemyBorderRect.X + 11, enemyBorderRect.Y + 11, 28, 28);

            playerName = new Vector2(playerBorderRect.X + 55, playerBorderRect.Y + 5);
            enemyName = new Vector2(enemyBorderRect.X + 55, enemyBorderRect.Y + 5);
        }

        #endregion

        #region Method Region

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            combatBackground = GameRef.Content.Load<Texture2D>(@"Scenes\scenebackground");

            avatarFont = GameRef.Content.Load<SpriteFont>(@"Fonts\GameFont");
            avatarBorder = GameRef.Content.Load<Texture2D>(@"Misc\avatarborder");
            avatarHealth = GameRef.Content.Load<Texture2D>(@"Misc\avatarhealth");

            font = GameRef.Content.Load<SpriteFont>(@"Fonts\scenefont");

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            PlayerIndex? index = PlayerIndex.One;

            if (Xin.CheckKeyReleased(Keys.Space) || Xin.CheckKeyReleased(Keys.Enter))
            {
                if (levelUp)
                {                    
                    manager.PushState((LevelUpState)GameRef.LevelUpState, PlayerIndexInControl);
                    GameRef.LevelUpState.SetAvatar(player);

                    this.Visible = true;
                }
                else
                {
                    manager.PopState();
                    manager.PopState();
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 position = battlePosition;

            base.Draw(gameTime);

            GameRef.SpriteBatch.Begin();

            GameRef.SpriteBatch.Draw(combatBackground, Vector2.Zero, Color.White);

            for (int i = 0; i < 2; i++)
            {
                GameRef.SpriteBatch.DrawString(font, battleState[i], position, Color.Black);
                position.Y += avatarFont.LineSpacing;
            }

            GameRef.SpriteBatch.DrawString(font, battleState[2], position, Color.Red);

            GameRef.SpriteBatch.Draw(player.Texture, playerRect, Color.White);
            GameRef.SpriteBatch.Draw(enemy.Texture, enemyRect, Color.White);

            GameRef.SpriteBatch.Draw(avatarBorder, playerBorderRect, Color.White);

            playerHealth = (float)player.CurrentHealth / (float)player.GetHealth();
            MathHelper.Clamp(playerHealth, 0f, 1f);
            playerHealthRect.Width = (int)(playerHealth * 286);

            GameRef.SpriteBatch.Draw(avatarHealth, playerHealthRect, healthSourceRect, Color.White);

            GameRef.SpriteBatch.Draw(avatarBorder, enemyBorderRect, Color.White);

            enemyHealth = (float)enemy.CurrentHealth / (float)enemy.GetHealth();
            MathHelper.Clamp(enemyHealth, 0f, 1f);
            enemyHealthRect.Width = (int)(enemyHealth * 286);

            GameRef.SpriteBatch.Draw(avatarHealth, enemyHealthRect, healthSourceRect, Color.White);
            GameRef.SpriteBatch.DrawString(avatarFont, player.Name, playerName, Color.White);
            GameRef.SpriteBatch.DrawString(avatarFont, enemy.Name, enemyName, Color.White);

            GameRef.SpriteBatch.Draw(player.Texture, playerMiniRect, Color.White);
            GameRef.SpriteBatch.Draw(enemy.Texture, enemyMiniRect, Color.White);

            GameRef.SpriteBatch.End();
        }

        public void SetAvatars(Avatar player, Avatar enemy)
        {
            levelUp = false;

            long expGained = 0;

            this.player = player;
            this.enemy = enemy;

            if (player.Alive)
            {
                expGained = player.WinBattle(enemy);
                battleState[0] = player.Name + " has won the battle!";
                battleState[1] = player.Name + " has gained " + expGained + " experience";

                if (player.CheckLevelUp())
                {
                    battleState[1] += " and gained a level!";

                    foreach (string s in player.KnownMoves.Keys)
                    {
                        if (player.KnownMoves[s].Unlocked == false && player.Level >= player.KnownMoves[s].UnlockedAt)
                        {
                            player.KnownMoves[s].Unlock();
                            battleState[1] += " " + s + " was unlocked!";
                        }
                    }

                    levelUp = true;
                }
                else
                {
                    battleState[1] += ".";
                }
            }
            else
            {
                expGained = player.LoseBattle(enemy);

                battleState[0] = player.Name + " has lost the battle.";
                battleState[1] = player.Name + " has gained " + expGained + " experience";

                if (player.CheckLevelUp())
                {
                    battleState[1] += " and gained a level!";

                    foreach (string s in player.KnownMoves.Keys)
                    {
                        if (player.KnownMoves[s].Unlocked == false && player.Level >= player.KnownMoves[s].UnlockedAt)
                        {
                            player.KnownMoves[s].Unlock();
                            battleState[1] += " " + s + " was unlocked!";
                        }
                    }

                    levelUp = true;
                }
                else
                {
                    battleState[1] += ".";
                }
            }
        }

        #endregion
    }
}
