/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys.Structures
{
    public class HandStateData
    {
        /// <summary>
        ///     Reference to the Other Hand State from a different hand which chords with this state.
        /// </summary>
        public HandStateData ChordedHand { get; set; }

        /// <summary>
        ///     All HitObjects referenced for this Hand State
        /// </summary>
        public List<DPHitObject> HitObjects { get; } = new List<DPHitObject>();

        /// <summary>
        ///     Reference Timing Position for this State. It will use the StartTime of the first HitObject that initialized this object.
        /// </summary>
        public double Time => HitObjects[0].StartTime;

        /// <summary>
        ///     Determined by the hand this data point focuses on.
        /// </summary>
        public Hand Hand { get; }

        /// <summary>
        ///     Determined by the current state of each finger.
        /// </summary>
        public Finger FingerState { get; private set; }

        /// <summary>
        ///     Determined by how close the hitobjects are to becoming a perfect chord.
        ///     0 = Perfect Chord
        ///     1 = Furthest from being a perfect chord
        /// </summary>
        public double ChordProximity { get; private set; }

        /// <summary>
        ///     Difficulty for this specific Hand State
        /// </summary>
        public double StateDifficulty { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hitObjects"></param>
        public HandStateData(DPHitObject hitObject)
        {
            Hand = hitObject.Hand;
            AddHitObjectToChord(hitObject);
        }

        /// <summary>
        ///     Add HitObject as a Chord to this state.
        /// </summary>
        /// <param name="hitObjects"></param>
        public void AddHitObjectToChord(DPHitObject hitObject) => HitObjects.Add(hitObject);

        /// <summary>
        ///     Evaluate Difficulty for this specific Hand State.
        /// </summary>
        /// <param name="constants"></param>
        /// <param name="nextState"></param>
        public double EvaluateDifficulty(DifficultyConstantsKeys constants, double actionLength)
        {
            StateDifficulty = 0;
            FingerState = Finger.None;
            HitObjects.ForEach
            (
                x =>
                {
                    FingerState |= x.FingerState;
                    x.EvaluateDifficulty(constants);
                    StateDifficulty += x.Difficulty;
                }
            );

            StateDifficulty /= HitObjects.Count;

            // Apply Chord Multiplier if Other Hand is chorded with this hand.
            if (ChordedHand != null)
                StateDifficulty *= constants.TwoHandedChordMultiplier.Value;

            ;// Action Length must be greater than 0
            if (actionLength <= 0)
                throw new Exception("HandStateData Action Delta is 0 or negative value.");

            // Set and Return Difficulty Value
            return StateDifficulty = Math.Max(1, StateDifficulty * constants.DifficultyMultiplier * (double)Math.Sqrt(constants.BpmToActionLengthMs / actionLength) + constants.DifficultyOffset);
        }

        /// <summary>
        ///     Will determine the Chord Proximity of this Hand State.
        ///     Todo: this method is unused for now.
        ///     Todo: Ideally, it'll be used to interpolate between chord/non-chord for Diff-Calc, but it is not efficient.
        /// </summary>
        private void CalculateChordProximity(DifficultyConstantsKeys constants)
        {
            // There should not be 0 HitObjects in the Hand State
            if (HitObjects.Count == 0)
                throw new Exception("HitObject List for HandStateData has no HitObjects");

            // Set Finger State to appropriate value and calculate ChordProximity
            var lead = HitObjects[0];
            DPHitObject furthest = null;
            foreach (var ob in HitObjects)
            {
                if (DifficultyProcessorKeys.LaneToHand4K[ob.HitObject.Lane] != Hand)
                {
                    if (furthest == null || ob.HitObject.StartTime > furthest.HitObject.StartTime)
                        furthest = ob;
                }
            }

            // Calculate Chord Proximity from furthest chord
            ChordProximity = furthest == null ? 0 : Math.Abs(furthest.HitObject.StartTime - lead.HitObject.StartTime) / constants.ChordThresholdOtherHandMs;
        }
    }
}
