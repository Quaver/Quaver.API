using System;
using Quaver.API.Test.Commands;

namespace Quaver.API.Test
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine($"Welcome to the Quaver.API test bench.\n" +
                                    $"Here you can run a bunch of things to test the API.\n\n" +
                                    $"Commands:\n" +
                                    $"'-qua <file path.qua>' - Get information about a .qua file\n" +
                                    $"'-osu <file path.osu> <output.qua>' - Convert an osu! (.osu) beatmap to .qua");

                return;
            }
            
            switch (args[0])
            {
                case "-osu":
                    new OsuCommand(args).Execute();
                    break;
                case "-qua":
                    new QuaCommand(args).Execute();
                    break;
                default:
                    throw new ArgumentException();
            }
        }     
    }
}