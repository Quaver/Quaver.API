using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Newtonsoft.Json.Linq;
using Quaver.API.Replays;

namespace Quaver.Tools.Commands
{
    public class ReplayCommand : Command
    {
        /// <summary>
        /// </summary>
        public Replay Replay { get; }

        /// <summary>
        /// </summary>
        /// <param name="args"></param>
        public ReplayCommand(string[] args) : base(args) => Replay = new Replay(args[1]);

        /// <summary>
        /// </summary>
        public override void Execute() => Console.WriteLine(JObject.FromObject(new
        {
            Replay.PlayerName,
            Replay.Md5,
            Replay.MapMd5,
            Replay.QuaverVersion,
            Date = Replay.Date.ToString(CultureInfo.InvariantCulture),
            Replay.Mods,
            Replay.Mode,
            Replay.TimePlayed,
            Replay.Score,
            Replay.Accuracy,
            Replay.MaxCombo,
            Replay.CountMarv,
            Replay.CountPerf,
            Replay.CountGreat,
            Replay.CountGood,
            Replay.CountOkay,
            Replay.CountMiss,
            Replay.PauseCount,
            Replay.HasData,
            Replay.Frames.Count
        }));
    }
}
