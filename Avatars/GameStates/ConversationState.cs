using Avatars.CharacterComponents;
using Avatars.Components;
using Avatars.ConversationComponents;
using Avatars.PlayerComponents;
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
    public interface IConversationState
    {
        void SetConversation(Player player, ICharacter character);
        void StartConversation();
    }

    public class ConversationState : BaseGameState, IConversationState
    {
        #region Field Region

        private Conversation conversation;
        private SpriteFont font;
        private Texture2D background;
        private Player player;
        private ICharacter speaker;

        #endregion

        #region Property Region
        #endregion

        #region Constructor Region

        public ConversationState(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IConversationState), this);
        }

        #endregion

        #region Method Region

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            font = GameRef.Content.Load<SpriteFont>(@"Fonts\scenefont");
            background = GameRef.Content.Load<Texture2D>(@"Scenes\scenebackground");
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (Xin.CheckKeyReleased(Keys.Space) || Xin.CheckKeyReleased(Keys.Enter))
            {
                switch (conversation.CurrentScene.OptionAction.Action)
                {
                    case ActionType.Buy :
                        break;
                    case ActionType.Change :
                        speaker.SetConversation(conversation.CurrentScene.OptionScene);
                        manager.PopState();
                        break;
                    case ActionType.End :
                        manager.PopState();
                        break;
                    case ActionType.GiveItems :
                        break;
                    case ActionType.GiveKey :
                        break;
                    case ActionType.Quest :
                        break;
                    case ActionType.Sell :
                        break;
                    case ActionType.Talk :
                        conversation.ChangeScene(conversation.CurrentScene.OptionScene);
                        break;
                }
            }

            conversation.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GameRef.SpriteBatch.Begin();
            conversation.Draw(gameTime, GameRef.SpriteBatch);
            GameRef.SpriteBatch.End();
        }

        public void SetConversation(Player player, ICharacter character)
        {
            this.player = player;
            speaker = character;

            if (ConversationManager.ConversationList.ContainsKey(character.Conversation))
                this.conversation = ConversationManager.ConversationList[character.Conversation];
            else
                manager.PopState();
        }

        public void StartConversation()
        {
            conversation.StartConversation();
        }

        #endregion
    }
}
