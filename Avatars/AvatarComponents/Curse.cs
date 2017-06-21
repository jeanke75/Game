using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avatars.AvatarComponents
{
    public class Curse : IMove
    {
        #region Field Region

        private string name;
        private Target target;
        private MoveType moveType;
        private MoveElement moveElement;
        private Status status;
        private int unlockedAt;
        private bool unlocked;
        private int duration;
        private int attack;
        private int defense;
        private int speed;
        private int health;

        #endregion

        #region Property Region

        public string Name
        {
            get { return name; }
        }

        public Target Target
        {
            get { return target; }
        }

        public MoveType MoveType
        {
            get { return moveType; }
        }

        public MoveElement MoveElement
        {
            get { return moveElement; }
        }

        public Status Status
        {
            get { return status; }
        }

        public int UnlockedAt
        {
            get { return unlockedAt; }
            set { unlockedAt = value; }
        }

        public bool Unlocked
        {
            get { return unlocked; }
        }

        public int Duration
        {
            get { return duration; }
            set { duration = value; }
        }

        public int Attack
        {
            get { return attack; }
        }

        public int Defense
        {
            get { return defense; }
        }

        public int Speed
        {
            get { return speed; }
        }

        public int Health
        {
            get { return health; }
        }

        #endregion

        #region Constructor Region

        public Curse()
        {
            name = "Curse";
            target = Target.Enemy;
            moveType = MoveType.Debuff;
            moveElement = MoveElement.Dark;
            status = Status.Normal;
            unlocked = false;
            duration = 5;
            attack = Avatar.Random.Next(2, 6);
            defense = Avatar.Random.Next(2, 6);
            speed = Avatar.Random.Next(2, 6);
            health = Avatar.Random.Next(0, 0);
        }

        #endregion

        #region Method Region

        public void Unlock()
        {
            unlocked = true;
        }

        public object Clone()
        {
            Curse curse = new Curse();
            curse.unlocked = this.unlocked;
            curse.unlockedAt = this.unlockedAt;
            return curse;
        }

        #endregion
    }
}
