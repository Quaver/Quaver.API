using System;
using System.IO;
using Quaver.API.Enums;
using Quaver.API.Helpers;
using Quaver.API.Maps;
using Quaver.API.Replays;
using Quaver.API.Replays.Virtual;

namespace Quaver.API.Test.Commands
{
    public class ReplayCommand : Command
    {
          /// <summary>
        ///     The parsed replay.
        /// </summary>
        public Replay Replay { get; }

        /// <summary>
        ///     The associated map for the replay.
        /// </summary>
        public Qua Map { get; }

        /// <summary>
        ///     The md5 hash of the map.
        /// </summary>
        public string MapMd5 { get; }

        /// <summary>
        /// </summary>
        /// <param name="args"></param>
        public ReplayCommand(string[] args) : base(args)
        {
            Replay = new Replay(args[1]);
            Map = Qua.Parse(args[2]);
            MapMd5 = CryptoHelper.FileToMd5(args[2]);
        }

        /// <summary>
        /// </summary>
        public override void Execute()
        {
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("\t\tREPLAY");
            Console.WriteLine("-------------------------------------");
            Console.WriteLine(Replay.ToString());

            Console.WriteLine();

            Console.WriteLine("-------------------------------------");
            Console.WriteLine("\t\tMAP");
            Console.WriteLine("-------------------------------------");
            // Console.WriteLine(Map.GetJson());

            Console.WriteLine();

            if (Replay.MapMd5 != MapMd5)
                throw new ArgumentException("The map specified isn't correct for this replay.");

            Console.WriteLine("-------------------------------------");
            Console.WriteLine("\t\tREPLAY INFO");
            Console.WriteLine("-------------------------------------");

            var keyPresses = Replay.GetKeyPresses();
            Console.WriteLine($"Key Presses: {keyPresses.Count}");

            var virtualPlayer = new VirtualReplayPlayer(Replay, Map);

            foreach (var frame in virtualPlayer.Replay.Frames)
            {
                virtualPlayer.PlayNextFrame();
            }

            Console.WriteLine(virtualPlayer.ScoreProcessor.Score);
            Console.WriteLine(virtualPlayer.ScoreProcessor.Accuracy);
            Console.WriteLine(virtualPlayer.ScoreProcessor.TotalJudgementCount);
            Console.WriteLine(virtualPlayer.ScoreProcessor.MaxCombo);

            Console.WriteLine("MARV: " + virtualPlayer.ScoreProcessor.CurrentJudgements[Judgement.Marv]);
            Console.WriteLine("PERF: " + virtualPlayer.ScoreProcessor.CurrentJudgements[Judgement.Perf]);
            Console.WriteLine("GREAT: " + virtualPlayer.ScoreProcessor.CurrentJudgements[Judgement.Great]);
            Console.WriteLine("GOOD: " + virtualPlayer.ScoreProcessor.CurrentJudgements[Judgement.Good]);
            Console.WriteLine("OK: " + virtualPlayer.ScoreProcessor.CurrentJudgements[Judgement.Okay]);
            Console.WriteLine("MS: " + virtualPlayer.ScoreProcessor.CurrentJudgements[Judgement.Miss]);

            // Write Key Presses to file.
            var str = "";
            virtualPlayer.ScoreProcessor.Stats.ForEach(x => { str += $"{x.Type}|{x.SongPosition}|{x.HitObject.Lane}|{x.HitDifference}|{x.Judgement}\r\n"; });
            File.WriteAllText("./debug.txt", str);
        }
    }
}