using System;
using Quaver.API.Maps;
using Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys;

namespace Quaver.API.Test.Commands
{
    public class QssCommand : Command
    {
        public Qua Map { get; }

        public QssCommand(string[] args) : base(args) => Map = Qua.Parse(args[1]);
        
        public override void Execute()
        {
            var qss = new StrainSolverKeys(Map);
            Console.WriteLine($"Map: {Map.Artist} - {Map.Title} [{Map.DifficultyName}]");
            Console.WriteLine($"Overall Difficulty: {qss.OverallDifficulty}");
        }
    }
}