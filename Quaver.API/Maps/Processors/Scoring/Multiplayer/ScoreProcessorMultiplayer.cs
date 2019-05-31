using System;
using Quaver.API.Enums;

namespace Quaver.API.Maps.Processors.Scoring.Multiplayer
{
    public class ScoreProcessorMultiplayer
    {
        /// <summary>
        /// </summary>
        public ScoreProcessor Processor { get; set; }

        /// <summary>
        ///     The type of health/life system that'll be used in multiplayer
        /// </summary>
        public MultiplayerHealthType HealthType { get; }

        /// <summary>
        ///     The amount of lives the player has in multiplayer
        /// </summary>
        public int Lives { get; set; }

        /// <summary>
        ///     If the player is eliminated from the game
        /// </summary>
        public bool IsEliminated => (HealthType == MultiplayerHealthType.Lives && Lives == 0) || IsBattleRoyaleEliminated;

        /// <summary>
        ///     If the player has failed at any point during the game.
        /// </summary>
        public bool HasFailed { get; protected set; }

        /// <summary>
        ///     If the user is currently failed and regenerating their health
        /// </summary>
        public bool IsRegeneratingHealth { get; protected set; }

        /// <summary>
        ///     If the player is elimintated from battle royale
        /// </summary>
        public bool IsBattleRoyaleEliminated { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="healthType"></param>
        /// <param name="lives"></param>
        public ScoreProcessorMultiplayer(MultiplayerHealthType healthType, int lives)
        {
            HealthType = healthType;
            Lives = lives;
        }

        /// <summary>
        ///     Handle health/lives/elimination calculations within multiplayer
        /// </summary>
        public virtual void CalculateHealth()
        {
            if (Processor.Mods.HasFlag(ModIdentifier.NoFail))
                return;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (Processor.Health == 0)
                HasFailed = true;

            switch (HealthType)
            {
                // When player reaches 0 health, place them in a state where they have to reach 100 health
                // in order to be considered alive again
                case MultiplayerHealthType.Manual_Regeneration:
                    // ReSharper disable twice CompareOfFloatsByEqualityOperator
                    if (Processor.Health == 0)
                        IsRegeneratingHealth = true;
                    else if (Processor.Health == 100 && IsRegeneratingHealth)
                        IsRegeneratingHealth = false;
                    break;
                // If we're dealing with lives, remove lives from the player & restore their health back to 100
                case MultiplayerHealthType.Lives:
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if (Processor.Health == 0)
                    {
                        if (Lives == 0)
                            return;

                        Lives--;

                        if (Lives == 0)
                            return;

                        Processor.Health = 100;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}