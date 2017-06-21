using Avatars.AvatarComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avatars.GameStates
{
    public enum CurrentTurn
    {
        Players, Enemies
    }

    public interface IDamageState
    {
        void SetAvatars(Avatar player, Avatar enemy);
        void SetMoves(IMove playerMove, IMove enemyMove);
        void Start();
    }

    public class DamageState : BaseGameState, IDamageState
    {
        #region Field Region

        private CurrentTurn turn;
        private Texture2D combatBackground;
        private SpriteFont avatarFont;
        private SpriteFont font;
        private Rectangle playerRect;
        private Rectangle enemyRect;
        private TimeSpan cTimer;
        private TimeSpan dTimer;
        private Avatar player;
        private Avatar enemy;
        private IMove playerMove;
        private IMove enemyMove;
        private bool first;
        private Rectangle playerBorderRect;
        private Rectangle enemyBorderRect;
        private Rectangle playerMiniRect;
        private Rectangle enemyMiniRect;
        private Rectangle playerHealthRect;
        private Rectangle enemyHealthRect;
        private Rectangle healthSourceRect;
        private float playerHealth;
        private float enemyHealth;
        private Texture2D avatarBorder;
        private Texture2D avatarHealth;
        private Vector2 playerName;
        private Vector2 enemyName;

        #endregion

        #region Property Region
        #endregion

        #region Constructor Region

        public DamageState(Game game)
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
            combatBackground = GameRef.Content.Load<Texture2D>(@"Scenes\scenebackground");

            avatarBorder = GameRef.Content.Load<Texture2D>(@"Misc\avatarborder");
            avatarHealth = GameRef.Content.Load<Texture2D>(@"Misc\avatarhealth");

            avatarFont = Game.Content.Load<SpriteFont>(@"Fonts\GameFont");
            font = Game.Content.Load<SpriteFont>(@"Fonts\scenefont");

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            PlayerIndex index;

            if ((cTimer > TimeSpan.FromSeconds(4) || !enemy.Alive || !player.Alive) && dTimer > TimeSpan.FromSeconds(3))
            {
                if (!enemy.Alive || !player.Alive)
                {
                    manager.PopState();
                    manager.PushState((BattleOverState)GameRef.BattleOverState, PlayerIndex.One);
                    GameRef.BattleOverState.SetAvatars(player, enemy);
                }
                else
                {
                    manager.PopState();
                }
            }
            else if (cTimer > TimeSpan.FromSeconds(2) && first && enemy.Alive && player.Alive)
            {
                first = false;
                dTimer = TimeSpan.Zero;
                if (turn == CurrentTurn.Players)
                {
                    turn = CurrentTurn.Enemies;
                    enemy.ResoleveMove(enemyMove, player);
                }
                else
                {
                    turn = CurrentTurn.Players;
                    player.ResoleveMove(playerMove, enemy);
                }
            }
            else if (cTimer == TimeSpan.Zero)
            {
                dTimer = TimeSpan.Zero;
                if (turn == CurrentTurn.Players)
                {
                    player.ResoleveMove(playerMove, enemy);
                }
                else
                {
                    enemy.ResoleveMove(enemyMove, player);
                }
            }

            cTimer += gameTime.ElapsedGameTime;
            dTimer += gameTime.ElapsedGameTime;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GameRef.SpriteBatch.Begin();

            GameRef.SpriteBatch.Draw(combatBackground, Vector2.Zero, Color.White);

            GameRef.SpriteBatch.Draw(player.Texture, playerRect, Color.White);
            GameRef.SpriteBatch.Draw(enemy.Texture, enemyRect, Color.White);

            Vector2 location = new Vector2(25, 475);

            if (turn == CurrentTurn.Players)
            {
                GameRef.SpriteBatch.DrawString(font, player.Name + " uses " + playerMove.Name + ".", location, Color.Black);

                if (playerMove.Target == Target.Enemy && playerMove.MoveType == MoveType.Attack)
                {
                    location.Y += avatarFont.LineSpacing;

                    if (Avatar.GetMoveModifier(playerMove.MoveElement, enemy.Element) < 1f)
                    {
                        GameRef.SpriteBatch.DrawString(font, "It is not very effective.", location, Color.Black);    
                    }
                    else if (Avatar.GetMoveModifier(playerMove.MoveElement, enemy.Element) > 1f)
                    {
                        GameRef.SpriteBatch.DrawString(font, "It is super effective.", location, Color.Black);
                    }
                }
            }
            else
            {
                GameRef.SpriteBatch.DrawString(font, "Enemy " + enemy.Name + " uses " + enemyMove.Name + ".", location, Color.Black);

                if (enemyMove.Target == Target.Enemy && playerMove.MoveType == MoveType.Attack)
                {
                    location.Y += avatarFont.LineSpacing;

                    if (Avatar.GetMoveModifier(enemyMove.MoveElement, player.Element) < 1f)
                    {
                        GameRef.SpriteBatch.DrawString(font, "It is not very effective.", location, Color.Black);
                    }
                    else if (Avatar.GetMoveModifier(enemyMove.MoveElement, player.Element) > 1f)
                    {
                        GameRef.SpriteBatch.DrawString(font, "It is super effective.", location, Color.Black);
                    }
                }
            }

            GameRef.SpriteBatch.Draw(avatarBorder, playerBorderRect, Color.White);
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

            if (player.GetSpeed() >= enemy.GetSpeed())
            {
                turn = CurrentTurn.Players;
            }
            else
            {
                turn = CurrentTurn.Enemies;
            }
        }

        public void SetMoves(IMove playerMove, IMove enemyMove)
        {
            this.playerMove = playerMove;
            this.enemyMove = enemyMove;
        }

        public void Start()
        {
            cTimer = TimeSpan.Zero;
            dTimer = TimeSpan.Zero;
            first = true;
        }

        #endregion
    }
}
