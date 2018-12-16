using Quaver.Tools.Commands;
using System;
using System.Diagnostics;

namespace Quaver.Tools
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            // TODO: temp
            var user = "admin";
            var commands = new string[][] {
                //new string[] { "-optimize" }
                
                new string[] { "-calcfolder", $"c:/users/{user}/desktop/testmaps/dan/tech", "None" },
                new string[] { "-calcfolder", $"c:/users/{user}/desktop/testmaps/dan/speed", "None" },
                new string[] { "-calcfolder", $"c:/users/{user}/desktop/testmaps/dan/jack", "None" },
                new string[] { "-calcfolder", $"c:/users/{user}/desktop/testmaps/dan/stamina", "None" },
                new string[] { "-calcfolder", $"c:/users/{user}/desktop/testmaps/dan/full-reform", "None" },
                new string[] { "-calcfolder", $"c:/users/{user}/desktop/testmaps/dan/full-old", "None" },
                new string[] { "-calcfolder", $"c:/users/{user}/desktop/testmaps", "None" }
                //new string[] { "-calcfolder", $"c:/Users/{user}/desktop/oss", "None" }
                
            };
            //args = new string[] { "-calcfolder", "c:/users/admin/desktop/qss/testmaps", "None" };
            Console.WriteLine("Calculating Difficulties...");
            var sw = new Stopwatch();

            sw.Start();
            if (commands.Length == 0)
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

            for (var i = 0; i < commands.Length; i++)
            switch (commands[i][0])
            {
                case "-optimize":
                    new OptimizeCommand(commands[i]).Execute();
                    break;
                case "-calcdiff":
                    new CalcDiffCommand(commands[i]).Execute();
                    break;
                case "-calcfolder":
                    new CalcFolderCommand(commands[i]).Execute();
                    break;
                case "-replay":
                    new ReplayCommand(commands[i]).Execute();
                    break;
                case "-virtualreplay":
                    new VirtualReplayPlayerCommand(commands[i]).Execute();
                    break;
                default:
                    throw new ArgumentException();
            }
            sw.Stop();

            Console.WriteLine($"TIME ELAPSED: {sw.Elapsed}");
        }
    }
}
