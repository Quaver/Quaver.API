using Quaver.API.Maps.Parsers;

namespace Quaver.Tools.Commands
{
    public class ConvertOsuCommand : Command
    {
        private OsuBeatmap Beatmap { get; set; }

        private string OutputPath { get; }

        public ConvertOsuCommand(string[] args) : base(args)
        {
            OutputPath = args[2];
            Beatmap = new OsuBeatmap(args[1]);

            Beatmap.ToQua().Save(args[2]);
        }

        public override void Execute()
        {
        }
    }
}