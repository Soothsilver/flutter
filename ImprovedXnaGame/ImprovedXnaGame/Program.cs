using System;

namespace ImprovedXnaGame
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ImprovedGame game = new ImprovedGame())
            {
                game.Run();
            }
        }
    }
#endif
}

