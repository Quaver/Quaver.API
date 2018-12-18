using System;
using System.Collections.Generic;
using System.Text;
using Accord.Math.Convergence;
using Accord.Math.Optimization;
using Quaver.API.Maps.Parsers;
using Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys;

namespace Quaver.Tools.Examples
{
    /// <summary>
    /// 
    /// </summary>
    public class OptimizationExample
    {
        /// <summary>
        ///     Reference Constants
        /// </summary>
        private static StrainConstantsKeys Constants { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private static string SampleDirectory { get; } = "c:/users/denys/desktop/testmaps/dan/full-reform";

        /// <summary>
        /// 
        /// </summary>
        private static int TotalIterations { get; } = 20;

        /// <summary>
        ///     Used to count total iterations
        /// </summary>
        private static int N { get; set; }

        /// <summary>
        ///     Sample Files used for Calc
        /// </summary>
        private static List<string> OptimizationSamples { get; } = new List<string>()
        {
            $"{SampleDirectory}/Various Artists - Dan ~ REFORM ~ 1st Pack (DDMythical) [~ INTRO-1st ~ (Marathon)].qua",
            $"{SampleDirectory}/Various Artists - Dan ~ REFORM ~ 1st Pack (DDMythical) [~ INTRO-2nd ~ (Marathon)].qua",
            $"{SampleDirectory}/Various Artists - Dan ~ REFORM ~ 1st Pack (DDMythical) [~ INTRO-3rd ~ (Marathon)].qua",
            $"{SampleDirectory}/Various Artists - Dan ~ REFORM ~ 1st Pack (DDMythical) [~ 1st ~ (Marathon)].qua",
            $"{SampleDirectory}/Various Artists - Dan ~ REFORM ~ 1st Pack (DDMythical) [~ 2nd ~ (Marathon)].qua",
            $"{SampleDirectory}/Various Artists - Dan ~ REFORM ~ 1st Pack (DDMythical) [~ 3rd ~ (Marathon)].qua",
            $"{SampleDirectory}/Various Artists - Dan ~ REFORM ~ 1st Pack (DDMythical) [~ 4th ~ (Marathon)].qua",
            $"{SampleDirectory}/Various Artists - Dan ~ REFORM ~ 1st Pack (DDMythical) [~ 5th ~ (Marathon)].qua",
            $"{SampleDirectory}/Various Artists - Dan ~ REFORM ~ 2nd Pack (DDMythical) [~ 6th ~ (Marathon)].qua",
            $"{SampleDirectory}/Various Artists - Dan ~ REFORM ~ 2nd Pack (DDMythical) [~ 7th ~ (Marathon)].qua",
            $"{SampleDirectory}/Various Artists - Dan ~ REFORM ~ 2nd Pack (DDMythical) [~ 8th ~ (Marathon)].qua",
            $"{SampleDirectory}/Various Artists - Dan ~ REFORM ~ 2nd Pack (DDMythical) [~ 9th ~ (Marathon)].qua",
            $"{SampleDirectory}/Various Artists - Dan ~ REFORM ~ 2nd Pack (DDMythical) [~ EXTRA-ALPHA ~ (Marathon)].qua",
            $"{SampleDirectory}/Various Artists - Dan ~ REFORM ~ 2nd Pack (DDMythical) [~ EXTRA-BETA ~ (Marathon)].qua",
            $"{SampleDirectory}/Various Artists - Dan ~ REFORM ~ 2nd Pack (DDMythical) [~ EXTRA-cGAMMA ~ (Marathon)].qua",
            $"{SampleDirectory}/Various Artists - Dan ~ REFORM ~ 2nd Pack (DDMythical) [~ EXTRA-DELTA ~ (Marathon)].qua",
            $"{SampleDirectory}/Various Artists - Dan ~ REFORM ~ 2nd Pack (DDMythical) [~ EXTRA-EPSILON ~ (Marathon)].qua"
        };

        /// <summary>
        /// 
        /// </summary>
        public static void Optimize()
        {
            // Initialize Variables
            Constants = new StrainConstantsKeys();
            var target = Constants.ConstantsToArray();
            Func<double[], double> fx = GetDeltaDeviation;
            var solution = new NelderMead(target.Length, fx)
            {
                MaximumValue = -1e6,
                Convergence = new GeneralConvergence(Constants.ConstantVariables.Count)
                {
                    MaximumEvaluations = TotalIterations
                }
            };

            // Optimize
            solution.Minimize(target);

            // Write Results
            Console.WriteLine("----------- RESULTS -----------");
            Console.WriteLine(Constants.GetInfoFromVariables());
        }

        /// <summary>
        ///     Returns Standard Deviation of the delta between each Map.
        /// </summary>
        /// <returns></returns>
        private static double GetDeltaDeviation(double[] input)
        {
            // Update Constants to match input
            Constants.UpdateConstants(input);
            N++;

            // Get Average Interval between each difficulty from Samples.
            double sigma = 0;
            double xbar = 0;
            var sample = new double[OptimizationSamples.Count];
            for (var i = 0; i < OptimizationSamples.Count; i++)
            {
                sample[i] = new OsuBeatmap(OptimizationSamples[i]).ToQua().SolveDifficulty(Constants).OverallDifficulty;
                if (i > 0)
                    xbar += sample[i] - sample[i - 1];
            }
            xbar /= (sample.Length - 1);

            // Calculate Standard Deviation.
            for (var i = 0; i < OptimizationSamples.Count - 1; i++)
                sigma += Math.Pow(sample[i + 1] - sample[i] - xbar, 2);

            sigma = 1 + sigma / OptimizationSamples.Count;
            Console.WriteLine($"n = {N}, f(x) = {sigma}");
            return sigma;
        }
    }
}
