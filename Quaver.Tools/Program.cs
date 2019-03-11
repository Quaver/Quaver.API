/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using Quaver.Tools.Commands;
using System;
using System.Diagnostics;

namespace Quaver.Tools
{
    internal static class Program
    {
        internal static void Main(string[] args)
        {
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
                                  $"-virtualreplay <a.qr> <b.qua> <mods (int)> <-hl (optional to read headerless)> `Simulate a replay and retrieve its score outcome.`\n" +
                                  $"-buildreplay <replay_path> <output_path> <quaver_version> <map_md5> <timestamp> <mode> <mods> <score> <accuracy> <max_combo> <count_marv> <count_perf> <count_great> <count_good> <count_okay> <count_miss> <pause_count> <username>\n" +
                                  $"-calcrating <difficulty_rating> <accuracy>\n" +
                                  $"-convertosu <file_path.osu> <output.qua>\n" +
                                  $"-convertsm <file_path.sm> <output directory>\n" +
                                  $"-changequaids <file_path.qua> <output_path.qua> <map_id> <mapset_id>");
                return;
            }

            switch (args[0])
            {
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
                case "-buildreplay":
                    new ReplayBuildCommand(args).Execute();
                    break;
                case "-calcrating":
                    new CalcRatingCommand(args).Execute();
                    break;
                case "-convertosu":
                    new ConvertOsuCommand(args).Execute();
                    break;
                case "-convertsm":
                    new ConvertSmCommand(args).Execute();
                    break;
                case "-changequaids":
                    new ChangeQuaIdsCommand(args).Execute();
                    break;
                default:
                    throw new ArgumentException();
            }
            sw.Stop();

            Console.WriteLine($"Time Elapsed: {sw.Elapsed}");
        }
    }
}
