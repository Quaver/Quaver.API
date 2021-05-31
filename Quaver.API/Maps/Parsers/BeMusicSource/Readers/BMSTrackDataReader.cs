using System;
using System.Collections.Generic;
using System.Linq;
using Quaver.API.Maps.Parsers.BeMusicSource.Classes;
using Quaver.API.Maps.Parsers.BeMusicSource.Utils;

namespace Quaver.API.Maps.Parsers.BeMusicSource.Readers
{
    public class BMSTrackDataReader
    {
        public readonly BMSTrackData TrackData = new BMSTrackData()
        {
            BPMChanges = new List<BMSLocalBPMChange>(),
            Stops = new List<BMSLocalStop>(),
            MeasureScale = 1.0
        };

        public BMSTrackDataReader(IEnumerable<BMSLineData> trackLines, BMSFileData fileData)
        {
             foreach (var line in trackLines.Where(line => line.Message.Length != 0))
             {
                 switch (line.Channel)
                 {
                     case "02":
                         TrackData.MeasureScale = double.Parse(line.Message);
                         continue;
                     case "03":
                     case "08":
                         for (var i = 0; i < line.Message.Length / 2; i++)
                         {
                             var value = BMSStringUtil.GetHexAtIndex(i, line.Message);
                             if (value == "00" || line.Channel == "08" && !fileData.BPMChangeIndex.ContainsKey(value)) continue;

                             var bpm = line.Channel == "03"
                                 ? int.Parse(value, System.Globalization.NumberStyles.HexNumber) : fileData.BPMChangeIndex[value];
                             TrackData.BPMChanges.Add(new BMSLocalBPMChange()
                             {
                                 Position = BMSStringUtil.GetPositionInTrack(i, line.Message.Length),
                                 BPM = Math.Abs(bpm),
                                 IsNegative = bpm < 0.0
                             });
                         }
                         continue;
                     case "09":

                         for (var i = 0; i < line.Message.Length / 2; i++) {
                             var value = BMSStringUtil.GetHexAtIndex(i, line.Message);
                             if (value == "00" || !fileData.StopIndex.ContainsKey(value)) continue;

                             TrackData.Stops.Add(new BMSLocalStop()
                             {
                                 Position = BMSStringUtil.GetPositionInTrack(i, line.Message.Length),
                                 Duration = fileData.StopIndex[value]
                             });
                         }
                         continue;
                 }
             }

             if (TrackData.BPMChanges.Count > 0)
             {
                 TrackData.BPMChanges.Sort((x, y) => x.Position.CompareTo(y.Position));

             }

             if (TrackData.Stops.Count > 0)
             {
                 TrackData.Stops.Sort((x, y) => x.Position.CompareTo(y.Position));
             }

        }
    }
}