using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Quaver.API.Enums;
using Quaver.API.Maps;
using Quaver.API.Maps.Processors.Scoring;
using Quaver.API.Maps.Processors.Scoring.Data;

namespace Quaver.API.Gameplay
{
    /// <summary>
    ///     Artificial player with a pre-generated score. Used during gameplay.
    /// </summary>
    public class Bot
    {
        /// <summary>
        ///     The map being played.
        /// </summary>
        private Qua Map { get; }

        /// <summary>
        ///     The bot's randomly generated name.
        /// </summary>
        private string _name;
        public string Name
        {
            get => _name;
            set => _name = $"Bot {value}";
        }

        /// <summary>
        ///     The level of the bot (how good it is)
        /// </summary>
        public BotLevel Level { get; }

        /// <summary>
        ///     The list of HitStats for this bot.
        /// </summary>
        public List<HitStat> HitStats { get; }

        /// <summary>
        ///     RNG
        /// </summary>
        private static Random Rng { get; } = new Random();

        /// <summary>
        ///     Ctor - 
        /// </summary>
        /// <param name="map"></param>
        /// <param name="level"></param>
        public Bot(Qua map, BotLevel level)
        {
            Map = map;
            Level = level;
            HitStats = new List<HitStat>();

            // Generate username for this bot.
            Name = GenerateRandomName();

            // Create a fake score processor so we can access the judgement values.
            var scoreProcessor = new ScoreProcessorKeysNEW(new Qua(), 0);

            switch (Level)
            {
                // Garbage bot. Okays every single object.
                case BotLevel.Horrible:
                    Map.HitObjects.ForEach(x =>
                    {
                        HitStats.Add(new HitStat(HitStatType.Hit, KeyPressType.Press, (int)scoreProcessor.JudgementWindow[Judgement.Okay]));

                        if (x.IsLongNote)
                            HitStats.Add(new HitStat(HitStatType.Hit, KeyPressType.Press, (int)scoreProcessor.JudgementWindow[Judgement.Okay]));
                    });
                    break;
                // Bad Player.
                case BotLevel.Noob:
                    Map.HitObjects.ForEach(x =>
                    {
                        HitStats.Add(new HitStat(HitStatType.Hit, KeyPressType.Press, (int)scoreProcessor.JudgementWindow[Judgement.Good]));

                        if (x.IsLongNote)
                            HitStats.Add(new HitStat(HitStatType.Hit, KeyPressType.Press, (int)scoreProcessor.JudgementWindow[Judgement.Good]));
                    });
                    break;
                // An amateur player, gets the job done.
                case BotLevel.Amateur:
                    Map.HitObjects.ForEach(x =>
                    {
                        HitStats.Add(new HitStat(HitStatType.Hit, KeyPressType.Press, (int)scoreProcessor.JudgementWindow[Judgement.Great]));

                        if (x.IsLongNote)
                            HitStats.Add(new HitStat(HitStatType.Hit, KeyPressType.Press, (int)scoreProcessor.JudgementWindow[Judgement.Great]));
                    });
                    break;
                // High level player.
                case BotLevel.Decent:
                    Map.HitObjects.ForEach(x =>
                    {
                        HitStats.Add(new HitStat(HitStatType.Hit, KeyPressType.Press, (int)scoreProcessor.JudgementWindow[Judgement.Perf]));

                        if (x.IsLongNote)
                            HitStats.Add(new HitStat(HitStatType.Hit, KeyPressType.Press, (int)scoreProcessor.JudgementWindow[Judgement.Perf]));
                    });
                    break;
                // God. Has marvelous on everything.
                case BotLevel.ATTang:
                    Name = "ATTang";

                    Map.HitObjects.ForEach(x =>
                    {
                        HitStats.Add(new HitStat(HitStatType.Hit, KeyPressType.Press, (int)scoreProcessor.JudgementWindow[Judgement.Marv]));

                        if (x.IsLongNote)
                            HitStats.Add(new HitStat(HitStatType.Hit, KeyPressType.Press, (int)scoreProcessor.JudgementWindow[Judgement.Marv]));
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Generates a random file name from the list of bot names.
        /// </summary>
        /// <returns></returns>
        public static string GenerateRandomName()
        {
            var names = ResourceStore.names.Split('\n');
            return names[Rng.Next(1, names.Length - 1)];
        }
    }
}