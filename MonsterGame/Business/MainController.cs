using System;

namespace Business
{
    public class MainController
    {
        private static MainController singleton = null;
        private static GameLogic gameLogic = null;

        private MainController(IStore store)
        {
            gameLogic = new GameLogic(store);
        }

        public static void CreateInstance(IStore store)
        {
            if (singleton == null)
                singleton = new MainController(store);
            else
                throw new InvalidOperationException("Creating more than one instance of the store.");
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