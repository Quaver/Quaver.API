using Quaver.API.Enums;

namespace Quaver.API.Maps.Processors.Scoring.Data
{
    /// <summary>
    ///     Structure that contains data for an individual hit.
    /// </summary>
    public struct HitStat
    {
        /// <summary>
        ///     The type of hit this was.
        /// </summary>
        public HitStatType Type { get; }

        /// <summary>
        ///     The HitObject that this is referencing to.
        /// </summary>
        public HitObjectInfo HitObject { get; }

        /// <summary>
        ///     The position in the song the object was hit.
        /// </summary>
        public double SongPosition { get; }

        /// <summary>
        ///     The judgement received for this hit.
        /// </summary>
        public Judgement Judgement { get; }

        /// <summary>
        ///     The difference between the the hit and the song position.
        /// </summary>
        public double HitDifference { get; }

        /// <summary>
        ///     The user's accuracy at this point of the hit.
        /// </summary>
        public double Accuracy { get; }

        /// <summary>
        ///     The user's health at this point of the hit.
        /// </summary>
        public float Health { get; }

       
        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="hitObject"></param>
        /// <param name="songPos"></param>
        /// <param name="judgement"></param>
        /// <param name="hitDifference"></param>
        /// <param name="acc"></param>
        /// <param name="health"></param>
        public HitStat(HitStatType type, HitObjectInfo hitObject, double songPos, Judgement judgement, double hitDifference, double acc, float health)
        {
            HitObject = hitObject;
            SongPosition = songPos;
            Judgement = judgement;
            HitDifference = hitDifference;
            Accuracy = acc;
            Health = health;
            Type = type;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"Obj @ {SongPosition} | Hit Diff @ {HitDifference} | {Judgement} | {Accuracy}% | Life: {Health}";
    }
}