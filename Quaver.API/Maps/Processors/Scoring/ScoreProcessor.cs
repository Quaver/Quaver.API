/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Quaver.API.Enums;
using Quaver.API.Helpers;
using Quaver.API.Maps.Processors.Scoring.Data;
using Quaver.API.Maps.Processors.Scoring.Multiplayer;
using Quaver.API.Replays;

namespace Quaver.API.Maps.Processors.Scoring
{
    public abstract class ScoreProcessor
    {
        /// <summary>
        ///     The map that will have its score processed.
        /// </summary>
        public Qua Map { get;  }

        /// <summary>
        ///     The mods for this play.
        /// </summary>
        public ModIdentifier Mods { get; set; }

        /// <summary>
        ///     The total score the user has.
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        ///     The accuracy the user has.
        /// </summary>
        public float Accuracy { get; set; }

        /// <summary>
        ///     The current health.
        /// </summary>
        public float Health { get; set; } = 100;

        /// <summary>
        ///     The user's current combo.
        /// </summary>
        public int Combo { get; protected set; }

        /// <summary>
        ///     The max combo achieved for this play session.
        /// </summary>
        public int MaxCombo { get; set; }

        /// <summary>
        ///     If the score is currently failed.
        /// </summary>
        public bool Failed => Health <= 0 && !Mods.HasFlag(ModIdentifier.NoFail) || ForceFail;

        /// <summary>
        ///     Forces the game to fail
        /// </summary>
        public bool ForceFail { get; protected set; } = false;

        /// <summary>
        ///     The user's stats per object.
        /// </summary>
        public List<HitStat> Stats { get; set; }

        /// <summary>
        ///     The judgement window preset used for the score
        /// </summary>
        public JudgementWindows Windows { get; set; }

        /// <summary>
        ///     The username of the player achieving the score
        /// </summary>
        public string PlayerName { get; set; } = "";

        /// <summary>
        ///     The date and time the score was set
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        ///     The Steam id of the user who set the score - typically used for online scores.
        /// </summary>
        public ulong SteamId { get; set; }

        /// <summary>
        ///     The online id of the user
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        ///     For a standardized scoring reference. Should be manually set. Null by default
        /// </summary>
        public ScoreProcessor StandardizedProcessor { get; set; }

        /// <summary>
        ///     The judgement count for each judgement, initialized to 0 by default.
        ///
        ///     Note: Not sure if modes will use different judgements, probably not.
        /// </summary>
        public Dictionary<Judgement, int> CurrentJudgements { get; } = new Dictionary<Judgement, int>()
        {
            {Judgement.Marv, 0},
            {Judgement.Perf, 0},
            {Judgement.Great, 0},
            {Judgement.Good, 0},
            {Judgement.Okay, 0},
            {Judgement.Miss, 0}
        };

        /// <summary>
        ///     The judgement windows defined per mode.
        /// </summary>
        public abstract SortedDictionary<Judgement, float> JudgementWindow { get; set; }

        /// <summary>
        ///     The weighting for score defined per mode.
        /// </summary>
        public abstract Dictionary<Judgement, int> JudgementScoreWeighting { get; }

        /// <summary>
        ///     The weighting for health defined per mode.
        /// </summary>
        public abstract Dictionary<Judgement, float> JudgementHealthWeighting { get; }

        /// <summary>
        ///     The weighting for accuracy.
        /// </summary>
        public abstract Dictionary<Judgement, float> JudgementAccuracyWeighting { get; }

        /// <summary>
        ///     The window multiplier for long notes.
        ///     It multiplies the judgement window by this amount.
        /// </summary>
        public abstract SortedDictionary<Judgement, float> WindowReleaseMultiplier { get; }

        /// <summary>
        ///     Determined by if the player has obtained a full combo in the score.
        ///     This value can be applicable during real-time gameplay.
        /// </summary>
        public bool FullCombo => MaxCombo == TotalJudgementCount;

        /// <summary>
        ///     The total amount of judgements that the user has gotten in this play session.
        /// </summary>
        public virtual int TotalJudgementCount
        {
            get
            {
                var sum = 0;

                foreach (var item in CurrentJudgements)
                    sum += item.Value;

                return sum;
            }
        }

        /// <summary>
        ///     Contains everything related to multiplayer scoring.
        /// </summary>
        public ScoreProcessorMultiplayer MultiplayerProcessor { get; }

        /// <summary>
        ///     Ctor -
        /// </summary>
        /// <param name="map"></param>
        /// <param name="mods"></param>
        /// <param name="windows"></param>
        public ScoreProcessor(Qua map, ModIdentifier mods, JudgementWindows windows = null)
        {
            Map = map;
            Mods = mods;
            Stats = new List<HitStat>();

            InitializeJudgementWindows(windows);
            InitializeMods();
        }

        /// <summary>
        ///     For multiplayer
        /// </summary>
        /// <param name="map"></param>
        /// <param name="mods"></param>
        /// <param name="multiplayerProcessor"></param>
        /// <param name="windows"></param>
        public ScoreProcessor(Qua map, ModIdentifier mods, ScoreProcessorMultiplayer multiplayerProcessor, JudgementWindows windows = null)
            : this(map, mods, windows)
        {
            MultiplayerProcessor = multiplayerProcessor;
            MultiplayerProcessor.Processor = this;
        }

        /// <summary>
        ///     Score processor from replay.
        /// </summary>
        /// <param name="replay"></param>
        /// <param name="windows"></param>
        public ScoreProcessor(Replay replay, JudgementWindows windows = null)
        {
            Mods = replay.Mods;
            Score = replay.Score;
            Accuracy = replay.Accuracy;
            MaxCombo = replay.MaxCombo;
            PlayerName = replay.PlayerName ?? "";
            Date = replay.Date;
            CurrentJudgements[Judgement.Marv] = replay.CountMarv;
            CurrentJudgements[Judgement.Perf] = replay.CountPerf;
            CurrentJudgements[Judgement.Great] = replay.CountGreat;
            CurrentJudgements[Judgement.Good] = replay.CountGood;
            CurrentJudgements[Judgement.Okay] = replay.CountOkay;
            CurrentJudgements[Judgement.Miss] = replay.CountMiss;

            InitializeJudgementWindows(windows);
            InitializeMods();
        }

        /// <summary>
        ///     Adds a judgement to the score and recalculates the score.
        /// </summary>
        public abstract void CalculateScore(Judgement judgement, bool isLongNoteRelease = false);

        /// <summary>
        ///     Calculates the accuracy of the current play session.
        /// </summary>
        /// <returns></returns>
        protected abstract float CalculateAccuracy();

        /// <summary>
        ///     Changes the judgement windows for the processor
        /// </summary>
        private void InitializeJudgementWindows(JudgementWindows windows)
        {
            Windows = windows ?? new JudgementWindows
            {
                Name = "Standard*",
                IsDefault = true
            };

            JudgementWindow[Judgement.Marv] = Windows.Marvelous;
            JudgementWindow[Judgement.Perf] = Windows.Perfect;
            JudgementWindow[Judgement.Great] = Windows.Great;
            JudgementWindow[Judgement.Good] = Windows.Good;
            JudgementWindow[Judgement.Okay] = Windows.Okay;
            JudgementWindow[Judgement.Miss] = Windows.Miss;
        }

        /// <summary>
        ///     Initializes the mods for this given play.
        ///     (Recalculates hit windows.)
        /// </summary>
        private void InitializeMods()
        {
            for (var i = 0; i < JudgementWindow.Count; i++)
                JudgementWindow[(Judgement) i] *= ModHelper.GetRateFromMods(Mods);
        }

        /// <summary>
        ///     Initializes the health weighting of the map
        /// </summary>
        protected abstract void InitializeHealthWeighting();

        /// <summary>
        ///     Gets the judgement breakdown from hit data.
        /// </summary>
        /// <returns></returns>
        public string GetJudgementBreakdown()
        {
            var breakdown = "";
            Stats.ForEach(x => breakdown += (int) x.Judgement);

            return breakdown;
        }

        /// <summary>
        ///     Computes the hit statistics from the hit data.
        /// </summary>
        /// <returns></returns>
        public HitStatistics GetHitStatistics()
        {
            var largestHitWindow = JudgementWindow.Values.Max();
            var hitDifferences = new List<int>();
            var sum = 0;

            foreach (var breakdown in Stats)
            {
                var hitDifference = breakdown.HitDifference;

                // The LN releases are _not_ scaled here because we want an accurate mean.

                // No need to check for Type == Miss as all of them have hitDifference == int.MinValue.
                if (hitDifference != int.MinValue && Math.Abs(hitDifference) <= largestHitWindow)
                {
                    hitDifferences.Add(hitDifference);
                    sum += hitDifference; // This overflows at like 13 million max judgements.
                }
            }

            double mean = 0.0;
            double standardDeviation = 0.0;

            var count = hitDifferences.Count();
            if (count > 0)
            {
                mean = (double) sum / (double) count;
                standardDeviation = Math.Sqrt(hitDifferences.Average(v => Math.Pow(v - mean, 2)));

                // Undo the rate scaling.
                mean /= ModHelper.GetRateFromMods(Mods);
                // Since variance(ax) = a^2 variance(x), then std(ax) = a std(x)
                standardDeviation /= ModHelper.GetRateFromMods(Mods);
            }

            return new HitStatistics
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };
        }
    }
}
