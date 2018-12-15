using Accord.Math.Optimization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quaver.Tools.Commands
{
    public class OptimizeCommand
    {
        public OptimizeCommand()
        {
            var asd = new NelderMead(3);
        }
    }
}
