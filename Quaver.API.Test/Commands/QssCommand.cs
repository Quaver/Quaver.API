using System;
using Quaver.API.Maps;
using Quaver.API.Qss;

namespace Quaver.API.Test.Commands
{
    public class QssCommand : Command
    {
        public Qua Map { get; }

        public QssCommand(string[] args) : base(args) => Map = Qua.Parse(args[1]);
        
        public override void Execute()
        {
            //var data = StrainRatingData.GetQssData(Map);
            //Console.WriteLine($"Qss Subject: {Map.Artist} - {Map.Title} [{Map.DifficultyName}]");
            //Console.WriteLine($"Overall Difficulty: {data.OverallDifficulty}");
        }
    }
}