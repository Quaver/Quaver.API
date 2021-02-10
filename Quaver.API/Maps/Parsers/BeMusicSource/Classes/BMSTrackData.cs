using System.Collections.Generic;

namespace Quaver.API.Maps.Parsers.BeMusicSource.Classes
{
    public class BMSTrackData
    {
        public double MeasureScale { get; set; }

        public List<BMSLocalBPMChange> BPMChanges { get; set; }

        public List<BMSLocalStop> Stops { get; set; }
    }
}