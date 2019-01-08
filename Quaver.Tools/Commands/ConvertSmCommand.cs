using System.IO;
using Quaver.API.Maps.Parsers.StepMania;

namespace Quaver.Tools.Commands
{
    public class ConvertSmCommand : Command
    {
        private StepmaniaConverter Converter { get; }

        private string OutputPath { get; }

        public ConvertSmCommand(string[] args) : base(args)
        {
            OutputPath = args[2];
            Directory.CreateDirectory(OutputPath);

            Converter = new StepmaniaConverter(args[1]);

            var quas = Converter.ToQua();

            for (var i = 0; i < quas.Count; i++)
                quas[i].Save($"{OutputPath}/{i}.qua");
        }

        public override void Execute()
        {
        }
    }
}