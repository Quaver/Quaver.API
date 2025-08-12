/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Shared;
using Quaver.API.Enums;
using Quaver.API.Helpers;
using Quaver.API.Maps.Processors.Scoring.Data;
using Quaver.API.Maps.Processors.Scoring.Multiplayer;
using Quaver.API.Replays;
using HitObjectType = Quaver.API.Enums.HitObjectType;

namespace Quaver.API.Maps.Processors.Scoring
{
    public sealed class ScoreProcessorKeys : ScoreProcessor
    {
        /// <summary>
        ///     The version of the processor.
        /// </summary>
        public static string Version { get; } = "0.0.1";

        /// <summary>
        ///     The maximum amount of judgements.
        ///
        ///     Each normal object counts as 1.
        ///     Additionally a long note counts as two (Beginning and End).
        /// </summary>
        private int TotalJudgements { get; }

        /// <summary>
        ///     See: ScoreProcessorKeys.CalculateSummedScore();
        /// </summary>
        private int SummedScore { get; }

        /// <summary>
        ///     Counts consecutive hits for the score multiplier
        ///     Max Multiplier Count is MultiplierMaxIndex * MultiplierCountToIncreaseIndex
        /// </summary>
        private int MultiplierCount { get; set; }

        /// <summary>
        ///     After 10 hits, the multiplier index will increase.
        /// </summary>
        private int MultiplierIndex { get; set; }

        /// <summary>
        ///     Max Index for multiplier. Multiplier index can not increase more than this value.
        /// </summary>
        private int MultiplierMaxIndex { get; } = 15;

        /// <summary>
        ///     Multiplier Count needed in order to increase the Multiplier Index by 1.
        ///     By increasing this multiplier index, notes will be worth more score.
        /// </summary>
        private int MultiplierCountToIncreaseIndex { get; } = 10;

        /// <summary>
        ///     This determines the max score.
        /// </summary>
        private int MaxMultiplierCount => MultiplierMaxIndex * MultiplierCountToIncreaseIndex;

        /// <summary>
        ///     Total actual score. (Regular Score is ScoreCount / SummedScore)
        /// </summary>
        private int ScoreCount { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public override SortedDictionary<Judgement, float> JudgementWindow { get; set; } = new SortedDictionary<Judgement, float>
        {
            {Judgement.Marv, 18},
            {Judgement.Perf, 43},
            {Judgement.Great, 76},
            {Judgement.Good, 106},
            {Judgement.Okay, 127},
            {Judgement.Miss, 164}
        };

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public override SortedDictionary<Judgement, float> WindowReleaseMultiplier { get; } = new SortedDictionary<Judgement, float>
        {
            {Judgement.Marv, 1.5f},
            {Judgement.Perf, 1.5f},
            {Judgement.Great, 1.5f},
            {Judgement.Good, 1.5f},
            {Judgement.Okay, 1.5f},
        };

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public override Dictionary<Judgement, int> JudgementScoreWeighting { get; } = new Dictionary<Judgement, int>()
        {
            {Judgement.Marv, 100},
            {Judgement.Perf, 50},
            {Judgement.Great, 25},
            {Judgement.Good, 10},
            {Judgement.Okay, 5},
            {Judgement.Miss, 0}
        };

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public override Dictionary<Judgement, float> JudgementHealthWeighting { get; } = new Dictionary<Judgement, float>()
        {
            {Judgement.Marv, 0.5f},
            {Judgement.Perf, 0.4f},
            {Judgement.Great, 0.2f},
            {Judgement.Good, -3.0f},
            {Judgement.Okay, -4.5f},
            {Judgement.Miss, -6.0f}
        };

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public override Dictionary<Judgement, float> JudgementAccuracyWeighting { get; } = new Dictionary<Judgement, float>()
        {
            {Judgement.Marv, 100},
            {Judgement.Perf, 98.25f},
            {Judgement.Great, 65},
            {Judgement.Good, 25},
            {Judgement.Okay, -100},
            {Judgement.Miss, -50}
        };

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="map"></param>
        /// <param name="mods"></param>
        /// <param name="windows"></param>
        public ScoreProcessorKeys(Qua map, ModIdentifier mods, JudgementWindows windows = null) : base(map, mods, windows)
        {
            TotalJudgements = GetTotalJudgementCount();
            SummedScore = CalculateSummedScore();
            InitializeHealthWeighting();
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="map"></param>
        /// <param name="mods"></param>
        /// <param name="multiplayer"></param>
        /// <param name="windows"></param>
        public ScoreProcessorKeys(Qua map, ModIdentifier mods, ScoreProcessorMultiplayer multiplayer, JudgementWindows windows = null) : base(map, mods, multiplayer, windows)
        {
            TotalJudgements = GetTotalJudgementCount();
            SummedScore = CalculateSummedScore();
            InitializeHealthWeighting();
        }

        /// <summary>
        ///     Ctor -
        /// </summary>
        /// <param name="replay"></param>
        /// <param name="windows"></param>
        public ScoreProcessorKeys(Replay replay, JudgementWindows windows = null) : base(replay, windows){}

        /// <summary>
        ///     Accuracy Calculation component of CalculateScore() if a note has been pressed/released properly
        /// </summary>
        /// <param name="hitDifference"></param>
        /// <param name="keyPressType"></param>
        /// <param name="calculateAllStats"></param>
        /// <param name="isMine"></param>
        public Judgement CalculateScore(int hitDifference, KeyPressType keyPressType, bool calculateAllStats = true,
            bool isMine = false)
        {
            var absoluteDifference = 0;

            if (hitDifference != int.MinValue)
                absoluteDifference = Math.Abs(hitDifference);
            else
                return Judgement.Miss;

            var judgement = Judgement.Ghost;

            // Find judgement of hit
            for (var i = 0; i < JudgementWindow.Count; i++)
            {
                var j = (Judgement) i;

                // Handles the case of if you release too early on a LN.
                if (keyPressType == KeyPressType.Release && j == Judgement.Miss)
                    break;

                var window = keyPressType == KeyPressType.Release  ? JudgementWindow[j] * WindowReleaseMultiplier[j] : JudgementWindow[j];

                if (!(absoluteDifference <= window))
                    continue;

                // Releasing an LN late rounds the judgement up to what holding it forever would give
                // On Default*, this rounds up to a Good
                if (keyPressType == KeyPressType.Release && hitDifference < 0 && i > (int)Windows.LNMissJudgement)
                {
                    judgement = Windows.LNMissJudgement.Value;
                    break;
                }

                // Make Okays no longer possible on releases (good window increases) unless LNMissJudgement is Okay or worse
                if (keyPressType == KeyPressType.Release && j == Judgement.Okay && (int)Windows.LNMissJudgement < (int)Judgement.Okay)
                {
                    judgement = Judgement.Good;
                    break;
                }

                judgement = j;
                break;
            }

            // If the press/release was outside of hit window, do not score.
            if (judgement == Judgement.Ghost)
                return judgement;

            if (calculateAllStats)
                CalculateScore(judgement, keyPressType == KeyPressType.Release, isMine);

            return judgement;
        }

        public void CalculateScore(HitStat hitStat)
        {
            CalculateScore(hitStat.Judgement, hitStat.KeyPressType == KeyPressType.Release,
                hitStat.HitObject.Type is HitObjectType.Mine);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Calculate Score and Health increase/decrease with a given judgement.
        /// </summary>
        /// <param name="judgement"></param>
        /// <param name="isLongNoteRelease"></param>
        /// <param name="isMine"></param>
        public override void CalculateScore(Judgement judgement, bool isLongNoteRelease = false, bool isMine = false)
        {
            // Update Judgement count
            CurrentJudgements[judgement]++;

            // Calculate and set the new accuracy.
            Accuracy = CalculateAccuracy();

#region SCORE_CALCULATION
            var comboBreakJudgement = Windows?.ComboBreakJudgement ?? Judgement.Miss;

            if (comboBreakJudgement == Judgement.Marv || comboBreakJudgement == Judgement.Ghost)
                comboBreakJudgement = Judgement.Miss;

            // If the user didn't miss, then we want to update their combo and multiplier.
            if (judgement < comboBreakJudgement)
            {
                //Update Multiplier
                if (judgement == Judgement.Good)
                    MultiplierCount -= MultiplierCountToIncreaseIndex;
                else
                    MultiplierCount++;

                // Add to the combo since the user hit.
                // Only do this when the note is not a mine (so it is a regular note)
                if (!isMine)
                    Combo++;

                // Set the max combo if applicable.
                if (Combo > MaxCombo)
                    MaxCombo = Combo;
            }
            // The user missed, so we want to decrease their multipler by 2 indexes and reset their combo.
            else
            {
                MultiplierCount -= MultiplierCountToIncreaseIndex * 2;
                Combo = 0;

                if (Mods.HasFlag(ModIdentifier.NoMiss))
                {
                    Health = 0;
                    ForceFail = true;
                    return;
                }
            }

            // Make sure the multiplier count doesn't go below 0
            if (MultiplierCount < 0)
                MultiplierCount = 0;
            // Make sure the multiplier count is not over max multiplier count
            else if (MultiplierCount > MaxMultiplierCount)
                MultiplierCount = MaxMultiplierCount;

            // Update multiplier index and score count.
            MultiplierIndex = (int)Math.Floor((float)MultiplierCount/ MultiplierCountToIncreaseIndex);
            ScoreCount += JudgementScoreWeighting[judgement] + MultiplierIndex * MultiplierCountToIncreaseIndex;

            // Update total score.
            const float standardizedMaxScore = 1000000;
            Score = (int)(standardizedMaxScore * ((double)ScoreCount / SummedScore));
#endregion

#region HEALTH_CALCULATION
            var newHealth = Health += JudgementHealthWeighting[judgement];

            // Constrain health from 0-100
            if (newHealth <= 0)
                Health = 0;
            else if (newHealth >= 100)
                Health = 100;
            else
                Health = newHealth;

            // If we're in multiplayer, handle health accordingly (lives, etc.)
            MultiplayerProcessor?.CalculateHealth();
#endregion
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <returns></returns>
        protected override float CalculateAccuracy()
        {
            float accuracy = 0;

            foreach (var item in CurrentJudgements)
                accuracy += item.Value * JudgementAccuracyWeighting[item.Key];

            return Math.Max(accuracy / (TotalJudgementCount * JudgementAccuracyWeighting[Judgement.Marv]), 0) * JudgementAccuracyWeighting[Judgement.Marv];
        }

        /// <summary>
        ///     Calculates the final health weighting values for each judgement from
        ///     average actions per second. <see cref="Qua.GetActionsPerSecond"/>
        ///
        ///     Resource: https://www.desmos.com/calculator/veeobxirvz
        /// </summary>
        protected override void InitializeHealthWeighting()
        {
            if (Mods.HasFlag(ModIdentifier.Autoplay))
                return;

            var density = Map.GetActionsPerSecond(ModHelper.GetRateFromMods(Mods));

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (density == 0 || density >= 12 || double.IsNaN(density))
                return;

            // Set baseline density to 2
            if (density > 0 && density < 2)
                density = 2;

            var values = new Dictionary<Judgement, Tuple<float, float>>
            {
                {Judgement.Marv, new Tuple<float, float>(-0.14f, 2.68f)},
                {Judgement.Perf, new Tuple<float, float>(-0.2f, 3.4f)},
                {Judgement.Great, new Tuple<float, float>(-0.14f, 2.68f)},
                {Judgement.Good, new Tuple<float, float>(0.084f, -0.008f)},
                {Judgement.Okay, new Tuple<float, float>(0.081f, 0.028f)}
            };

            foreach (var item in values)
            {
                var val = values[item.Key];
                var multiplier = val.Item1 * density + val.Item2;

                var weight = JudgementHealthWeighting[item.Key];
                JudgementHealthWeighting[item.Key] = (float) Math.Round(multiplier * weight, 2, MidpointRounding.ToEven);
            }
        }

        /// <summary>
        ///     Gets the absolute total judgements of the map.
        ///
        ///     Every normal object counts as 1 judgement.
        ///     Every LN counts as 2 judgements (Beginning + End)
        /// </summary>
        /// <returns></returns>
        public int GetTotalJudgementCount()
        {
            return Map.HitObjects.Sum(o => o.JudgementCount);
        }

        /// <summary>
        ///     Calculates the max score you can achieve in a song.
        ///         (Note: It assumes every hit is a Marv)
        ///
        ///     The user's current score is divided by this value then multiplied by 1,000,000
        ///     to get the score out of a million - the true max score.
        /// </summary>
        /// <returns></returns>
        private int CalculateSummedScore()
        {
            var summedScore = 0;

            // Multiplier doesn't increase after this amount.
            var maxMultiplierCount = MultiplierMaxIndex * MultiplierCountToIncreaseIndex;

            // Calculate score for notes below max multiplier combo
            // Note: This block could be a constant for songs that have max combo that exceeds the max multiplier count, but that will mean we will have to manually change the constant everytime we update any single other constant.
            for (var i = 1; i <= TotalJudgements && i < maxMultiplierCount; i++)
                summedScore += JudgementScoreWeighting[Judgement.Marv] + MultiplierCountToIncreaseIndex * (int) Math.Floor((float)i / MultiplierCountToIncreaseIndex);

            // Calculate score for notes once max multiplier combo is reached
            if (TotalJudgements >= maxMultiplierCount)
                summedScore += (TotalJudgements - (maxMultiplierCount - 1)) * (JudgementScoreWeighting[Judgement.Marv] + maxMultiplierCount);

            return summedScore;
        }
    }
}
