using System.Collections.Generic;

namespace Quaver.API.Maps.Parsers.Bms.Utilities
{
    public static class PositionUtility
    {
        /// <summary>
        ///     Compiles a list of timing points, based on the track's tempo changes and stop commands.
        /// </summary>
        /// <param name="currentTime"></param>
        /// <param name="trackStartsWithBpm"></param>
        /// <param name="tempoChanges"></param>
        /// <param name="stopCommands"></param>
        /// <param name="measureScale"></param>
        /// <returns></returns>
        public static IEnumerable<BmsTimingPoint> GetPositionOfTimingPoints(
            double currentTime, double trackStartsWithBpm, List<BmsLocalTempoChange> tempoChanges, List<BmsLocalStopCommand> stopCommands, double measureScale)
        {
            var timingPoints = new List<BmsTimingPoint>();

            if (tempoChanges.Count > 0)
            {
                var timeElapsed = 0.0;

                for (var i = 0; i < tempoChanges.Count; i++)
                {
                    if (i == 0)
                    {
                        timeElapsed += DurationUtility.GetTrackDurationGivenBpm(trackStartsWithBpm, measureScale) *
                                       tempoChanges[i].Position;
                    }

                    var stopTime = OffsetUtility.GetStopOffset(trackStartsWithBpm, tempoChanges[i].Position,
                        tempoChanges, stopCommands);

                    // Quaver will already handle negative BPM for us by scrolling backward.
                    // The rest of the BMS parser calculates positions based on absolute value,
                    // should the BPM be negative.
                    timingPoints.Add(new BmsTimingPoint()
                    {
                        Bpm = tempoChanges[i].IsNegative ? -tempoChanges[i].Bpm : tempoChanges[i].Bpm,
                        StartTime = currentTime + stopTime + timeElapsed
                    });

                    timeElapsed += OffsetUtility.GetTempoChangeOffset(i, measureScale, tempoChanges);
                }
            }

            if (stopCommands.Count > 0)
            {
                var timeElapsed = 0.0;
                if (tempoChanges.Count > 0)
                {
                    for (var i = 0; i < tempoChanges.Count; i++)
                    {
                        if (i == 0)
                        {
                            timeElapsed += DurationUtility.GetTrackDurationGivenBpm(trackStartsWithBpm, measureScale) *
                                           tempoChanges[i].Position;
                        }

                        foreach (var stop in stopCommands)
                        {
                            if (i + 1 == tempoChanges.Count
                                || i + 1 < tempoChanges.Count
                                && tempoChanges[i + 1].Position > stop.Position
                                && stop.Position >= tempoChanges[i].Position)
                            {
                                var stopTime = OffsetUtility.GetStopOffset(trackStartsWithBpm, stop.Position,
                                    tempoChanges, stopCommands);
                                var startAt = currentTime + timeElapsed + stopTime +
                                              DurationUtility.GetTrackDurationGivenBpm(tempoChanges[i].Bpm,
                                                  measureScale) *
                                              ( stop.Position - tempoChanges[i].Position );
                                var endAt = startAt +
                                            DurationUtility.GetStopDuration(tempoChanges[i].Bpm,
                                                stop.Duration);
                                timingPoints.Add(new BmsTimingPoint {
                                    StartTime = startAt,
                                    Bpm = 0.0
                                });
                                timingPoints.Add(new BmsTimingPoint {
                                    StartTime = endAt,
                                    Bpm = tempoChanges[i].Bpm
                                });
                            }
                        }

                        timeElapsed += OffsetUtility.GetTempoChangeOffset(i, measureScale, tempoChanges);
                    }
                }
                else
                {
                    for (var i = 0; i < stopCommands.Count; i++)
                    {
                        if (i == 0)
                        {
                            timeElapsed += DurationUtility.GetTrackDurationGivenBpm(trackStartsWithBpm, measureScale) *
                                           stopCommands[i].Position;
                        }

                        var stopTime =
                            OffsetUtility.GetStopOffset(trackStartsWithBpm, stopCommands[i].Position, tempoChanges, stopCommands);
                        timingPoints.Add(new BmsTimingPoint
                        {
                            StartTime = currentTime + timeElapsed + stopTime,
                            Bpm = 0.000001
                        });
                        timingPoints.Add(new BmsTimingPoint
                        {
                            StartTime = currentTime + timeElapsed + stopTime +  DurationUtility.GetStopDuration(trackStartsWithBpm,
                                stopCommands[i].Duration),
                            Bpm = trackStartsWithBpm
                        });

                        if (i + 1 < stopCommands.Count)
                        {
                            timeElapsed += DurationUtility.GetTrackDurationGivenBpm(trackStartsWithBpm, measureScale) *
                                           ( stopCommands[i + 1].Position - stopCommands[i].Position );
                        } else if (i + 1 == stopCommands.Count)
                        {
                            timeElapsed += DurationUtility.GetTrackDurationGivenBpm(trackStartsWithBpm, measureScale) *
                                           ( ( 1.0 - stopCommands[i].Position ) / 1.0 );
                        }
                    }
                }
            }

            return timingPoints;
        }

        /// <summary>
        ///     Gets the position, from 0 to 1, of an object in the current track. Given a hexatridecimal pair like 11,
        ///     and the message is 00001100, the pair is at 0.5 in the track.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="messageLength"></param>
        /// <returns></returns>
        public static double GetPositionInTrack(int index, int messageLength) => index / ( messageLength / 2.0 );

        /// <summary>
        ///     Gets the position of an object's start time, in milliseconds.
        /// </summary>
        /// <param name="trackStartsWithBpm"></param>
        /// <param name="tempoChanges"></param>
        /// <param name="stopCommands"></param>
        /// <param name="index"></param>
        /// <param name="message"></param>
        /// <param name="measureScale"></param>
        /// <returns></returns>
        public static double GetPositionOfStartTime(
            double trackStartsWithBpm, List<BmsLocalTempoChange> tempoChanges, List<BmsLocalStopCommand> stopCommands, int index, string message, double measureScale)
        {
            var objectPosition = GetPositionInTrack(index, message.Length);

            if (tempoChanges.Count == 0 && stopCommands.Count == 0)
            {
                return DurationUtility.GetTrackDurationGivenBpm(trackStartsWithBpm, measureScale) * objectPosition;
            }

            var duration = 0.0;

            for (var i = 0; i < tempoChanges.Count; i++)
            {
                if (i == 0)
                {
                    if (objectPosition < tempoChanges[i].Position)
                    {
                        duration += DurationUtility.GetTrackDurationGivenBpm(trackStartsWithBpm, measureScale) *
                                    objectPosition;
                        break;
                    }
                    duration += DurationUtility.GetTrackDurationGivenBpm(trackStartsWithBpm, measureScale) *
                                tempoChanges[i].Position;
                }

                if (i + 1 == tempoChanges.Count
                    || i + 1 < tempoChanges.Count
                    && tempoChanges[i + 1].Position > objectPosition
                    && objectPosition >= tempoChanges[i].Position)
                {
                    duration += DurationUtility.GetTrackDurationGivenBpm(tempoChanges[i].Bpm, measureScale) *
                                ( objectPosition - tempoChanges[i].Position );
                    break;
                }
                duration += DurationUtility.GetTrackDurationGivenBpm(tempoChanges[i].Bpm, measureScale) *
                            ( tempoChanges[i+1].Position - tempoChanges[i].Position );
            }

            if (tempoChanges.Count == 0)
            {
                duration += DurationUtility.GetTrackDurationGivenBpm(trackStartsWithBpm, measureScale) * objectPosition;
            }

            if (stopCommands.Count > 0)
            {
                duration += OffsetUtility.GetStopOffset(trackStartsWithBpm, objectPosition, tempoChanges,
                    stopCommands);
            }

            return duration;
        }
    }
}