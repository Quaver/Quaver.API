/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using System.Text.RegularExpressions;
using Quaver.API.Enums;
using Quaver.API.Maps.Structures;

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
        ///     The type f key press this was for this stat.
        /// </summary>
        public KeyPressType KeyPressType { get; }

        /// <summary>
        ///     The HitObject that this is referencing to.
        /// </summary>
        public HitObjectInfo HitObject { get; }

        /// <summary>
        ///     The position in the song the object was hit.
        /// </summary>
        public int SongPosition { get; }

        /// <summary>
        ///     The judgement received for this hit.
        /// </summary>
        public Judgement Judgement { get; }

        /// <summary>
        ///     The difference between the the hit and the song position.
        /// </summary>
        public int HitDifference { get; }

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
        /// <param name="type"></param>
        /// <param name="keyPressType"></param>
        /// <param name="hitObject"></param>
        /// <param name="songPos"></param>
        /// <param name="judgement"></param>
        /// <param name="hitDifference"></param>
        /// <param name="acc"></param>
        /// <param name="health"></param>
        public HitStat(HitStatType type, KeyPressType keyPressType, HitObjectInfo hitObject, int songPos,
            Judgement judgement, int hitDifference, double acc, float health)
        {
            HitObject = hitObject;
            SongPosition = songPos;
            Judgement = judgement;
            HitDifference = hitDifference;
            Accuracy = acc;
            Health = health;
            Type = type;
            KeyPressType = keyPressType;
        }

        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="keyPressType"></param>
        /// <param name="hitDifference"></param>
        public HitStat(HitStatType type, KeyPressType keyPressType, int hitDifference)
        {
            Type = type;
            KeyPressType = keyPressType;
            HitDifference = hitDifference;

            HitObject = null;
            Judgement = Judgement.Ghost;
            SongPosition = 0;
            Accuracy = 0;
            Health = 0;
        }

        /// <summary>
        ///     Parse an individual breakdown.
        /// </summary>
        /// <param name="breakdownItem"></param>
        /// <returns></returns>
        public static HitStat FromBreakdownItem(string breakdownItem)
        {
            var match = Regex.Match(breakdownItem, @"^([-]?[\d]+)([N|P|R])$");

            if (!match.Success)
                throw new ArgumentException("breakdownItem doesn't match the specified format: " + breakdownItem);

            KeyPressType keyPressType;

            switch (match.Groups[2].Value)
            {
                case "N":
                    keyPressType = KeyPressType.None;
                    break;
                case "P":
                    keyPressType = KeyPressType.Press;
                    break;
                case "R":
                    keyPressType = KeyPressType.Release;
                    break;
                default:
                    throw new ArgumentException("Breakdown Item does not have a correct KeyPressType");
            }

            return new HitStat(HitStatType.Hit, keyPressType, int.Parse(match.Groups[1].Value));
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{SongPosition}|{KeyPressType}|{HitDifference}|{Judgement} @ HitObject {HitObject?.StartTime} ({HitObject?.Lane})";
    }
}
