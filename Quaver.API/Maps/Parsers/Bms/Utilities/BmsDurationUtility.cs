using System.Collections.Generic;

namespace Quaver.API.Maps.Parsers.Bms.Utilities
{
    public static class DurationUtility
    {
        private const double MinuteUnit = 60.0;
        private const double Millisecond = 1000.0;

        /// <summary>
        ///     Gets the duration of a single beat (in 4/4 meter) given BPM.
        /// </summary>
        /// <param name="bpm"></param>
        /// <returns></returns>
        private static double getBeatDuration(double bpm) => MinuteUnit / bpm * Millisecond;

        /// <summary>
        ///     Gets the duration in milliseconds of a track, given BPM, in 4/4 meter.
        /// </summary>
        /// <param name="bpm"></param>
        /// <returns></returns>
        private static double getBaseTrackDuration(double bpm) => getBeatDuration(bpm) * 4.0;

        /// <summary>
        ///     Get the duration of a STOP command, given BPM.
        ///     A STOP command causes the track to stop scrolling for a given time. STOP duration as a
        ///     unit of 1 represents 1/192 of a whole note in 4/4 meter. It is not affected by measure scale,
        ///     since STOP is a musical rest.
        /// </summary>
        /// <param name="bpm"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static double GetStopDuration(double bpm, double duration) =>
            getBaseTrackDuration(bpm) * ( duration / 192.0 );

        /// <summary>
        ///     Gets the duration of a track, given BPM and measure scale. Does not account for
        ///     tempo changes, etc.
        /// </summary>
        /// <param name="bpm"></param>
        /// <param name="measureScale"></param>
        /// <returns></returns>
        public static double GetTrackDurationGivenBpm(double bpm, double measureScale) => getBaseTrackDuration(bpm) * measureScale;

        /// <summary>
        ///     Gets the duration of the entire track, accounting for all
        ///     tempo changes, stop commands, and measure scale. This is crucial in determining
        ///     the length of every track, and most calculations are done relative to the time the
        ///     function returns.
        /// </summary>
        /// <returns></returns>
        public static double GetTotalTrackDuration(double trackStartsWithBpm, List<BmsLocalTempoChange> tempoChanges, List<BmsLocalStopCommand> stopCommands, double measureScale)
        {
            var duration = 0.0;

            switch (tempoChanges.Count)
            {
                case 0 when stopCommands.Count == 0:
                    // No changes are necessary
                    return GetTrackDurationGivenBpm(trackStartsWithBpm, measureScale);
                case 0:
                    duration += GetTrackDurationGivenBpm(trackStartsWithBpm, measureScale);
                    break;
                default:
                {
                    for (var i = 0; i < tempoChanges.Count; i++)
                    {
                        if (i == 0)
                        {
                            duration += GetTrackDurationGivenBpm(trackStartsWithBpm, measureScale) * tempoChanges[i].Position;
                        }

                        duration += OffsetUtility.GetTempoChangeOffset(i, measureScale, tempoChanges);
                    }

                    break;
                }
            }

            // Account for stop time
            return duration + OffsetUtility.GetStopOffset(trackStartsWithBpm, 1.0, tempoChanges, stopCommands);
        }

    }
}