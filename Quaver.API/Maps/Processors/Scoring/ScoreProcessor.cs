namespace Quaver.API.Maps.Processors
{
    public abstract class ScoreProcessor
    {
        /// <summary>
        ///     The map that will have its score processed.
        /// </summary>
        public Qua Map { get;  }

        /// <summary>
        ///     The score 
        /// </summary>
        public int Score { get; protected set; }

        public float Accuracy { get; protected set; }

        /// <summary>
        ///     Ctor - 
        /// </summary>
        /// <param name="map"></param>
        public ScoreProcessor(Qua map)
        {
            Map = map;
        }

         /// <summary>
        ///     Calculates score and accuracy for a given object and hit time.
        /// </summary>
        public abstract void CalculateScoreForObject();
    }
}