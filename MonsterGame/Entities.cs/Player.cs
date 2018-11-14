using System;

namespace Entities
{
    [Serializable]
    public abstract class Player
    {

        public Client Client { get; set; }

        public int  HP       { get; set; }

        public int  AP       { get; set; }

        public bool IsAlive  { get; set; }

        public int  NumOfActions { get; set; }

        public int NumOfAttacks { get; set; }

        public int NumOfMovements { get; set; }


        public int Score { get; set; }

        public Cell Position { get;set; }

        public Player(){
            NumOfAttacks = 0;
            NumOfMovements = 0;
            }

    }
    
}