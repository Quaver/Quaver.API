using System;
using Newtonsoft.Json.Linq;
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

            Console.WriteLine(JObject.FromObject(new
            {
                Metadata = new
                {
                    Map.Artist,
                    Map.Title,
                    Map.DifficultyName,
                    ObjectCount = Map.HitObjects.Count,
                    Map.Length
                },
                Difficulty = new
                {
                    qss.OverallDifficulty,
                    qss.AverageNoteDensity,
                    qss.Bracket,
                    qss.Roll,
                    qss.SJack,
                    qss.TJack
                }
            }));
        }
    }
}