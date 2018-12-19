using Quaver.Tools.Commands;
using System;
using System.Diagnostics;

namespace Quaver.Tools
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            args = new string[]
            {
                "-calcfolder",
                "C:/Users/denys/Desktop/testmaps/dan/full-reform",
                "None"
            };
            var sw = new Stopwatch();
            sw.Start();

            if (args.Length == 0)
            {
                Console.WriteLine($"Welcome to the Quaver.API test bench.\n" +
                                  $"Here you can run a bunch of things to test the API.\n\n" +
                                  $"Commands:\n" +
                                  $"-calcdiff <file_path> <mods> `Calculate the difficulty of a map`\n" +
                                  $"-calcfolder <folder_path> <mods> `Calculate the difficulty of an entire folder`\n" +
                                  $"-replay <file_path.qr> (-headerless) `Read a replay file and retrieve information about it.`\n" +
                                  $"-virtualreplay <a.qr> <b.qua> <mods (int)> <-hl (optional to read headerless)> `Simulate a replay and retrieve its score outcome.`");
                return;
            }

            switch (args[0])
            {
                case "-optimize":
                    new OptimizeCommand(args).Execute();
                    break;
                case "-calcdiff":
                    new CalcDiffCommand(args).Execute();
                    break;
                case "-calcfolder":
                    new CalcFolderCommand(args).Execute();
                    break;
                case "-replay":
                    new ReplayCommand(args).Execute();
                    break;
                case "-virtualreplay":
                    new VirtualReplayPlayerCommand(args).Execute();
                    break;
                default:
                    throw new ArgumentException();
            }
            sw.Stop();

            Console.WriteLine($"Time Elapsed: {sw.Elapsed}");
        }
    }
}
