using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            Console.Title = "Aurum Project by phoenix1294";

            if(args.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                PrintL("Без аргументов работать не будет!");
                Console.ForegroundColor = ConsoleColor.Yellow;
                PrintL("Краткий ликбез:");
                PrintL("      1. Нужно открыть CMD или PS (по вкусу) где лежит сборка");
                PrintL("      2. И написать что-то вроде 'au rofelsoft.ru 80'");
                PrintL("      'rofelsoft.ru' -> адрес нужного Aurum узла (может быть любым)");
                PrintL("      '80' -> порт, который необходимо открыть (0-65536)");
                PrintL("");
                PrintL("      Можно стать узлом, но необходимы следующие условия:");
                PrintL("      Открытые порты и белый IP, иначе работа не гарантируется");
                Console.ForegroundColor = ConsoleColor.Yellow;
                PrintL("      Для этого делаем пункт 1 и пишем 'au node 8'");
                PrintL("      '8' -> максимальное количество клиентских узлов (0-255)");

                Console.ForegroundColor = ConsoleColor.Red;
                PrintL("      МОЖНО ОТКРЫТЬ УЗЕЛ ЧЕРЕЗ ЧУЖОЙ УЗЕЛ");
            }
            else
            {
                if(args[0] == "node")
                {
                    AurumNode aurumNode = new AurumNode(byte.Parse(args[1]), ushort.Parse(args[2]));
                    aurumNode.Run();
                }
                else
                {
                    AurumClient aurumClient = new AurumClient(args[0], ushort.Parse(args[1]));
                    aurumClient.Run();
                }
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.Write("Клацни любую клавишу чтобы закрыть");
            Console.ReadKey(true);
        }
    }
}
