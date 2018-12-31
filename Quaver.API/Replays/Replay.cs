/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using osu_database_reader;
using Quaver.API.Enums;
using Quaver.API.Helpers;
using Quaver.API.Maps;
using Quaver.API.Maps.Processors.Scoring;

namespace Quaver.API.Replays
{
    public class Replay
    {
        /// <summary>
        ///     The version of the replay.
        /// </summary>
        public static string Version { get; } = "None";

        /// <summary>
        ///     The game mode this replay is for.
        /// </summary>
        public GameMode Mode { get; set; }

        /// <summary>
        ///     All of the replay frames.
        /// </summary>
        public List<ReplayFrame> Frames { get; }

        /// <summary>
        ///    The version of the replay the play was done on.
        /// </summary>
        public string ReplayVersion { get; set; }

        /// <summary>
        ///    The name of the player.
        /// </summary>
        public string PlayerName { get; set; }

        /// <summary>
        ///     The activated mods on this replay.
        /// </summary>
        public ModIdentifier Mods { get; set; }

        /// <summary>
        ///     The date of this replay.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        ///     The in milliseconds the score was played at.
        /// </summary>
        public long TimePlayed { get; set; }

        /// <summary>
        ///     The MD5 Hash of the replay.
        /// </summary>
        public string Md5 { get; }

        /// <summary>
        ///     The md5 hash of the map.
        /// </summary>
        public string MapMd5 { get; set; }

        /// <summary>
        ///     The score achieved
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        ///     The accuracy achieved
        /// </summary>
        public float Accuracy { get; set; }

        /// <summary>
        ///     The max combo achieved
        /// </summary>
        public int MaxCombo { get; set; }

        /// <summary>
        ///     Amount of marv judgements
        /// </summary>
        public int CountMarv { get; set; }

        /// <summary>
        ///     Amount of perf judgements
        /// </summary>
        public int CountPerf { get; set; }

        /// <summary>
        ///     Amount of great judgements
        /// </summary>
        public int CountGreat { get; set; }

        /// <summary>
        ///     Amount of good judgements
        /// </summary>
        public int CountGood { get; set; }

        /// <summary>
        ///     Amount of okay judgements
        /// </summary>
        public int CountOkay { get; set; }

        /// <summary>
        ///     Amount of miss judgements.
        /// </summary>
        public int CountMiss { get; set; }

        /// <summary>
        ///     The amount of times paused in the play.
        /// </summary>
        public int PauseCount { get; set; }

        /// <summary>
        ///     Ctor -
        ///     Create fresh replay
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="name"></param>
        /// <param name="mods"></param>
        /// <param name="md5"></param>
        public Replay(GameMode mode, string name, ModIdentifier mods, string md5)
        {
            PlayerName = name;
            Mode = mode;
            Mods = mods;
            MapMd5 = md5;
            Frames = new List<ReplayFrame>();
        }

        /// <summary>
        ///     Ctor - Read replay.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="readHeaderless"></param>
        public Replay(string path, bool readHeaderless = false)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();

            // Read the replay data
            using (var fs = new FileStream(path, FileMode.Open))
            using (var br = new BinaryReader(fs))
            {
                if (!readHeaderless)
                {
                    // Version None (Original data)
                    ReplayVersion = br.ReadString();
                    MapMd5 = br.ReadString();
                    Md5 = br.ReadString();
                    PlayerName = br.ReadString();
                    Date = Convert.ToDateTime(br.ReadString(), CultureInfo.InvariantCulture);
                    TimePlayed = br.ReadInt64();

                    // The dates are serialized incorrectly in older replays, so to keep compatability,
                    // use the time played.
                    Date = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddMilliseconds(TimePlayed);

                    Mode = (GameMode)br.ReadInt32();
                    Mods = (ModIdentifier)br.ReadInt32();
                    Score = br.ReadInt32();
                    Accuracy = br.ReadSingle();
                    MaxCombo = br.ReadInt32();
                    CountMarv = br.ReadInt32();
                    CountPerf = br.ReadInt32();
                    CountGreat = br.ReadInt32();
                    CountGood = br.ReadInt32();
                    CountOkay = br.ReadInt32();
                    CountMiss = br.ReadInt32();
                    PauseCount = br.ReadInt32();
                }

                // Create the new list of replay frames.
                Frames = new List<ReplayFrame>();

                // Split the frames up by commas
                var frames = new List<string>();

                if (!readHeaderless)
                {
                    frames = Encoding.ASCII.GetString(LZMACoder.Decompress(br.BaseStream).ToArray()).Split(',').ToList();
                }
                else
                {
                    frames = Encoding.ASCII.GetString(LZMACoder.Decompress(br.ReadBytes((int) br.BaseStream.Length))).Split(',').ToList();
                }

                // Add all the replay frames to the object
                foreach (var frame in frames)
                {
                    try
                    {
                        // Split up the frame string by SongTime|KeyPressState
                        var frameSplit = frame.Split('|');

                        Frames.Add(new ReplayFrame(int.Parse(frameSplit[0]), (ReplayKeyPressState)Enum.Parse(typeof(ReplayKeyPressState), frameSplit[1])));
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }
            }
        }

        /// <summary>
        ///    Writes the current replay to a binary file.
        /// </summary>
        public void Write(string path)
        {
            var frames = FramesToString();

            using (var replayDataStream = new MemoryStream(Encoding.ASCII.GetBytes(frames)))
            using (var bw = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                bw.Write(ReplayVersion);
                bw.Write(MapMd5);
                bw.Write(GetMd5(frames));
                bw.Write(PlayerName);
                bw.Write(DateTime.Now.ToString(CultureInfo.InvariantCulture));
                bw.Write(TimePlayed);
                bw.Write((int)Mode);
                bw.Write((int)Mods);
                bw.Write(Score);
                bw.Write(Accuracy);
                bw.Write(MaxCombo);
                bw.Write(CountMarv);
                bw.Write(CountPerf);
                bw.Write(CountGreat);
                bw.Write(CountGood);
                bw.Write(CountOkay);
                bw.Write(CountMiss);
                bw.Write(PauseCount);
                bw.Write(StreamHelper.ConvertStreamToByteArray(LZMACoder.Compress(replayDataStream)));
            }
        }

        /// <summary>
        ///     Adds a frame to the replay.
        /// </summary>
        public void AddFrame(int time, ReplayKeyPressState keys) => Frames.Add(new ReplayFrame(time, keys));

        /// <summary>
        ///    Populates the replay header properties from a score processor.
        /// </summary>
        public void FromScoreProcessor(ScoreProcessor processor)
        {
            Score = processor.Score;
            Accuracy = processor.Accuracy;
            MaxCombo = processor.MaxCombo;
            CountMarv = processor.CurrentJudgements[Judgement.Marv];
            CountPerf = processor.CurrentJudgements[Judgement.Perf];
            CountGreat = processor.CurrentJudgements[Judgement.Great];
            CountGood = processor.CurrentJudgements[Judgement.Good];
            CountOkay = processor.CurrentJudgements[Judgement.Okay];
            CountMiss = processor.CurrentJudgements[Judgement.Miss];
        }

        /// <summary>
        ///     Generates a perfect replay for the keys game mode.
        /// </summary>
        /// <param name="replay"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public static Replay GeneratePerfectReplayKeys(Replay replay, Qua map)
        {
            var nonCombined = new List<ReplayAutoplayFrame>();

            foreach (var hitObject in map.HitObjects)
            {
                // Add key press frame
                nonCombined.Add(new ReplayAutoplayFrame(hitObject, ReplayAutoplayFrameType.Press, hitObject.StartTime, KeyLaneToPressState(hitObject.Lane)));

                // If LN, add key up state at end time
                if (hitObject.IsLongNote)
                    nonCombined.Add(new ReplayAutoplayFrame(hitObject, ReplayAutoplayFrameType.Release, hitObject.EndTime - 1, KeyLaneToPressState(hitObject.Lane)));
                // If not ln, add key up frame 1ms after object.
                else
                    nonCombined.Add(new ReplayAutoplayFrame(hitObject, ReplayAutoplayFrameType.Release, hitObject.StartTime + 30, KeyLaneToPressState(hitObject.Lane)));
            }

            // Order objects by time
            nonCombined = nonCombined.OrderBy(x => x.Time).ToList();

            // Global replay state so we can loop through in track it.
            ReplayKeyPressState state = 0;

            // Add beginning frame w/ no press state. (-10000 just to be on the safe side.)
            replay.Frames.Add(new ReplayFrame(-10000, 0));

            var startTimeGroup = nonCombined.GroupBy(x => x.Time).ToDictionary(x => x.Key, x => x.ToList());

            foreach (var item in startTimeGroup)
            {
                foreach (var frame in item.Value)
                {
                    switch (frame.Type)
                    {
                        case ReplayAutoplayFrameType.Press:
                            state |= KeyLaneToPressState(frame.HitObject.Lane);
                            break;
                        case ReplayAutoplayFrameType.Release:
                            state -= KeyLaneToPressState(frame.HitObject.Lane);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                //Console.WriteLine($"Added frame at: {item.Key} with state: {state}");
                replay.Frames.Add(new ReplayFrame(item.Key, state));
            }

            return replay;
        }

        /// <summary>
        ///     Converts a lane to a key press state.
        /// </summary>
        /// <param name="lane"></param>
        /// <returns></returns>
        public static ReplayKeyPressState KeyLaneToPressState(int lane)
        {
            switch (lane)
            {
                case 1:
                    return ReplayKeyPressState.K1;
                case 2:
                    return ReplayKeyPressState.K2;
                case 3:
                    return ReplayKeyPressState.K3;
                case 4:
                    return ReplayKeyPressState.K4;
                case 5:
                    return ReplayKeyPressState.K5;
                case 6:
                    return ReplayKeyPressState.K6;
                case 7:
                    return ReplayKeyPressState.K7;
                default:
                    throw new ArgumentException("Lane specified must be between 1 and 7");
            }
        }

        /// <summary>
        ///     Converts a key press state to a list of lanes that are active.
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static List<int> KeyPressStateToLanes(ReplayKeyPressState keys)
        {
            var lanes = new List<int>();

            if (keys.HasFlag(ReplayKeyPressState.K1))
                lanes.Add(0);
            if (keys.HasFlag(ReplayKeyPressState.K2))
                lanes.Add(1);
            if (keys.HasFlag(ReplayKeyPressState.K3))
                lanes.Add(2);
            if (keys.HasFlag(ReplayKeyPressState.K4))
                lanes.Add(3);
            if (keys.HasFlag(ReplayKeyPressState.K5))
                lanes.Add(4);
            if (keys.HasFlag(ReplayKeyPressState.K6))
                lanes.Add(5);
            if (keys.HasFlag(ReplayKeyPressState.K7))
                lanes.Add(6);

             return lanes;
        }

        /// <summary>
        ///     Converts all replay frames to a string
        /// </summary>
        public string FramesToString(bool debug = false)
        {
            // The format for the replay frames are the following:
            //     Time|KeysPressed,
            var frameStr = "";

            if (debug)
                Frames.ForEach(x => frameStr += $"{x.ToDebugString()}\r\n");
            else
                Frames.ForEach(x => frameStr += $"{x.ToString()},");

            return frameStr;
        }

        /// <summary>
        ///     If the replay has any data in it.
        /// </summary>
        public bool HasData => Frames.Count > 0;

        /// <summary>
        ///     Gets all of the individual unique key presses during the replay.
        /// </summary>
        public List<ReplayKeyPressInfo> GetKeyPresses()
        {
            var keyPresses = new List<ReplayKeyPressInfo>();

            ReplayKeyPressState previousKeys = 0;

            foreach (var frame in Frames)
            {
                if (frame.Keys != previousKeys)
                {
                    var currentLanes = KeyPressStateToLanes(frame.Keys);
                    var previousLanes = KeyPressStateToLanes(previousKeys);

                    var keyDifferences = currentLanes.Except(previousLanes)
                        .Concat(previousLanes.Except(currentLanes))
                        .ToList();

                    foreach (var key in keyDifferences)
                    {
                        // key was pressed in this frame.
                        if (currentLanes.Contains(key))
                        {
                            keyPresses.Add(new ReplayKeyPressInfo(KeyLaneToPressState(key + 1), frame.Time));
                        }
                        // Key was released in this frame.
                        else if (previousLanes.Contains(key))
                        {
                            var foundPress = keyPresses.FindLast(x => x.Key == KeyLaneToPressState(key + 1));
                            foundPress.TimeReleased = frame.Time;
                        }
                    }
                }

                previousKeys = frame.Keys;
            }

            return keyPresses;
        }

        /// <summary>
        ///     Gets the md5 hash of the replay.
        /// </summary>
        /// <returns></returns>
        public string GetMd5(string frames)
        {
            return CryptoHelper.StringToMd5($"{ReplayVersion}-{TimePlayed}-{MapMd5}-{PlayerName}-{(int) Mode}-" +
                                     $"{(int) Mods}-{Score}-{Accuracy}-{MaxCombo}-{CountMarv}-{CountPerf}-" +
                                     $"{CountGreat}-{CountGood}-{CountOkay}-{CountMiss}-{PauseCount}-{frames}");
        }
    }
}
