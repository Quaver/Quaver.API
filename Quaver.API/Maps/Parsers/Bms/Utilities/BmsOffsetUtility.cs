using System.Collections.Generic;

namespace Quaver.API.Maps.Parsers.Bms.Utilities
{
    public static class OffsetUtility
    {
        /// <summary>
        ///     Calculates the offset, in milliseconds, all STOP commands in the track cause based on a given
        ///     position in the track.
        /// </summary>
        /// <param name="trackStartsWithBpm"></param>
        /// <param name="position"></param>
        /// <param name="tempoChanges"></param>
        /// <param name="stopCommands"></param>
        /// <returns></returns>
        public static double GetStopOffset(double trackStartsWithBpm, double position, List<BmsLocalTempoChange> tempoChanges,
            List<BmsLocalStopCommand> stopCommands)
        {
            var offset = 0.0;

            if (stopCommands.Count == 0)
            {
                // No STOP commands exist.
                return offset;
            }

            foreach (var stop in stopCommands)
            {
                // The position given is not greater than the STOP command. Therefore, it would have no
                // effect.
                if (!( position > stop.Position )) continue;
                var proposedBpm = trackStartsWithBpm;
                for (var i = 0; i < tempoChanges.Count; i++)
                {
                    if (i + 1 == tempoChanges.Count
                        || i + 1 < tempoChanges.Count
                        && tempoChanges[i+1].Position > stop.Position
                        && stop.Position >= tempoChanges[i].Position)
                    {
                        proposedBpm = tempoChanges[i].Bpm;
                        break;
                    }
                }

                offset += DurationUtility.GetStopDuration(proposedBpm, stop.Duration);
            }

            return offset;
        }

        /// <summary>
        ///     Calculates the offset, in milliseconds, tempo changes would cause, given an index of
        ///     where the function should start from.
        /// </summary>
        /// <param name="currentIndex"></param>
        /// <param name="measureScale"></param>
        /// <param name="tempoChanges"></param>
        /// <returns></returns>
        public static double GetTempoChangeOffset(int currentIndex, double measureScale, List<BmsLocalTempoChange> tempoChanges)
        {
            if (tempoChanges.Count == 0)
            {
                return 0.0;
            }

            if (currentIndex + 1 < tempoChanges.Count)
            {
                return DurationUtility.GetTrackDurationGivenBpm(tempoChanges[currentIndex].Bpm, measureScale) *
                       ( tempoChanges[currentIndex + 1].Position - tempoChanges[currentIndex].Position );
            }

            if (currentIndex + 1 == tempoChanges.Count)
            {
                return DurationUtility.GetTrackDurationGivenBpm(tempoChanges[currentIndex].Bpm, measureScale) * ( ( 1.0 -
                    tempoChanges[currentIndex].Position ) / 1.0 );
            }

            // This should never be the case
            return 0.0;
        }
    }
}