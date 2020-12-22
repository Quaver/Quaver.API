namespace Quaver.API.Maps.Parsers.Bms
{
    public class BmsLocalTempoChange : BmsObject
    {
        public double Bpm { get; set; }

        public bool IsNegative { get; set; }
    }
}