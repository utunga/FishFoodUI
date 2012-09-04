using System;
using FishFood;

namespace FishFoodUI
{
    #if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            IStateSimulation coreState = new FishBowl();
            using (var game = new Game(coreState))
            {
                game.Run();
            }
        }
    }   
    #endif
}

