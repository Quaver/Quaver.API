using System;
using Quaver.API.Maps.Parsers;

namespace Quaver.API.Test.Commands
{
    public class OsuCommand : Command
    {
        private string OsuPath { get; }
        private string OutputPath { get; }
        
        private OsuBeatmap Osu { get; set; }

        public OsuCommand(string[] args) : base(args)
        {
            OsuPath = args[1];
            OutputPath = args[2];
        }

        public override void Execute()
        {
            new OsuBeatmap(OsuPath).ToQua().Save(OutputPath);            
            Console.WriteLine($"osu! beatmap successfully converted at {OutputPath}");
        }
    }
}