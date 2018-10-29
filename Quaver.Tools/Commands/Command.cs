namespace Quaver.Tools.Commands
{
    public abstract class Command
    {
        public Command(string[] args) {}
        public abstract void Execute();
    }
}