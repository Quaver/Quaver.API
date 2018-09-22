using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Quaver.API.Enums;
using Quaver.API.Maps;
using Quaver.API.Maps.Processors.Scoring;
using Quaver.API.Maps.Processors.Scoring.Data;
using Weighted_Randomizer;

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

            // Contains the random weights of each judgement 
            IWeightedRandomizer<int> weights;

            // Create a fake score processor so we can access the judgement values.
            var scoreProcessor = new ScoreProcessorKeys(new Qua(), 0);

            switch (Level)
            {
                // Garbage bot. Okays every single object.
                case BotLevel.Horrible:
                    Map.HitObjects.ForEach(x =>
                    {
                        HitStats.Add(new HitStat(HitStatType.Hit, KeyPressType.Press, (int) scoreProcessor.JudgementWindow[Judgement.Okay]));
                        
                        if (x.IsLongNote)
                            HitStats.Add(new HitStat(HitStatType.Hit, KeyPressType.Press, (int)scoreProcessor.JudgementWindow[Judgement.Okay]));
                    });
                    break;
                // Bad Player.
                case BotLevel.Noob:
                    weights = new DynamicWeightedRandomizer<int>
                    {
                        {(int)scoreProcessor.JudgementWindow[Judgement.Marv], 30},
                        {(int)scoreProcessor.JudgementWindow[Judgement.Perf], 15},
                        {(int)scoreProcessor.JudgementWindow[Judgement.Great], 20},
                        {(int)scoreProcessor.JudgementWindow[Judgement.Good], 10},
                        {(int)scoreProcessor.JudgementWindow[Judgement.Okay], 5},
                        {(int)scoreProcessor.JudgementWindow[Judgement.Miss] + 1, 20}
                    };

                    GenerateRandomJudgements(weights);
                    break;
                // An amateur player, gets the job done.
                case BotLevel.Amateur:
                    weights = new DynamicWeightedRandomizer<int>
                    {
                        {(int)scoreProcessor.JudgementWindow[Judgement.Marv], 75},
                        {(int)scoreProcessor.JudgementWindow[Judgement.Perf], 15},
                        {(int)scoreProcessor.JudgementWindow[Judgement.Great], 5},
                        {(int)scoreProcessor.JudgementWindow[Judgement.Good], 1},
                        {(int)scoreProcessor.JudgementWindow[Judgement.Okay], 1},
                        {(int)scoreProcessor.JudgementWindow[Judgement.Miss] + 1, 3}
                    };

                    GenerateRandomJudgements(weights);
                    break;
                // High level player.
                case BotLevel.Decent:
                    weights = new DynamicWeightedRandomizer<int>
                    {
                        {(int)scoreProcessor.JudgementWindow[Judgement.Marv], 75},
                        {(int)scoreProcessor.JudgementWindow[Judgement.Perf], 20},
                        {(int)scoreProcessor.JudgementWindow[Judgement.Great], 2},
                        {(int)scoreProcessor.JudgementWindow[Judgement.Good], 1},
                        {(int)scoreProcessor.JudgementWindow[Judgement.Okay], 1},
                        {(int)scoreProcessor.JudgementWindow[Judgement.Miss] + 1, 1}
                    };

                    GenerateRandomJudgements(weights);
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
        ///     Generates and fills random weighted judgements
        /// </summary>
        /// <param name="weights"></param>
        private void GenerateRandomJudgements(IWeightedRandomizer<int> weights)
        {
            foreach (var obj in Map.HitObjects)
            {       
                HitStats.Add(new HitStat(HitStatType.Hit, KeyPressType.Press, weights.NextWithReplacement()));

                // Add another judgement if its a long note.
                if (obj.IsLongNote)
                    HitStats.Add(new HitStat(HitStatType.Hit, KeyPressType.Press, weights.NextWithReplacement()));
            }
        }

        /// <summary>
        ///     Generates a random file name from the list of bot names.
        /// </summary>
        /// <returns></returns>
        public static string GenerateRandomName()
        {
            var names = APIResources.names.Split('\n');
            return names[Rng.Next(1, names.Length - 1)];
        }
    }    
}