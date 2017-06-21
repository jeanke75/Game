using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Avatars.AvatarComponents
{
    public enum AvatarElement
    {
        Dark, Earth, Fire, Light, Water, Wind
    }

    public class Avatar
    {
        #region Field Region

        private static Random random = new Random();
        private Texture2D texture;
        private string name;
        private AvatarElement element;
        private int level;
        private long experience;
        private int costToBuy;
        private int speed;
        private int attack;
        private int defense;
        private int health;
        private int currentHealth;
        private List<IMove> effects;
        private Dictionary<string, IMove> knownMoves;

        #endregion

        #region Property Region

        public string Name
        {
            get { return name; }
        }

        public int Level
        {
            get { return level; }
            set { level = (int)MathHelper.Clamp(value, 1, 100); }
        }

        public long Experience
        {
            get { return experience; }
        }

        public Texture2D Texture
        {
            get { return texture; }
        }

        public Dictionary<string, IMove> KnownMoves
        {
            get { return knownMoves; }
        }

        public AvatarElement Element
        {
            get { return element; }
        }

        public List<IMove> Effects
        {
            get { return effects; }
        }

        public static Random Random
        {
            get { return random; }
        }

        public int BaseAttack
        {
            get { return attack; }
        }

        public int BaseDefense
        {
            get { return defense; }
        }

        public int BaseSpeed
        {
            get { return speed; }
        }

        public int BaseHealth
        {
            get { return health; }
        }

        public int CurrentHealth
        {
            get { return currentHealth; }
        }

        public bool Alive
        {
            get { return (currentHealth > 0); }
        }

        #endregion

        #region Constructor Region

        private Avatar()
        {
            level = 1;
            knownMoves = new Dictionary<string, IMove>();
            effects = new List<IMove>();
        }

        #endregion

        #region Method region

        public void ResoleveMove(IMove move, Avatar target)
        {
            bool found = false;
            switch (move.Target)
            {
                case Target.Self:
                    if (move.MoveType == MoveType.Buff)
                    {
                        found = false;
                        for (int i = 0; i < effects.Count; i++)
                        {
                            if (effects[i].Name == move.Name)
                            {
                                effects[i].Duration += move.Duration;
                                found = true;
                            }
                        }

                        if (!found)
                            effects.Add((IMove)move.Clone());
                    }
                    else if (move.MoveType == MoveType.Heal)
                    {
                        currentHealth += move.Health;
                        if (currentHealth > health)
                            currentHealth = health;
                    }
                    else if (move.MoveType == MoveType.Status)
                    {
                    }

                    break;
                case Target.Enemy:
                    if (move.MoveType == MoveType.Debuff)
                    {
                        found = false;
                        for (int i = 0; i < target.Effects.Count; i++)
                        {
                            if (target.Effects[i].Name == move.Name)
                            {
                                target.Effects[i].Duration += move.Duration;
                                found = true;
                            }
                        }

                        if (!found)
                            target.Effects.Add((IMove)move.Clone());
                    }
                    else if (move.MoveType == MoveType.Attack)
                    {
                        float modifier = GetMoveModifier(move.MoveElement, target.Element);

                        float tDamage = GetAttack() + move.Health * modifier - target.GetDefense();

                        if (tDamage < 1f)
                            tDamage = 1f;

                        target.ApplyDamage((int)tDamage);
                    }

                    break;
            }
        }

        public static float GetMoveModifier(MoveElement moveElement, AvatarElement avatarElement)
        {
            float modifier = 1f;

            switch (moveElement)
            {
                case MoveElement.Dark:
                    if (avatarElement == AvatarElement.Light)
                        modifier += .25f;
                    else if (avatarElement == AvatarElement.Wind)
                        modifier -= .25f;
                    break;
                case MoveElement.Earth:
                    if (avatarElement == AvatarElement.Water)
                        modifier += .25f;
                    else if (avatarElement == AvatarElement.Wind)
                        modifier -= .25f;
                    break;
                case MoveElement.Fire:
                    if (avatarElement == AvatarElement.Wind)
                        modifier += .25f;
                    else if (avatarElement == AvatarElement.Water)
                        modifier -= .25f;
                    break;
                case MoveElement.Light:
                    if (avatarElement == AvatarElement.Dark)
                        modifier += .25f;
                    else if (avatarElement == AvatarElement.Earth)
                        modifier -= .25f;
                    break;
                case MoveElement.Water:
                    if (avatarElement == AvatarElement.Fire)
                        modifier += .25f;
                    else if (avatarElement == AvatarElement.Earth)
                        modifier -= .25f;
                    break;
                case MoveElement.Wind:
                    if (avatarElement == AvatarElement.Light)
                        modifier += .25f;
                    else if (avatarElement == AvatarElement.Earth)
                        modifier -= .25f;
                    break;

            }

            return modifier;
        }

        public void ApplyDamage(int tDamage)
        {
            currentHealth -= tDamage;
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                effects[i].Duration--;

                if (effects[i].Duration < 1)
                {
                    effects.RemoveAt(i);
                    i--;
                }
            }
        }

        public int GetAttack()
        {
            int attackMod = 0;

            foreach (IMove move in effects)
            {
                if (move.MoveType == MoveType.Buff)
                    attackMod += move.Attack;

                if (move.MoveType == MoveType.Debuff)
                    attackMod -= move.Attack;
            }

            return attack + attackMod;
        }

        public int GetDefense()
        {
            int defenseMod = 0;

            foreach (IMove move in effects)
            {
                if (move.MoveType == MoveType.Buff)
                    defenseMod += move.Defense;

                if (move.MoveType == MoveType.Debuff)
                    defenseMod -= move.Defense;
            }

            return defense + defenseMod;
        }

        public int GetSpeed()
        {
            int speedMod = 0;

            foreach (IMove move in effects)
            {
                if (move.MoveType == MoveType.Buff)
                    speedMod += move.Speed;
                if (move.MoveType == MoveType.Debuff)
                    speedMod -= move.Speed;
            }

            return speed + speedMod;
        }

        public int GetHealth()
        {
            int healthMod = 0;

            foreach (IMove move in effects)
            {
                if (move.MoveType == MoveType.Buff)
                    healthMod += move.Health;
                if (move.MoveType == MoveType.Debuff)
                    healthMod += move.Health;
            }

            return health + healthMod;
        }

        public void StartCombat()
        {
            effects.Clear();
            currentHealth = health;
        }

        public long WinBattle(Avatar target)
        {
            int levelDiff = target.Level - level;
            long expGained = 0;

            if (levelDiff <= -10)
            {
                expGained = 10;
            }
            else if (levelDiff <= -5)
            {
                expGained = (long)(100f * (float)Math.Pow(2, levelDiff));
            }
            else if (levelDiff <= 0)
            {
                expGained = (long)(50f * (float)Math.Pow(2, levelDiff));
            }
            else if (levelDiff <= 5)
            {
                expGained = (long)(5f * (float)Math.Pow(2, levelDiff));
            }
            else if (levelDiff <= 10)
            {
                expGained = (long)(10f * (float)Math.Pow(2, levelDiff));
            }
            else
            {
                expGained = (long)(50f * (float)Math.Pow(2, target.Level));
            }

            experience += expGained;
            return expGained;
        }

        public long LoseBattle(Avatar target)
        {
            long expGained = (long)((float)WinBattle(target) * .5f);
            experience += expGained;

            return expGained;
        }

        public bool CheckLevelUp()
        {
            bool leveled = false;

            if (experience >= 50 * (1 + (long)Math.Pow((level - 1), 2.5)))
            {
                leveled = true;
                level++;
            }

            return leveled;
        }

        public object Clone()
        {
            Avatar avatar = new Avatar();

            avatar.name = this.name;
            avatar.texture = this.texture;
            avatar.element = this.element;
            avatar.costToBuy = this.costToBuy;
            avatar.level = this.level;
            avatar.experience = this.experience;
            avatar.attack = this.attack;
            avatar.defense = this.defense;
            avatar.speed = this.speed;
            avatar.health = this.health;
            avatar.currentHealth = this.health;

            foreach (string s in this.knownMoves.Keys)
            {
                avatar.knownMoves.Add(s, this.knownMoves[s]);
            }

            return avatar;
        }

        #endregion

        public void AssignPoint(string s, int p)
        {
            switch (s)
            {
                case "Attack":
                    attack += p;
                    break;
                case "Defense":
                    defense += p;
                    break;
                case "Speed":
                    speed += p;
                    break;
                case "Health":
                    health += p;
                    break;
            }
        }

        public static Avatar FromString(string description, ContentManager content)
        {
            Avatar avatar = new Avatar();
            string[] parts = description.Split(',');

            avatar.name = parts[0];
            avatar.texture = content.Load<Texture2D>(@"AvatarImages\" + parts[0]);
            avatar.element = (AvatarElement)Enum.Parse(typeof(AvatarElement), parts[1]);
            avatar.costToBuy = int.Parse(parts[2]);
            avatar.level = int.Parse(parts[3]);
            avatar.attack = int.Parse(parts[4]);
            avatar.defense = int.Parse(parts[5]);
            avatar.speed = int.Parse(parts[6]);
            avatar.health = int.Parse(parts[7]);
            avatar.currentHealth = avatar.health;

            avatar.knownMoves = new Dictionary<string, IMove>();

            for (int i = 8; i < parts.Length; i++)
            {
                string[] moveParts = parts[i].Split(':');

                if (moveParts[0] != "None")
                {
                    IMove move = MoveManager.GetMove(moveParts[0]);
                    move.UnlockedAt = int.Parse(moveParts[1]);

                    if (move.UnlockedAt <= avatar.Level)
                        move.Unlock();

                    avatar.knownMoves.Add(move.Name, move);
                }
            }

            return avatar;
        }
    }
}
