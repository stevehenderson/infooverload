using System;

namespace PilotTool
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (PilotTool game = new PilotTool(true))
            {
                game.Run();
            }
        }
    }
}

