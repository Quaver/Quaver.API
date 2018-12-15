using Accord.Math.Optimization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Accord.Math.Convergence;
using Accord.Statistics;
using Quaver.API.Enums;
using Quaver.API.Maps;
using Quaver.API.Maps.Parsers;
using Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys;

namespace Quaver.Tools.Commands
{
    internal class OptimizeCommand : Command
    {
        /// <summary>
        ///     Reference Constants
        /// </summary>
        private StrainConstantsKeys Constants { get; }

        /// <summary>
        ///     Used to terminate the Optimization Loop
        /// </summary>
        private GeneralConvergence GeneralConvergence { get; set; }

        /// <summary>
        ///     Optimization will stop after this amount of iteration
        /// </summary>
        private int Limit { get; } = 500;

        /// <summary>
        ///     Current iteration count
        /// </summary>
        private int N { get; set; } = 0;

        public OptimizeCommand(string[] args) : base(args)
        {
            Constants = new StrainConstantsKeys();
            GeneralConvergence = new GeneralConvergence(Constants.ConstantVariables.Count);
        }

        public override void Execute()
        {
            // Initialize Variables
            var target = Constants.ConstantsToArray();
            Func<double[], double> fx = OptimizeVariables;
            var solution = new NelderMead(target.Length, fx)
            {
                MaximumValue = -1e6,
                Convergence = GeneralConvergence
            };

            // Optimize
            solution.Minimize(target);

            // Write Results
            Console.WriteLine("----------- RESULTS -----------");
            Console.WriteLine(Constants.GetInfoFromVariables());
        }

        /// <summary>
        ///     Calculate specific map files for optimization.
        ///     TODO: reference files from somewhere in the solution?
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private double OptimizeVariables(double[] input)
        {
            N++;
            Constants.UpdateConstants(input);
            double xbar = 0;
            double sigma = 0;

            // Compute for overall difficulties
            var user = "denys";
            var baseFolder = $"c:/users/{user}/desktop/testmaps/dan/full-reform";
            var files = Directory.GetFiles(baseFolder, "*.qua", SearchOption.AllDirectories).ToList();
            files.AddRange(Directory.GetFiles(baseFolder, "*.osu", SearchOption.AllDirectories));
            var diffs = new double[files.Count];
            for (var i = 0; i < files.Count; i++)
            {
                var file = files[i];
                Qua map = null;

                if (file.EndsWith(".qua"))
                    map = Qua.Parse(file);
                else if (file.EndsWith(".osu"))
                    map = new OsuBeatmap(file).ToQua();

                var solver = map.SolveDifficulty(Constants);
                var diff = solver.OverallDifficulty;
                diffs[i] = diff;
            }

            // Compute for mean
            for (var i = 0; i < diffs.Length - 1; i++)
            {
                xbar += diffs[i + 1] - diffs[i];
            }
            xbar /= ( diffs.Length - 2 );

            // Compute for deviation
            for (var i = 0; i < diffs.Length - 1; i++)
            {
                sigma += Math.Pow((diffs[i + 1] - diffs[i]) - xbar, 2);
            }
            sigma /= ( diffs.Length - 2 );


            // Log and terminate optimization if necessary
            Console.WriteLine($"n = {N}, f(x) = {sigma}");
            if (N >= Limit) GeneralConvergence.Cancel = true;
            return sigma;
        }
    }
}
