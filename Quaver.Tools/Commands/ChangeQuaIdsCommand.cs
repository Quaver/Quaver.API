using Quaver.API.Maps;

namespace Quaver.Tools.Commands
{
    public class ChangeQuaIdsCommand : Command
    {
        public ChangeQuaIdsCommand(string[] args) : base(args)
        {
            var qua = Qua.Parse(args[1], false);
            qua.MapId = int.Parse(args[3]);
            qua.MapSetId = int.Parse(args[4]);
            qua.Save(args[2]);
        }

        public override void Execute()
        {
        }
    }
}