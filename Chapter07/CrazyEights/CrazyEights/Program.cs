using System;

namespace CrazyEights
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (CrazyEightsGame game = new CrazyEightsGame())
            {
                game.Run();
            }
        }
    }
}
