using System;

namespace Entities
{
    [Serializable]
    public class Monster : Player
    {
        public Monster()
        {
            HP = 100;
            AP = 10;
            IsAlive = true;
        }

        public override string ToString()
        {
            return "Monster";
        }

    }
    
}