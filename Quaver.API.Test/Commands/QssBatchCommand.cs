using System;
using Quaver.API.Maps;
using Quaver.API.Maps.Processors.Difficulty;

namespace Quaver.API.Test.Commands
{
    public class QssBatchCommand : Command
    {
        public Qua Map { get; }

        public QssBatchCommand(string[] args) : base(args)
        {
            var filePath = args[1];

            //1. find qss files in file path
            //2. parse qss files
            //3. calculate each diff from qss files

            //Map = Qua.Parse(args[1]);
        }
        
        public override void Execute()
        {
            var qss = new StrainSolverKeys(Map);
            Console.WriteLine($"Map: {Map.Artist} - {Map.Title} [{Map.DifficultyName}]");
            Console.WriteLine($"Overall Difficulty: {qss.OverallDifficulty}");
        }
    }
}