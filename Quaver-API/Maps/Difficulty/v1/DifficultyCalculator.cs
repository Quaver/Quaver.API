using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quaver.API.Maps.Difficulty.v1.Patterns;

namespace Quaver.API.Maps.Difficulty.v1
{
    public static class DifficultyCalculator
    {
        /// <summary>
        ///     Calculates the difficulty of vibro patterns.
        ///     
        ///     Vibro difficulty should be based on the following:
        ///         - Speed
        ///         - Stamina
        ///         - Control (?)
        /// 
        ///     There needs to be some sort of line that compares vibro skill to jack & stream skill.
        ///     Since vibro is an entirely different skill in itself, 
        /// </summary>
        /// <param name="hitObjects"></param>
        /// <returns></returns>
        public static double CalculateVibroDifficulty(List<JackPatternInfo> patterns)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Calculates the difficulty of jack patterns
        ///     
        ///     Ideally jack difficulty should be based on the following:
        ///         - Speed
        ///         - Stamina
        ///         - Pattern Type (Single/Jump/Hand/Quad Jacks)
        /// </summary>
        /// <param name="hitObjects"></param>
        /// <returns></returns>
        public static double CalculateJackDifficulty(List<JackPatternInfo> patterns)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Calculates the difficulty of stream patterns
        ///     
        ///     Stream difficulty should be based on the following:
        ///         - Speed
        ///         - Stamina
        ///         - Pattern Type (Single/Jump/Hand/Quad Streams)
        /// </summary>
        /// <param name="hitObjects"></param>
        /// <returns></returns>
        public static double CalculateStreamDifficulty(List<StreamPatternInfo> patterns)
        {
            throw new NotImplementedException();
        }
    }
}
