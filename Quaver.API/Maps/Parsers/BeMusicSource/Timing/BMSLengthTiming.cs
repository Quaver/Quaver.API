using Quaver.API.Maps.Parsers.BeMusicSource.Classes;

namespace Quaver.API.Maps.Parsers.BeMusicSource.Timing
{
    public static class BMSLengthTiming
    {
        private const double Millisecond = 1000.0;
        private const double MinuteUnit = 60.0;
        private const double RegularMeter = 4.0;
        private const double StopWholeNote = 192.0;

        private static double getBeatLength(double bpm)
        {
            if (bpm == 0.0) return 0.0;
            return MinuteUnit / bpm * Millisecond;
        }

        private static double getBaseTrackLength(double bpm) => getBeatLength(bpm) * RegularMeter;

        public static double STOPLengthToTime(double bpm, double duration) =>
            getBaseTrackLength(bpm) * ( duration / StopWholeNote );

        public static double TrackLengthGivenBPM(double bpm, double measureScale) => getBaseTrackLength(bpm) * measureScale;

        public static double GetTotalTrackLength(double initialBpm, BMSTrackData trackData)
        {
            var baseLen = 0.0;
            var len = TrackLengthGivenBPM(initialBpm, trackData.MeasureScale);
            switch (trackData.BPMChanges.Count)
            {
                case 0 when trackData.Stops.Count == 0:
                    return len;
                case 0:
                    baseLen += len;
                    break;
            }

            for (var i = 0; i < trackData.BPMChanges.Count; i++)
            {
                if (i == 0) baseLen += len * (trackData.BPMChanges[i].Position / 100.0);
                baseLen += BMSOffsetTiming.GetBPMChangeOffset(i, trackData);
            }

            return baseLen + BMSOffsetTiming.GetSTOPOffset(initialBpm, 100.0, trackData);
        }
    }
}