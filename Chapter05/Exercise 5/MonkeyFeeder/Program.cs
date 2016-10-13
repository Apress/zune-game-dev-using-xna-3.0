using System;

namespace MonkeyFeeder
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (MonkeyFeederGame game = new MonkeyFeederGame())
            {
                game.Run();
            }
        }
    }
}
