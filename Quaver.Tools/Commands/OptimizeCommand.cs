using Accord.Math.Optimization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quaver.Tools.Commands
{
    internal class OptimizeCommand : Command
    {
        public OptimizeCommand(string[] args) : base(args)
        {
            //initialize stuff
        }

        public override void Execute()
        {
            var limit = 100;
            var n = 0;
            var target = new double[] { 0.2222f, 0.000011f, 0.00000002f, 0.111f, 0.5555555f };
            Func<double[], double> fx = OptimizeVariables;
            var solution = new NelderMead(target.Length, fx)
            {
                MaximumValue = -1e6
            };
            while (n < limit)
            {
                for (var i = 0; i < target.Length; i++)
                {
                    solution.LowerBounds[i] = target[i] + 2;
                    solution.UpperBounds[i] = target[i] - 2;
                }
                var success = solution.Minimize(target);

                Console.WriteLine(solution.Value);
                n++;
            }

            Console.WriteLine("----------- RESULTS -----------");
            foreach (var num in target)
            {
                Console.WriteLine($"num: {num}");
            }
        }

        /// <summary>
        ///     Calculate specific map files for optimization.
        ///     TODO: reference files from somewhere in the solution?
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private double OptimizeVariables(double[] input)
        {
            double average = 0;
            foreach (var num in input)
            {
                average += num;
            }

            return 1 / (Math.Pow(average / (input.Length + 1), 2) + 1) + Math.Pow(input[2],2);
        }
    }
}
