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
using Quaver.API.Maps;
using Quaver.API.Maps.Processors.Scoring;
using Quaver.API.Maps.Processors.Scoring.Data;
using Quaver.API.Maps.Structures;

namespace Quaver.API.Replays.Virtual
{
    public class VirtualReplayPlayer
    {
         /// <summary>
        ///     The replay that's being played.
        /// </summary>
        public Replay Replay { get; }

        /// <summary>
        ///     The map played.
        /// </summary>
        public Qua Map { get; }

        /// <summary>
        ///     The score processor for the virtual replay.
        /// </summary>
        public ScoreProcessorKeys ScoreProcessor { get; }

        /// <summary>
        ///     All of the HitObjects that are currently active and available.
        /// </summary>
        public List<HitObjectInfo> ActiveHitObjects { get; }

        /// <summary>
        ///     All of the currently held active long notes.
        /// </summary>
        public List<HitObjectInfo> ActiveHeldLongNotes { get; }

        /// <summary>
        ///     The list of active HitObjects that are scheduled for removal.
        /// </summary>
        private List<HitObjectInfo> ActiveHitObjectsToRemove { get; set; }

        /// <summary>
        ///     The list of active held long notes that are scheduled for removal.
        /// </summary>
        private List<HitObjectInfo> ActiveHeldLongNotesToRemove { get; set; }

        /// <summary>
        ///     The current frame in the replay.
        /// </summary>
        public int CurrentFrame { get; private set; } = -1;

        /// <summary>
        ///     The current time in the replay.
        /// </summary>
        public int Time => Replay.Frames[CurrentFrame].Time;

        /// <summary>
        ///     Keeps track of if certain keys are pressed or not.
        /// </summary>
        public List<VirtualReplayKeyBinding> InputKeyStore { get; }

        /// <summary>
        /// </summary>
        /// <param name="replay"></param>
        /// <param name="map"></param>
        public VirtualReplayPlayer(Replay replay, Qua map)
        {
            Replay = replay;
            Map = map;

            ScoreProcessor = new ScoreProcessorKeys(map, Replay.Mods);

            ActiveHitObjects = new List<HitObjectInfo>();
            ActiveHeldLongNotes = new List<HitObjectInfo>();

            map.HitObjects.ForEach(x => ActiveHitObjects.Add(x));

            // Add virtual key bindings based on the game mode of the replay.
            switch (Map.Mode)
            {
                case GameMode.Keys4:
                    InputKeyStore = new List<VirtualReplayKeyBinding>()
                    {
                        new VirtualReplayKeyBinding(ReplayKeyPressState.K1),
                        new VirtualReplayKeyBinding(ReplayKeyPressState.K2),
                        new VirtualReplayKeyBinding(ReplayKeyPressState.K3),
                        new VirtualReplayKeyBinding(ReplayKeyPressState.K4),
                    };
                    break;
                case GameMode.Keys7:
                    InputKeyStore = new List<VirtualReplayKeyBinding>()
                    {
                        new VirtualReplayKeyBinding(ReplayKeyPressState.K1),
                        new VirtualReplayKeyBinding(ReplayKeyPressState.K2),
                        new VirtualReplayKeyBinding(ReplayKeyPressState.K3),
                        new VirtualReplayKeyBinding(ReplayKeyPressState.K4),
                        new VirtualReplayKeyBinding(ReplayKeyPressState.K5),
                        new VirtualReplayKeyBinding(ReplayKeyPressState.K6),
                        new VirtualReplayKeyBinding(ReplayKeyPressState.K7),
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Plays the next frame of the replay.
        /// </summary>
        public void PlayNextFrame()
        {
            if (CurrentFrame + 1 >= Replay.Frames.Count)
                throw new InvalidOperationException("Cannot play the next replay frame because there aren't any frames left!");

            CurrentFrame++;

            // Store the objects that need to be removed from the list of active objects.
            ActiveHitObjectsToRemove = new List<HitObjectInfo>();
            ActiveHeldLongNotesToRemove = new List<HitObjectInfo>();

            HandleKeyPressesInFrame();
            HandleMissedLongNoteReleases();
            HandleMissedHitObjects();
        }

        /// <summary>
        ///     Plays the entire replay.
        /// </summary>
        public void PlayAllFrames()
        {
            while (CurrentFrame + 1 < Replay.Frames.Count)
                PlayNextFrame();
        }

        /// <summary>
        ///     Handles all key presses in the current replay frame.
        /// </summary>
        private void HandleKeyPressesInFrame()
        {
            // Retrieve a list of the key press states in integer form.
            var currentFramePressed = Replay.KeyPressStateToLanes(Replay.Frames[CurrentFrame].Keys);
            var previousFramePressed = CurrentFrame > 0 ? Replay.KeyPressStateToLanes(Replay.Frames[CurrentFrame - 1].Keys) : new List<int>();

            // Update the key press state in the store.
            for (var i = 0; i < InputKeyStore.Count; i++)
                InputKeyStore[i].Pressed = currentFramePressed.Contains(i);

            // Check the difference in key press states for the current and previous frames.
            var keyDifferences = currentFramePressed.Except(previousFramePressed)
                                .Concat(previousFramePressed.Except(currentFramePressed))
                                .ToList();

            // Go through each frame and handle key presses/releases.
            foreach (var key in keyDifferences)
            {
                // This key was uniquely pressed during this frame.
                if (currentFramePressed.Contains(key))
                {
                    // Find the nearest object in the lane that the user has pressed.
                    var nearestObjectIndex = GetIndexOfNearestLaneObject(key + 1, Time);

                    if (nearestObjectIndex == -1)
                        continue;

                    // Grab the actual HitObject instance.
                    var hitObject = ActiveHitObjects[nearestObjectIndex];

                    // Calculate the hit difference.
                    var hitDifference = hitObject.StartTime - Time;

                    // Calculate Score.
                    var judgement = ScoreProcessor.CalculateScore(hitDifference, KeyPressType.Press);

                    switch (judgement)
                    {
                        // Don't handle ghost key presses, so just continue further.
                        case Judgement.Ghost:
                            continue;
                        // Object needs to be removed completely if it's a miss.
                        case Judgement.Miss:
                            break;
                        default:
                            // Long notes need to be changed to a held status.
                            if (hitObject.IsLongNote)
                                ActiveHeldLongNotes.Add(hitObject);
                            break;
                    }

                    // Add a new hit stat to the score processor.
                    var stat = new HitStat(HitStatType.Hit, KeyPressType.Press, hitObject, Time, judgement, hitDifference,
                        ScoreProcessor.Accuracy, ScoreProcessor.Health);

                    ScoreProcessor.Stats.Add(stat);

                    // Object needs to be removed from ActiveObjects.
                    ActiveHitObjectsToRemove.Add(hitObject);
                }
                // This key was uniquely released during this frame.
                else if (previousFramePressed.Contains(key))
                {
                    // Find the index of the actual closest LN and handle the key release
                    // if so.
                    foreach (var hitObject in ActiveHeldLongNotes)
                    {
                        // Handle the release of the note.
                        if (hitObject.Lane != key + 1)
                            continue;

                        // Calculate the hit difference.
                        var hitDifference = hitObject.EndTime - Time;

                        // Calculate Score
                        var judgement = ScoreProcessor.CalculateScore(hitDifference, KeyPressType.Release);

                        // LN was released during a hit window.
                        if (judgement != Judgement.Ghost)
                        {
                            // Add a new hit stat to the score processor.
                            var stat = new HitStat(HitStatType.Hit, KeyPressType.Release, hitObject, Time, judgement, hitDifference,
                                ScoreProcessor.Accuracy, ScoreProcessor.Health);

                            ScoreProcessor.Stats.Add(stat);
                        }
                        // The LN was released too early (miss)
                        else
                        {
                            ScoreProcessor.CalculateScore(Judgement.Miss);

                            // Add a new stat to ScoreProcessor.
                            var stat = new HitStat(HitStatType.Hit, KeyPressType.Release, hitObject, Time, Judgement.Miss, hitDifference,
                                ScoreProcessor.Accuracy, ScoreProcessor.Health);

                            ScoreProcessor.Stats.Add(stat);
                        }

                        // Remove the object from its held state.
                        ActiveHeldLongNotesToRemove.Add(hitObject);
                    }
                }
            }

            // Remove all active objects after handling key presses/releases.
            ActiveHitObjectsToRemove.ForEach(x => ActiveHitObjects.Remove(x));
            ActiveHeldLongNotesToRemove.ForEach(x => ActiveHeldLongNotes.Remove(x));
        }

        /// <summary>
        ///     Handles the replay frames for missed long notes.
        /// </summary>
        private void HandleMissedLongNoteReleases()
        {
            // Handle missed LN releases.
            foreach (var hitObject in ActiveHeldLongNotes)
            {
                var releaseWindow = ScoreProcessor.JudgementWindow[Judgement.Okay] * ScoreProcessor.WindowReleaseMultiplier[Judgement.Okay];

                // Check if the LN's release was missed.
                if (!(Time > hitObject.EndTime + releaseWindow))
                    continue;

                ScoreProcessor.CalculateScore(Judgement.Okay);

                // Add new miss stat.
                var stat = new HitStat(HitStatType.Miss, KeyPressType.None, hitObject, Time, Judgement.Okay, int.MinValue,
                    ScoreProcessor.Accuracy, ScoreProcessor.Health);

                ScoreProcessor.Stats.Add(stat);

                // Queue the object to be removed.
                ActiveHeldLongNotesToRemove.Add(hitObject);
            }

            ActiveHeldLongNotesToRemove.ForEach(x => ActiveHeldLongNotes.Remove(x));
        }

        /// <summary>
        ///     Handles completely missed HitObjects.
        /// </summary>
        private void HandleMissedHitObjects()
        {
            // Handle missed notes.
            foreach (var hitObject in ActiveHitObjects)
            {
                if (Time > hitObject.StartTime + ScoreProcessor.JudgementWindow[Judgement.Okay])
                {
                    // Add a miss to the score.
                    ScoreProcessor.CalculateScore(Judgement.Miss);

                    // Create a new HitStat to add to the ScoreProcessor.
                    var stat = new HitStat(HitStatType.Miss, KeyPressType.None, hitObject, Time, Judgement.Miss, int.MinValue,
                        ScoreProcessor.Accuracy, ScoreProcessor.Health);

                    ScoreProcessor.Stats.Add(stat);

                    // Long notes count as two misses, so add another one if the object is one.
                    if (hitObject.IsLongNote)
                    {
                        ScoreProcessor.CalculateScore(Judgement.Miss);
                        ScoreProcessor.Stats.Add(stat);
                    }

                    ActiveHitObjectsToRemove.Add(hitObject);
                }
                else
                {
                    break;
                }
            }

            // Remove all objects
            ActiveHitObjectsToRemove.ForEach(x => ActiveHitObjects.Remove(x));
            ActiveHeldLongNotesToRemove.ForEach(x => ActiveHeldLongNotes.Remove(x));
        }

        /// <summary>
        ///     Gets the index of the nearest object in a given lane.
        /// </summary>
        /// <param name="lane"></param>
        /// <param name="songTime"></param>
        /// <returns></returns>
        public int GetIndexOfNearestLaneObject(int lane, double songTime)
        {
            for (var i = 0; i < ActiveHitObjects.Count; i++)
            {
                if (ActiveHitObjects[i].Lane == lane && ActiveHitObjects[i].StartTime - songTime > -ScoreProcessor.JudgementWindow[Judgement.Okay])
                    return i;
            }

            return -1;
        }
    }
}
