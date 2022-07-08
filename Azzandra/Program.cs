using System;

namespace Azzandra
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        public static Engine Engine;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Engine = new Engine();
            Engine.Run();

            /*
            using (var game = new Engine())
                game.Run();
                */
        }
    }
#endif
}
