using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Quaver.API.Enums;
using Quaver.API.Maps;
using RandomNameGeneratorLibrary;
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
        ///     The list of judgements for this particular bot.
        /// </summary>
        public List<Judgement> Judgements { get; }
        
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
            Judgements = new List<Judgement>();

            // Generate username for this bot.
            Name = GenerateRandomName();

            // Contains the random weights of each judgement 
            IWeightedRandomizer<int> weights;
            
            switch (Level)
            {
                // Garbage bot. Okays every single object.
                case BotLevel.Horrible:
                    Map.HitObjects.ForEach(x =>
                    {
                        Judgements.Add(Judgement.Okay);
                        
                        if (x.IsLongNote)
                            Judgements.Add(Judgement.Okay);
                    });
                    break;
                // Bad Player.
                case BotLevel.Noob:
                    weights = new DynamicWeightedRandomizer<int>();
                    
                    weights.Add((int)Judgement.Marvelous, 30);
                    weights.Add((int)Judgement.Perfect, 15);
                    weights.Add((int)Judgement.Great, 20);
                    weights.Add((int)Judgement.Good, 10);
                    weights.Add((int)Judgement.Okay, 5);
                    weights.Add((int)Judgement.Miss, 20);
                      
                    GenerateRandomJudgements(weights);
                    break;
                // An amateur player, gets the job done.
                case BotLevel.Amateur:
                    weights = new DynamicWeightedRandomizer<int>();
                    
                    weights.Add((int)Judgement.Marvelous, 75);
                    weights.Add((int)Judgement.Perfect, 15);
                    weights.Add((int)Judgement.Great, 5);
                    weights.Add((int)Judgement.Good, 1);
                    weights.Add((int)Judgement.Okay, 1);
                    weights.Add((int)Judgement.Miss, 3);
                      
                    GenerateRandomJudgements(weights);
                    break;
                // High level player.
                case BotLevel.Decent:
                    weights = new DynamicWeightedRandomizer<int>();
                    
                    weights.Add((int)Judgement.Marvelous, 75);
                    weights.Add((int)Judgement.Perfect, 20);
                    weights.Add((int)Judgement.Great, 2);
                    weights.Add((int)Judgement.Good, 1);
                    weights.Add((int)Judgement.Okay, 1);
                    weights.Add((int)Judgement.Miss, 1);
                      
                    GenerateRandomJudgements(weights);
                    break;
                // God. Has marvelous on everything.
                case BotLevel.ATTang:
                    Name = "ATTang";
                    
                    Map.HitObjects.ForEach(x =>
                    {
                        Judgements.Add(Judgement.Marvelous);
                        
                        if (x.IsLongNote)
                            Judgements.Add(Judgement.Marvelous);
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
                Judgements.Add((Judgement) weights.NextWithReplacement());

                // Add another judgement if its a long note.
                if (obj.IsLongNote)
                    Judgements.Add((Judgement) weights.NextWithReplacement());
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