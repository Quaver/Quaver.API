﻿using System;
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
        public int Lives { get; protected set; }

        /// <summary>
        ///     If the player is eliminated from the game
        ///
        ///     1. Has no more lives left
        /// </summary>
        public bool IsEliminated => HealthType == MultiplayerHealthType.Lives && Lives == 0;

        /// <summary>
        ///     If the player has failed at any point during the game.
        /// </summary>
        public bool HasFailed { get; protected set; }

        /// <summary>
        ///     If the user is currently failed and regenerating their health
        /// </summary>
        public bool IsRegeneratingHealth { get; protected set; }

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
                case MultiplayerHealthType.ManualGeneration:
                    // ReSharper disable twice CompareOfFloatsByEqualityOperator
                    if (Processor.Health == 0)
                        IsRegeneratingHealth = true;
                    else if (Processor.Health == 100 && IsRegeneratingHealth)
                        IsRegeneratingHealth = false;
                    break;
                // If we're dealing with lives, we want to
                case MultiplayerHealthType.Lives:
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if (Processor.Health == 0)
                    {
                        if (Lives == 0)
                            return;

                        Lives--;
                        Processor.Health = 100;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}