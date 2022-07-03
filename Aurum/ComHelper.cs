using System;

namespace Aurum
{
    class ComHelper
    {
        private static int CurrentLogPosition = 2;

        private static void CalculateLog()
        {
            //Console.SetCursorPosition(0, CurrentLogPosition);
            //CurrentLogPosition++;
        }

        public static string GetReducedSize(int count)
        {
            if (count > 1048576)
            {
                return string.Format("{0} MB", Math.Round(count / 1048576f, 2));
            }
            if (count > 1024)
            {
                return string.Format("{0} KB", Math.Round(count / 1024f, 2));
            }
            return string.Format("{0} B", count);
        }

        public static void Log(object e)
        {
            CalculateLog();
            Console.ForegroundColor = ConsoleColor.White;
            PrintL(e);
        }

        public static void Succ(object e)
        {
            CalculateLog();
            Console.ForegroundColor = ConsoleColor.Green;
            PrintL(e);
        }

        public static void Err(object e)
        {
            CalculateLog();
            Console.ForegroundColor = ConsoleColor.Red;
            PrintL(e);
        }

        public static void Warn(object e)
        {
            CalculateLog();
            Console.ForegroundColor = ConsoleColor.Yellow;
            PrintL(e);
        }

        private static void PrintL(object e)
        {
            Console.WriteLine("[AURUM] {0}                ", e);
        }
    }
}
