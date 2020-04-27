/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Quaver.API.Enums;
using Quaver.API.Helpers;
using Quaver.API.Maps;
using Quaver.API.Maps.Parsers;
using Quaver.API.Replays;
using Quaver.API.Replays.Virtual;

namespace Quaver.Tools.Commands
{
    public class VirtualReplayPlayerCommand : Command
    {
        /// <summary>
        /// </summary>
        public Replay Replay { get; }

        /// <summary>
        /// </summary>
        public Qua Map { get; }

        /// <summary>
        /// </summary>
        public string MapMd5 { get; }

        /// <summary>
        /// </summary>
        /// <param name="args"></param>
        public VirtualReplayPlayerCommand(string[] args) : base(args)
        {
            var readHeaderLess = false;

            if (args.Length == 5)
            {
                if (args[4] == "-hl")
                    readHeaderLess = true;
            }

            Replay = new Replay(args[1], readHeaderLess) { Mods = (ModIdentifier) long.Parse(args[3]) };

            if (args[2].EndsWith(".qua"))
                Map = Qua.Parse(args[2]);
            else if (args[2].EndsWith(".osu"))
                Map = new OsuBeatmap(args[2]).ToQua();

            MapMd5 = CryptoHelper.FileToMd5(args[2]);

            if (MapMd5 != Replay.MapMd5 && !readHeaderLess)
                throw new ArgumentException("The specified replay doesn't match the map.");
        }

        /// <summary>
        /// </summary>
        public override void Execute()
        {
            var virtualPlayer = new VirtualReplayPlayer(Replay, Map);
            virtualPlayer.Replay.Frames.ForEach(x => virtualPlayer.PlayNextFrame());

            var hits = new List<string>();

            foreach (var stat in virtualPlayer.ScoreProcessor.Stats)
            {
                var val = $"{stat.HitDifference}{(stat.HitObject.IsLongNote ? "L" : "")}";
                hits.Add(val);
            }

            Console.WriteLine(JObject.FromObject(new
            {
                Map = new
                {
                    Md5 = MapMd5,
                    Map.Artist,
                    Map.Title,
                    Map.DifficultyName,
                    Map.Mode,
                    Map.Creator,
                    Map.Length,
                    Map.MapId,
                    Map.MapSetId,
                    ObjectCount = Map.HitObjects.Count,
                    NormalObjectCount = Map.HitObjects.FindAll(x => !x.IsLongNote).Count,
                    LongNoteCount = Map.HitObjects.FindAll(x => x.IsLongNote).Count,
                    KeyCount = Map.GetKeyCount(),
                },
                Replay = new
                {
                    Replay.Md5,
                    Replay.MapMd5,
                    Replay.ReplayVersion,
                    Replay.PlayerName,
                    Replay.Date,
                    Replay.TimePlayed,
                    Replay.Mode,
                    Replay.Mods,
                    Replay.PauseCount,
                    Replay.HasData,
                    FrameCount = Replay.Frames.Count,
                    Length = Replay.Frames.Last().Time,
                    Replay.Score,
                    Replay.Accuracy,
                    Replay.MaxCombo,
                    Replay.CountMarv,
                    Replay.CountPerf,
                    Replay.CountGreat,
                    Replay.CountGood,
                    Replay.CountOkay,
                    Replay.CountMiss
                },
                VirtualReplayPlayer = new
                {
                    virtualPlayer.ScoreProcessor.Score,
                    virtualPlayer.ScoreProcessor.Accuracy,
                    virtualPlayer.ScoreProcessor.MaxCombo,
                    CountMarv = virtualPlayer.ScoreProcessor.CurrentJudgements[Judgement.Marv],
                    CountPerf = virtualPlayer.ScoreProcessor.CurrentJudgements[Judgement.Perf],
                    CountGreat = virtualPlayer.ScoreProcessor.CurrentJudgements[Judgement.Great],
                    CountGood = virtualPlayer.ScoreProcessor.CurrentJudgements[Judgement.Good],
                    CountOkay = virtualPlayer.ScoreProcessor.CurrentJudgements[Judgement.Okay],
                    CountMiss = virtualPlayer.ScoreProcessor.CurrentJudgements[Judgement.Miss],
                    Hits = hits
                },
            }));
        }
    }
}
