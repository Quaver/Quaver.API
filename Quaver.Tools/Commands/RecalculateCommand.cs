using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quaver.API.Enums;
using Quaver.API.Maps;
using Quaver.API.Maps.Processors.Rating;
using Quaver.API.Maps.Processors.Scoring;
using Quaver.API.Maps.Structures;
using Quaver.Tools.Helpers;
using RestSharp;

namespace Quaver.Tools.Commands
{
    public class RecalculateCommand : Command
    {
        private int UserId { get; }

        private int Mode { get; }

        public RecalculateCommand(string[] args) : base(args)
        {
            UserId = int.Parse(args[1]);
            Mode = int.Parse(args[2]);
        }

        public override void Execute()
        {
            var request = new RestRequest($"https://api.quavergame.com/v1/users/scores/best", Method.GET);
            request.AddQueryParameter("id", $"{UserId}");
            request.AddQueryParameter("mode", $"{Mode}");

            var client = new RestClient("https://api.quavergame.com/");
            var response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"Could not retrieve best scores");

            Recalculate(JObject.Parse(response.Content)["scores"]);
        }

        /// <summary>
        ///     Recalculates all scores
        /// </summary>
        /// <param name="scores"></param>
        private void Recalculate(JToken scores)
        {
            var comparedScores = new List<ComparedScores>();

            foreach (var score in scores)
            {
                // Make a dummy Qua for this score
                var qua = new Qua();

                var totalObjects = (int)score["count_marv"] + (int)score["count_perf"] + (int)score["count_great"] +
                                   (int)score["count_good"] + (int)score["count_okay"] + (int)score["count_miss"];

                for (var i = 0; i < totalObjects; i++)
                    qua.HitObjects.Add(new HitObjectInfo());

                var processor = new ScoreProcessorKeys(qua, 0);

                for (var i = 0; i < (int)score["count_marv"]; i++)
                    processor.CalculateScore(Judgement.Marv);

                for (var i = 0; i < (int)score["count_perf"]; i++)
                    processor.CalculateScore(Judgement.Perf);

                for (var i = 0; i < (int)score["count_great"]; i++)
                    processor.CalculateScore(Judgement.Great);

                for (var i = 0; i < (int)score["count_good"]; i++)
                    processor.CalculateScore(Judgement.Good);

                for (var i = 0; i < (int)score["count_okay"]; i++)
                    processor.CalculateScore(Judgement.Okay);

                for (var i = 0; i < (int)score["count_miss"]; i++)
                    processor.CalculateScore(Judgement.Miss);

                var difficultyRating = (double)score["performance_rating"] / Math.Pow((double)score["accuracy"] / 98, 6);

                comparedScores.Add(new ComparedScores($"{score["map"]["artist"]} - {score["map"]["title"]} [{score["map"]["difficulty_name"]}]",
                    (double)score["accuracy"], processor.Accuracy, (double)score["performance_rating"], difficultyRating));
            }

            Console.WriteLine(comparedScores.ToStringTable(new[]
            {
                "Map Name",
                "Difficulty",
                "Orig. Acc%",
                "New Acc%",
                "Orig. Rating",
                "New Rating",
            },
                u => u.MapName,
                u => u.DifficultyRating,
                u => u.OriginalAccuracy,
                u => u.NewAccuracy,
                u => u.OriginalRating,
                u => new RatingProcessorKeys(u.DifficultyRating).CalculateRating(u.NewAccuracy)));
        }
    }

    public class ComparedScores
    {
        public string MapName { get; }

        public double DifficultyRating { get; }

        public double OriginalAccuracy { get; }

        public float NewAccuracy { get; }

        public double OriginalRating { get; }

        public ComparedScores(string name, double originalAcc, float newAcc, double originalRating, double diff)
        {
            MapName = name;
            OriginalAccuracy = originalAcc;
            NewAccuracy = newAcc;
            OriginalRating = originalRating;
            DifficultyRating = diff;
        }
    }
}