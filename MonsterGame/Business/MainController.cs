using System;

namespace Business
{
    public class CoreController
    {
        private static CoreController single_instance = null;
        private static GameLogic gameLogic = null;

        private CoreController(IStore store)
        {
            gameLogic = new GameLogic(store);
        }

        public static void Build(IStore store)
        {
            if (single_instance == null)
                single_instance = new CoreController(store);
            else
                throw new InvalidOperationException("Building more than one instance of the store.");
        }

        public static GameLogic GameLogicInstance()
        {
            return gameLogic;
        }

        public static void Reset()
        {
            gameLogic = null;
        }
    }
}