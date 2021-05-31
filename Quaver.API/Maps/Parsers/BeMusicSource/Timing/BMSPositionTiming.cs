using System;
using System.Collections.Generic;
using Quaver.API.Maps.Parsers.BeMusicSource.Classes;

namespace Quaver.API.Maps.Parsers.BeMusicSource.Timing
{
    public static class BMSPositionTiming
    {
        /// <summary>
        /// good luck reading this shit LOL
        /// </summary>
        /// <param name="currentTime"></param>
        /// <param name="startTrackWithBpm"></param>
        /// <param name="trackData"></param>
        /// <returns></returns>
        public static SortedDictionary<double, double> CalculateTimingPoints(double currentTime,
            double startTrackWithBpm,
            BMSTrackData trackData, int trackInt)
        {
            var pts = new SortedDictionary<double, double>();
            var baseLen = BMSLengthTiming.TrackLengthGivenBPM(startTrackWithBpm, trackData.MeasureScale);

            if (trackData.BPMChanges.Count > 0)
            {
                var timeElapsed = 0.0;
                for (var i = 0; i < trackData.BPMChanges.Count; i++)
                {
                    if (i == 0)
                        timeElapsed += baseLen *
                                       ( trackData.BPMChanges[i].Position / 100.0 );
                    var stopTime =
                        BMSOffsetTiming.GetSTOPOffset(startTrackWithBpm, trackData.BPMChanges[i].Position, trackData);

                    var calcTime = currentTime + stopTime + timeElapsed;
                    pts[calcTime] = trackData.BPMChanges[i].IsNegative
                        ? -trackData.BPMChanges[i].BPM
                        : trackData.BPMChanges[i].BPM;

                    timeElapsed += BMSOffsetTiming.GetBPMChangeOffset(i, trackData);
                }
            }

            if (trackData.Stops.Count == 0) return pts;
            // 𝘆𝗼𝘂 𝗹𝗼𝗼𝗸𝗶𝗻 𝗺𝗮𝗱 𝘀𝘂𝘀
            var fuck = 0.0;
            for (var stopIndex = 0; stopIndex < trackData.Stops.Count; stopIndex++)
            {
                var stopTime = BMSOffsetTiming.GetSTOPOffset(startTrackWithBpm, trackData.Stops[stopIndex].Position,
                    trackData);
                if (trackData.BPMChanges.Count > 0)
                {
                    var localTimeElapsed = 0.0;

                    for (var bpmIndex = 0; bpmIndex < trackData.BPMChanges.Count; bpmIndex++)
                    {
                        if (bpmIndex == 0)
                            localTimeElapsed += baseLen * ( trackData.BPMChanges[bpmIndex].Position / 100.0 );
                        if (bpmIndex + 1 < trackData.BPMChanges.Count &&
                            trackData.BPMChanges[bpmIndex + 1].Position > trackData.Stops[stopIndex].Position &&
                            trackData.Stops[stopIndex].Position >= trackData.BPMChanges[bpmIndex].Position ||
                            bpmIndex + 1 == trackData.BPMChanges.Count &&
                            trackData.Stops[stopIndex].Position >= trackData.BPMChanges[bpmIndex].Position)
                        {
                            var startAt = currentTime + stopTime + localTimeElapsed +
                                          BMSLengthTiming.TrackLengthGivenBPM(trackData.BPMChanges[bpmIndex].BPM,
                                              trackData.MeasureScale) *
                                          ( ( trackData.Stops[stopIndex].Position -
                                              trackData.BPMChanges[bpmIndex].Position ) /
                                            100.0 );
                            var endAt = startAt + BMSLengthTiming.STOPLengthToTime(trackData.BPMChanges[bpmIndex].BPM,
                                trackData.Stops[stopIndex].Duration);

                            pts[startAt] = 0.0;
                            pts[endAt] = trackData.BPMChanges[bpmIndex].BPM;
                            break;
                        }

                        if (bpmIndex + 1 == trackData.BPMChanges.Count &&
                            trackData.Stops[stopIndex].Position < trackData.BPMChanges[0].Position)
                        {
                            var startAt = currentTime + stopTime +
                                          BMSLengthTiming.TrackLengthGivenBPM(startTrackWithBpm,
                                              trackData.MeasureScale) *
                                          ( trackData.Stops[stopIndex].Position / 100.0 );
                            var endAt = startAt +
                                        BMSLengthTiming.STOPLengthToTime(startTrackWithBpm,
                                            trackData.Stops[stopIndex].Duration);

                            pts[startAt] = 0.0;
                            pts[endAt] = startTrackWithBpm;
                            break;
                        }

                        localTimeElapsed += BMSOffsetTiming.GetBPMChangeOffset(bpmIndex, trackData);
                    }

                    continue;
                }

                if (stopIndex == 0) fuck += baseLen * ( trackData.Stops[stopIndex].Position / 100.0 );

                var start = currentTime + fuck + stopTime;
                var end = start +
                          BMSLengthTiming.STOPLengthToTime(startTrackWithBpm, trackData.Stops[stopIndex].Duration);

                pts[start] = 0.0;
                pts[end] = startTrackWithBpm;

                if (stopIndex + 1 < trackData.Stops.Count)
                {
                    fuck += baseLen *
                            ( ( trackData.Stops[stopIndex + 1].Position - trackData.Stops[stopIndex].Position ) /
                              100.0 );
                }
                else if (stopIndex + 1 == trackData.Stops.Count)
                {
                    fuck += baseLen *
                            ( ( 100.0 - trackData.Stops[stopIndex].Position ) / 100.0 );
                }
            }

            return pts;
        }

        public static double GetStartTime(BMSTrackData trackData, int index, string message, double startTrackWithBpm)
        {
            var measure = message.Length / 2.0;
            var pos = index / measure * 100.0;
            var baseLen = BMSLengthTiming.TrackLengthGivenBPM(startTrackWithBpm, trackData.MeasureScale);

            if (trackData.BPMChanges.Count == 0 && trackData.Stops.Count == 0) return baseLen * ( pos / 100.0 );

            var timeAdded = 0.0;

            for (var i = 0; i < trackData.BPMChanges.Count; i++)
            {
                if (i == 0)
                {
                    if (pos < trackData.BPMChanges[i].Position)
                    {
                        timeAdded += baseLen * ( pos / 100.0 );
                        break;
                    }

                    timeAdded += baseLen * ( trackData.BPMChanges[i].Position / 100.0 );
                }

                if (i + 1 == trackData.BPMChanges.Count && pos >= trackData.BPMChanges[i].Position ||
                    i + 1 < trackData.BPMChanges.Count && trackData.BPMChanges[i + 1].Position > pos &&
                    pos >= trackData.BPMChanges[i].Position)
                {
                    timeAdded +=
                        BMSLengthTiming.TrackLengthGivenBPM(trackData.BPMChanges[i].BPM, trackData.MeasureScale) *
                        ( ( pos - trackData.BPMChanges[i].Position ) / 100.0 );
                    break;
                }

                if (i + 1 < trackData.BPMChanges.Count)
                {
                    timeAdded +=
                        BMSLengthTiming.TrackLengthGivenBPM(trackData.BPMChanges[i].BPM, trackData.MeasureScale) *
                        ( ( trackData.BPMChanges[i + 1].Position - trackData.BPMChanges[i].Position ) / 100.0 );
                }
            }

            if (trackData.BPMChanges.Count == 0)
            {
                timeAdded += baseLen * ( pos / 100.0 );
            }

            if (trackData.Stops.Count > 0)
            {
                timeAdded += BMSOffsetTiming.GetSTOPOffset(startTrackWithBpm, pos, trackData);
            }

            return timeAdded;
        }
    }
}