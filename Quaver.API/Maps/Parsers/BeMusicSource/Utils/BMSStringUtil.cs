using System;
using System.Collections.Generic;

namespace Quaver.API.Maps.Parsers.BeMusicSource.Utils
{
    public static class BMSStringUtil
    {
        public static string GetHexAtIndex(int index, string message) => message.Substring(index * 2, 2);

        public static int GetLaneNumber(string input) =>
            "0123456789abcdefghijklmnopqrstuvwxyz".IndexOf(input, StringComparison.InvariantCulture);

        public static double GetPositionInTrack(int index, int messageLength) => 100.0 * ( index / ( messageLength / 2.0 ) );

        public static string GetDifficultyName(string i, string sub)
        {
            var b = "Lv. " + i;
            if (sub == null) return b;
            return sub + " " + b;
        }

        public static string AppendSubArtistsToArtist(string a, List<string> subArtists)
        {
            if (subArtists.Count == 0) return a;
            return a + " <" + string.Join(" | ", subArtists.ToArray()) + ">";
        }
    }
}