using System;
using Quaver.API.Maps;

namespace Quaver.API.Test.Commands
{
    public class QuaCommand : Command
    {
        public Qua Map { get; }

        public QuaCommand(string[] args) : base(args) => Map = Qua.Parse(args[1]);
        
        public override void Execute()
        {
            Console.WriteLine($"Artist: {Map.Artist}\n" +
                                $"Title: {Map.Title}\n" +
                                $"Difficulty Name: {Map.Mode}\n" +
                                $"Mode: {Map.Mode}\n" +
                                $"Average NPS: {Map.AverageNotesPerSecond()}\n" +
                                $"Common BPM: {Map.GetCommonBpm()}\n" +
                                $"Difficulty Rating: {Map.CalculateDifficulty()}");
        }
    }
}