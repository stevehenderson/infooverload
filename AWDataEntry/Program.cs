using System;

namespace AWDataEntry
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (AWDataEntry game = new AWDataEntry(true))
            {
                game.Run();
            }
        }
    }
}

