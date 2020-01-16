using System.IO;
using Quaver.API.Maps.Parsers.Stepmania;

namespace Quaver.Tools.Commands
{
    public class ConvertSmCommand : Command
    {
        private StepFile Converter { get; }

        private string OutputPath { get; }

        public ConvertSmCommand(string[] args) : base(args)
        {
            OutputPath = args[2];
            Directory.CreateDirectory(OutputPath);

            Converter = new StepFile(args[1]);

            var quas = Converter.ToQuas();

            for (var i = 0; i < quas.Count; i++)
                quas[i].Save($"{OutputPath}/{i}.qua");
        }

        public override void Execute()
        {
        }
    }
}