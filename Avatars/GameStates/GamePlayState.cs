using System;
using Avatars.AvatarComponents;
using Avatars.CharacterComponents;
using Avatars.Components;
using Avatars.ConversationComponents;
using Avatars.MapComponents;
using Avatars.PlayerComponents;
using Avatars.TileEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Avatars.GameStates
{
    public interface IGamePlayState
    {
        void SetUpNewGame();
        void LoadExistingGame();
        void StartGame();
    }

    public class GamePlayState : BaseGameState, IGamePlayState
    {
        Engine engine = new Engine(Game1.ScreenRectangle, 64, 64);
        World world;
        Camera camera;
        Player player;

        public GamePlayState(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IGamePlayState), this);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 motion = Vector2.Zero;
            int cp = 8;

            if (Xin.KeyboardState.IsKeyDown(Keys.Z) && Xin.KeyboardState.IsKeyDown(Keys.Q))
            {
                motion.X = -1;
                motion.Y = -1;
                player.Sprite.CurrentAnimation = AnimationKey.WalkLeft;
            }
            else if (Xin.KeyboardState.IsKeyDown(Keys.Z) && Xin.KeyboardState.IsKeyDown(Keys.D))
            {
                motion.X = 1;
                motion.Y = -1;
                player.Sprite.CurrentAnimation = AnimationKey.WalkRight;
            }
            else if (Xin.KeyboardState.IsKeyDown(Keys.S) && Xin.KeyboardState.IsKeyDown(Keys.Q))
            {
                motion.X = -1;
                motion.Y = 1;
                player.Sprite.CurrentAnimation = AnimationKey.WalkLeft;
            }
            else if (Xin.KeyboardState.IsKeyDown(Keys.S) && Xin.KeyboardState.IsKeyDown(Keys.D))
            {
                motion.X = 1;
                motion.Y = 1;
                player.Sprite.CurrentAnimation = AnimationKey.WalkRight;
            }
            else if (Xin.KeyboardState.IsKeyDown(Keys.Z))
            {
                motion.Y = -1;
                player.Sprite.CurrentAnimation = AnimationKey.WalkUp;
            }
            else if (Xin.KeyboardState.IsKeyDown(Keys.S))
            {
                motion.Y = 1;
                player.Sprite.CurrentAnimation = AnimationKey.WalkDown;
            }
            else if (Xin.KeyboardState.IsKeyDown(Keys.Q))
            {
                motion.X = -1;
                player.Sprite.CurrentAnimation = AnimationKey.WalkLeft;
            }
            else if (Xin.KeyboardState.IsKeyDown(Keys.D))
            {
                motion.X = 1;
                player.Sprite.CurrentAnimation = AnimationKey.WalkRight;
            }

            if (motion != Vector2.Zero)
            {
                motion.Normalize();
                motion *= (player.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds);

                Rectangle pRect = new Rectangle(
                    (int)player.Sprite.Position.X + (int)motion.X + cp,
                    (int)player.Sprite.Position.Y + (int)motion.Y + cp,
                    Engine.TileWidth - cp,
                    Engine.TileHeight - cp);

                foreach (string s in world.CurrentMap.Characters.Keys)
                {
                    ICharacter c = GameRef.CharacterManager.GetCharacter(s);
                    Rectangle r = new Rectangle(
                        (int)world.CurrentMap.Characters[s].X * Engine.TileWidth + cp,
                        (int)world.CurrentMap.Characters[s].Y * Engine.TileHeight + cp,
                        Engine.TileWidth - cp,
                        Engine.TileHeight - cp);

                    if (pRect.Intersects(r))
                    {
                        motion = Vector2.Zero;
                        break;
                    }
                }

                Vector2 newPosition = player.Sprite.Position + motion;

                player.Sprite.Position = newPosition;
                player.Sprite.IsAnimating = true;
                player.Sprite.LockToMap(new Point(world.CurrentMap.WidthInPixels, world.CurrentMap.HeightInPixels));
            }
            else
            {
                player.Sprite.IsAnimating = false;
            }

            camera.LockToSprite(world.CurrentMap, player.Sprite, Game1.ScreenRectangle);
            player.Sprite.Update(gameTime);

            if (Xin.CheckKeyReleased(Keys.Space) || Xin.CheckKeyReleased(Keys.Enter))
            {
                foreach (string s in world.CurrentMap.Characters.Keys)
                {
                    ICharacter c = CharacterManager.Instance.GetCharacter(s);
                    float distance = Vector2.Distance(player.Sprite.Center, c.Sprite.Center);

                    if (Math.Abs(distance) < 72f)
                    {
                        IConversationState conversationState = (IConversationState)GameRef.Services.GetService(typeof(IConversationState));
                        manager.PushState(
                            (ConversationState)conversationState,
                            PlayerIndexInControl);

                        conversationState.SetConversation(player, c);
                        conversationState.StartConversation();
                    }
                }

                foreach (Rectangle r in world.CurrentMap.PortalLayer.Portals.Keys)
                {
                    Portal p = world.CurrentMap.PortalLayer.Portals[r];

                    float distance = Vector2.Distance(
                        player.Sprite.Center,
                        new Vector2(
                            r.X * Engine.TileWidth + Engine.TileWidth / 2,
                            r.Y * Engine.TileHeight + Engine.TileHeight / 2));

                    if (Math.Abs(distance) < 64f)
                    {
                        world.ChangeMap(p.DestinationLevel, new Rectangle(p.DestinationTile.X, p.DestinationTile.Y, 32, 32));

                        player.Position = new Vector2(
                            p.DestinationTile.X * Engine.TileWidth, 
                            p.DestinationTile.Y * Engine.TileHeight);
                        camera.LockToSprite(world.CurrentMap, player.Sprite, Game1.ScreenRectangle);

                        return;
                    }
                }
            }

            if (Xin.CheckKeyReleased(Keys.B))
            {
                foreach (string s in world.CurrentMap.Characters.Keys)
                {
                    ICharacter c = CharacterManager.Instance.GetCharacter(s);
                    float distance = Vector2.Distance(player.Sprite.Center, c.Sprite.Center);

                    if (Math.Abs(distance) < 72f && !c.Battled)
                    {
                        GameRef.BattleState.SetAvatars(player.CurrentAvatar, c.BattleAvatar);
                        manager.PushState(
                            (BattleState)GameRef.BattleState,
                            PlayerIndexInControl);
                        c.Battled = true;
                    }
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (world.CurrentMap != null && camera != null)
                world.CurrentMap.Draw(gameTime, GameRef.SpriteBatch, camera);

            GameRef.SpriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                null,
                null,
                null,
                camera.Transformation);

            player.Sprite.Draw(gameTime, GameRef.SpriteBatch);

            GameRef.SpriteBatch.End();
        }

        public void SetUpNewGame()
        {
            Texture2D spriteSheet = content.Load<Texture2D>(@"PlayerSprites\maleplayer");
            TileMap map = null;
            world = new World();

            player = new Player(GameRef, "Wesley", false, spriteSheet);
            player.AddAvatar("fire", AvatarManager.GetAvatar("fire"));
            player.SetAvatar("fire");

            ConversationManager.CreateConversations(GameRef);

            ICharacter teacherOne = Character.FromString(GameRef, "Lance,teacherone,WalkDown,teacherone,water");
            ICharacter teacherTwo = PCharacter.FromString(GameRef, "Marissa,teachertwo,WalkDown,tearchertwo,wind,earth");
            
            teacherOne.SetConversation("LanceHello");
            teacherTwo.SetConversation("MarissaHello");

            GameRef.CharacterManager.AddCharacter("teacherone", teacherOne);
            GameRef.CharacterManager.AddCharacter("teachertwo", teacherTwo);

            MapManager.FromBinFile("Town1", content);
            map = MapManager.GetMap("Town1");

            map.Characters.Add("teacherone", new Point(0, 4));
            
            map.PortalLayer.Portals.Add(new Rectangle(7, 3, 32, 32), new Portal(new Point(7, 3), new Point(4, 8), "Basement1"));

            world.AddMap("Town1", map);
            world.ChangeMap("Town1", Rectangle.Empty);

            MapManager.FromBinFile("Basement1", content);
            map = MapManager.GetMap("Basement1");

            map.Characters.Add("teachertwo", new Point(4, 1));

            map.PortalLayer.Portals.Add(new Rectangle(4, 9, 32, 32), new Portal(new Point(4, 9), new Point(7, 4), "Town1"));
            map.PortalLayer.Portals.Add(new Rectangle(5, 9, 32, 32), new Portal(new Point(5, 9), new Point(7, 4), "Town1"));

            world.AddMap("Basement1", map);

            camera = new Camera();
        }

        public void LoadExistingGame()
        {
        }

        public void StartGame()
        {
        }
    }
}