using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Avatars.AvatarComponents;
using Avatars.ConversationComponents;
using Avatars.Components;

namespace Avatars.GameStates
{
    public interface IBattleState
    {
        void SetAvatars(Avatar player, Avatar enemy);
        void StartBattle();
    }

    public class BattleState : BaseGameState, IBattleState
    {
        #region Field Region

        private Avatar player;
        private Avatar enemy;
        private GameScene combatScene;
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
        private SpriteFont font;
        private SpriteFont avatarFont;

        #endregion

        #region Property Region
        #endregion

        #region Constructor Region

        public BattleState(Game game)
            : base(game)
        {
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
            if (combatScene == null)
            {
                combatBackground = GameRef.Content.Load<Texture2D>(@"Scenes\scenebackground");

                avatarFont = GameRef.Content.Load<SpriteFont>(@"Fonts\scenefont");
                avatarBorder = GameRef.Content.Load<Texture2D>(@"Misc\avatarborder");
                avatarHealth = GameRef.Content.Load<Texture2D>(@"Misc\avatarhealth");

                font = GameRef.Content.Load<SpriteFont>(@"Fonts\gamefont");

                combatScene = new GameScene(GameRef, "", new List<SceneOption>());
            }

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            PlayerIndex? index = PlayerIndex.One;

            if (Xin.CheckKeyReleased(Keys.P))
                manager.PopState();

            combatScene.Update(gameTime, index.Value);

            if (Xin.CheckKeyReleased(Keys.Space) || Xin.CheckKeyReleased(Keys.Enter))
            {
                manager.PushState((DamageState)GameRef.DamageState, index);
                GameRef.DamageState.SetAvatars(player, enemy);

                IMove enemyMove = null;

                do
                {
                    int move = random.Next(0, enemy.KnownMoves.Count);
                    int i = 0;

                    foreach (string s in enemy.KnownMoves.Keys)
                    {
                        if (move == i)
                            enemyMove = (IMove)enemy.KnownMoves[s].Clone();
                        i++;
                    }

                } while (!enemyMove.Unlocked);

                GameRef.DamageState.SetMoves((IMove)player.KnownMoves[combatScene.OptionText].Clone(), enemyMove);
                GameRef.DamageState.Start();

                player.Update(gameTime);
                enemy.Update(gameTime);
            }

            Visible = true;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GameRef.SpriteBatch.Begin();

            combatScene.Draw(gameTime, GameRef.SpriteBatch, combatBackground, font);

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
            this.player = player;
            this.enemy = enemy;

            player.StartCombat();
            enemy.StartCombat();

            List<SceneOption> moves = new List<SceneOption>();

            if (combatScene == null)
                LoadContent();

            foreach (string s in player.KnownMoves.Keys)
            {
                SceneOption option = new SceneOption(s, s, new SceneAction());
                moves.Add(option);
            }

            combatScene.Options = moves;
        }

        public void StartBattle()
        {
            player.StartCombat();
            enemy.StartCombat();
            playerHealth = 100f;
            enemyHealth = 100f;
        }

        #endregion
    }
}
