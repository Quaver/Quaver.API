namespace Quaver.API.Maps.Parsers.Bms
{
    public class BmsHitObject
    {
        public double StartTime { get; set; }

        public double EndTime { get; set; }

        public bool IsLongNote { get; set; }

        public int Lane { get; set; }

        public BmsSound KeySound { get; set; }
    }
}