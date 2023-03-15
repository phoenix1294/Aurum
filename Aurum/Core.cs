using System;

namespace Aurum
{
    class Core
    {
        static void PrintL(object e)
        {
            Console.WriteLine(e);
        }

        static void Main(string[] args)
        {
            Console.Title = "Aurum by phoenix1294";

            if(args.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                PrintL("Без аргументов работать не будет!");
            }
            else
            {
                if(args[0] == "server")
                {
                    AurumServer aurumNode = new AurumServer(ushort.Parse(args[1]));
                    aurumNode.Run();
                }
                if(args[0] == "client")
                {
                    AurumClient aurumClient = new AurumClient(args[1], ushort.Parse(args[2]));
                    aurumClient.Run();
                }
                PrintL("Unknown command");
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\nPress key to exit...");
            Console.ReadKey(true);
        }
    }
}
