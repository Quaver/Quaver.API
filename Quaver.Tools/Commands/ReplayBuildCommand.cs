/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using System.Linq;
using Quaver.API.Enums;
using Quaver.API.Replays;

namespace Quaver.Tools.Commands
{
    public class ReplayBuildCommand : Command
    {
        /// <summary>
        ///     The replay to be built
        /// </summary>
        private Replay Replay { get; }

        /// <summary>
        ///     The path that the built replay will be written to
        /// </summary>
        private string OutputPath { get; }

        /// <summary>
        /// <replay_path> <output_path> <quaver_version> <map_md5> <username> <timestamp> <mode> <mods> <score>
        /// <accuracy> <max_combo> <count_marv> <count_perf> <count_great> <count_good> <count_okay> <count_miss> <pause_count>
        /// </summary>
        /// <param name="args"></param>
        public ReplayBuildCommand(string[] args) : base(args)
        {
            Replay = new Replay(args[1], true)
            {
                QuaverVersion = args[3],
                MapMd5 = args[4],
                PlayerName = args[5],
                TimePlayed = long.Parse(args[6]),
                Mode = (GameMode) int.Parse(args[7]),
                Mods = (ModIdentifier) long.Parse(args[8]),
                Score = int.Parse(args[9]),
                Accuracy = float.Parse(args[10]),
                MaxCombo = int.Parse(args[11]),
                CountMarv = int.Parse(args[12]),
                CountPerf = int.Parse(args[13]),
                CountGreat = int.Parse(args[14]),
                CountGood = int.Parse(args[15]),
                CountOkay = int.Parse(args[16]),
                CountMiss = int.Parse(args[17]),
                PauseCount = int.Parse(args[18])
            };

            OutputPath = args[2];
        }

        public override void Execute() => Replay.Write(OutputPath);
    }
}
