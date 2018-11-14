using System;

namespace Entities
{
    [Serializable]
    public class Survivor : Player
    {
        
        public Survivor() 
        {
            HP = 20;
            AP = 5;
            IsAlive = true;
        }

        public override string ToString()
        {
            return "Survivor";
        }

    }
    
}