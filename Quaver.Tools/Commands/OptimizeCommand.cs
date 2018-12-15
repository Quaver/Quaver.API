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
        ///     Reference Directory for map files.
        /// </summary>
        private string BaseFolder { get; } = "c:/users/denys/desktop/testmaps/dan/full-reform";

        /// <summary>
        ///     Reference Map Files. The optimizer will assume that all files are sorted alphabetically, are in difficulty order, and have a fixed difficulty interval.
        /// </summary>
        private List<string> Files { get; }

        /// <summary>
        ///     Current iteration count
        /// </summary>
        private int N { get; set; } = 0;

        public OptimizeCommand(string[] args) : base(args)
        {
            Constants = new StrainConstantsKeys();
            GeneralConvergence = new GeneralConvergence(Constants.ConstantVariables.Count);
            Files = Directory.GetFiles(BaseFolder, "*.qua", SearchOption.AllDirectories).ToList();
            Files.AddRange(Directory.GetFiles(BaseFolder, "*.osu", SearchOption.AllDirectories));
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
            var diffs = new double[Files.Count];
            for (var i = 0; i < Files.Count; i++)
            {
                Qua map = null;

                if (Files[i].EndsWith(".qua"))
                    map = Qua.Parse(Files[i]);
                else if (Files[i].EndsWith(".osu"))
                    map = new OsuBeatmap(Files[i]).ToQua();

                diffs[i] = map.SolveDifficulty(Constants).OverallDifficulty;
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
