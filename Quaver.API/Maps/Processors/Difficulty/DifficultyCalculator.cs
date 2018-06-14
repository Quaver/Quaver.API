namespace Quaver.API.Maps.Processors.Difficulty
{
    public abstract class DifficultyCalculator
    {
        /// <summary>
        ///     Reference to the map to calculate difficulty for.
        /// </summary>
        private Qua Map { get;  }

        /// <summary>
        ///     Ctor -
        /// </summary>
        /// <param name="map"></param>
        public DifficultyCalculator(Qua map) => Map = map;

        /// <summary>
        ///     Calculates the difficulty of the map.
        /// </summary>
        /// <returns></returns>
        public abstract double CalculateDifficulty();
    }
}