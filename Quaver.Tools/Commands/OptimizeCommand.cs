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
            //var iterations = 100;
            var target = new double[] { 1, 1 };
            Func<double[], double> fx = OptimizeVariables;
            var solution = new NelderMead(target.Length, fx);

            for (var i = 0; i < target.Length; i++)
            {
                solution.LowerBounds[i] = target[i];
                solution.UpperBounds[i] = target[i];
            }

            solution.MaximumValue = -1e6;
            var success = solution.Minimize(target);
        }

        /// <summary>
        ///     Calculate specific map files for optimization.
        ///     TODO: reference files from somewhere in the solution?
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private double OptimizeVariables(double[] input)
        {
            return 1;
        }
    }
}
