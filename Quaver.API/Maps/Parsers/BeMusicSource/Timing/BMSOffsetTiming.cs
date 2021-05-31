using Quaver.API.Maps.Parsers.BeMusicSource.Classes;

namespace Quaver.API.Maps.Parsers.BeMusicSource.Timing
{
    public static class BMSOffsetTiming
    {
        public static double GetBPMChangeOffset(int currentIndex, BMSTrackData trackData)
        {
            if (trackData.BPMChanges.Count == 0) return 0.0;
            var len = BMSLengthTiming.TrackLengthGivenBPM(trackData.BPMChanges[currentIndex].BPM,
                trackData.MeasureScale);
            if (currentIndex + 1 < trackData.BPMChanges.Count)
            {
                return len *
                       ( ( trackData.BPMChanges[currentIndex + 1].Position -
                           trackData.BPMChanges[currentIndex].Position ) /
                         100.0 );
            }

            if (currentIndex + 1 == trackData.BPMChanges.Count)
            {
                return len * ( ( 100.0 - trackData.BPMChanges[currentIndex].Position ) / 100.0 );
            }

            return 0.0;
        }

        public static double GetSTOPOffset(double initialBpm, double pos, BMSTrackData trackData)
        {
            var len = 0.0;

            foreach (var stop in trackData.Stops)
            {
                if (!( pos > stop.Position )) continue;
                var b = initialBpm;
                for (var i = 0; i < trackData.BPMChanges.Count; i++)
                {
                    if (i + 1 < trackData.BPMChanges.Count && trackData.BPMChanges[i + 1].Position > stop.Position &&
                        stop.Position >= trackData.BPMChanges[i].Position || i + 1 == trackData.BPMChanges.Count &&
                        stop.Position >= trackData.BPMChanges[i].Position)
                    {
                        b = trackData.BPMChanges[i].BPM;
                        break;
                    }
                }

                len += BMSLengthTiming.STOPLengthToTime(b, stop.Duration);
            }

            return len;
        }
    }
}