using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Quaver.API.Maps;
using Quaver.API.Maps.AutoMod;

namespace Quaver.Tools.Commands
{
    public class AutoModCommand : Command
    {
        private string Dir { get; }

        public AutoModCommand(string[] args) : base(args)
        {

            Dir = args[1];
        }

        public override void Execute()
        {
            var totalIssues = 0;

            foreach (var file in Directory.GetFiles(Dir))
            {
                if (Path.GetExtension(file) != ".qua")
                    continue;

                var qua = Qua.Parse(file);
                var automod = new AutoMod(qua);
                automod.Run();

                totalIssues += automod.Issues.Count;
            }

            Console.WriteLine(JObject.FromObject(new
            {
                HasIssues = totalIssues > 0
            }));
        }
    }
}