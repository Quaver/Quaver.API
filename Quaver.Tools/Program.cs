using System;
using System.Diagnostics;
using Quaver.Tools.Commands;

namespace Quaver.Tools
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            // TODO: temp
            var user = "denys";
            var test = new string[][] {
            new string[] { "-calcfolder", $"c:/users/{user}/desktop/testmaps/dan/tech", "None" },
            new string[] { "-calcfolder", $"c:/users/{user}/desktop/testmaps/dan/speed", "None" },
            new string[] { "-calcfolder", $"c:/users/{user}/desktop/testmaps/dan/jack", "None" },
            new string[] { "-calcfolder", $"c:/users/{user}/desktop/testmaps/dan/stamina", "None" },
            new string[] { "-calcfolder", $"c:/users/{user}/desktop/testmaps/dan/full-reform", "None" },
            new string[] { "-calcfolder", $"c:/users/{user}/desktop/testmaps/dan/full-old", "None" },
            new string[] { "-calcfolder", $"c:/users/{user}/desktop/testmaps/", "None" }
        };
            //args = new string[] { "-calcfolder", "c:/users/admin/desktop/qss/testmaps", "None" };
            var sw = new Stopwatch();

            sw.Start();
            if (test.Length == 0)
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

            for (var i = 0; i < test.Length; i++)
            switch (test[i][0])
            {
                case "-calcdiff":
                    new CalcDiffCommand(test[i]).Execute();
                    break;
                case "-calcfolder":
                    new CalcFolderCommand(test[i]).Execute();
                    break;
                case "-replay":
                    new ReplayCommand(test[i]).Execute();
                    break;
                case "-virtualreplay":
                    new VirtualReplayPlayerCommand(test[i]).Execute();
                    break;
                default:
                    throw new ArgumentException();
            }
            sw.Stop();

            Console.WriteLine($"TIME ELAPSED: {sw.Elapsed}");
        }
    }
}
