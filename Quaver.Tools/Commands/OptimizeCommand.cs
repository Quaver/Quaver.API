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
        private static string BaseFolder { get; } = "c:/users/admin/desktop/testmaps/dan/full-reform"; //full-reform

        private static string JackDirectory { get; } = "C:/Users/admin/Desktop/testmaps/dan/jack";

        private static string SpeedDirectory { get; } = "C:/Users/admin/Desktop/testmaps/dan/speed";

        private static string TechDirectory { get; } = "C:/Users/admin/Desktop/testmaps/dan/tech";

        private static string StaminaDirectory { get; } = "C:/Users/admin/Desktop/testmaps/dan/stamina";

        private List<List<string>> DanCalcTest { get; } = new List<List<string>>()
        {
            new List<string>()
            {
                $"{JackDirectory}/1Various Artists - Dan ~ REFORM ~ JackMap Pack (DDMythical) [Miracle Chance ~ 1st ~ (Marathon)].osu",
                $"{SpeedDirectory}/1Various Artists - Dan ~ REFORM ~ SpeedMap Pack (DDMythical) [Shelter ~ 1st ~ (Marathon)].osu",
                $"{StaminaDirectory}/1Various Artists - Dan ~ REFORM ~ StaminaMap Pack (DDMythical) [Tommorow Perfume ~ 1st ~ (Marathon)].osu",
                $"{TechDirectory}/1Various Artists - Dan ~ REFORM ~ TechMap Pack (DDMythical) [Want U 2 ~ 1st ~ (Marathon)].osu",
            },
            new List<string>()
            {
                $"{JackDirectory}/2Various Artists - Dan ~ REFORM ~ JackMap Pack (DDMythical) [Flashes ~ 2nd ~ (Marathon)].osu",
                $"{SpeedDirectory}/2Various Artists - Dan ~ REFORM ~ SpeedMap Pack (DDMythical) [Snow Storm ~ 2nd ~ (Marathon)].osu",
                $"{StaminaDirectory}/2Various Artists - Dan ~ REFORM ~ StaminaMap Pack (DDMythical) [Eternal Drain ~ 2nd ~ (Marathon)].osu",
                $"{TechDirectory}/2Various Artists - Dan ~ REFORM ~ TechMap Pack (DDMythical) [Vermillion ~ 2nd ~ (Marathon)].osu",
            },
            new List<string>()
            {
                $"{JackDirectory}/3Various Artists - Dan ~ REFORM ~ JackMap Pack (DDMythical) [CrossOver ~ 3rd ~ (Marathon)].osu",
                $"{SpeedDirectory}/3Various Artists - Dan ~ REFORM ~ SpeedMap Pack (DDMythical) [Entelecheia ~ 3rd ~ (Marathon)].osu",
                $"{StaminaDirectory}/3Various Artists - Dan ~ REFORM ~ StaminaMap Pack (DDMythical) [True Blue ~ 3rd ~ (Marathon)].osu",
                $"{TechDirectory}/3Various Artists - Dan ~ REFORM ~ TechMap Pack (DDMythical) [Kirlian Shores ~ 3rd ~ (Marathon)].osu",
            },
            new List<string>()
            {
                $"{JackDirectory}/4Various Artists - Dan ~ REFORM ~ JackMap Pack (DDMythical) [It's Time To Party! ~ 4th ~ (Marathon)].osu",
                $"{SpeedDirectory}/4Various Artists - Dan ~ REFORM ~ SpeedMap Pack (DDMythical) [Shannon's Theorem ~ 4th ~ (Marathon)].osu",
                $"{StaminaDirectory}/4Various Artists - Dan ~ REFORM ~ StaminaMap Pack (DDMythical) [Angel Of Darkness ~ 4th ~ (Marathon)].osu",
                $"{TechDirectory}/4Various Artists - Dan ~ REFORM ~ TechMap Pack (DDMythical) [Ephemera ~ 4th ~ (Marathon)].osu",
            },
            new List<string>()
            {
                $"{JackDirectory}/5Various Artists - Dan ~ REFORM ~ JackMap Pack (DDMythical) [Energy Flower 3007 ~ 5th ~ (Marathon)].osu",
                $"{SpeedDirectory}/5Various Artists - Dan ~ REFORM ~ SpeedMap Pack (DDMythical) [Palette GAMMA ~ 5th ~ (Marathon)].osu",
                $"{StaminaDirectory}/5Various Artists - Dan ~ REFORM ~ StaminaMap Pack (DDMythical) [Elektric U-Phoria ~ 5th ~ (Marathon)].osu",
                $"{TechDirectory}/5Various Artists - Dan ~ REFORM ~ TechMap Pack (DDMythical) [Brainfog ~ 5th ~ (Marathon)].osu",
            },
            new List<string>()
            {
                $"{JackDirectory}/6Various Artists - Dan ~ REFORM ~ JackMap Pack (DDMythical) [Sweet Cherry X ~ 6th ~ (Marathon)].osu",
                $"{SpeedDirectory}/6Various Artists - Dan ~ REFORM ~ SpeedMap Pack (DDMythical) [SOS ~ 6th ~ (Marathon)].osu",
                $"{StaminaDirectory}/6Various Artists - Dan ~ REFORM ~ StaminaMap Pack (DDMythical) [M-A ~ 6th ~ (Marathon)].osu",
                $"{TechDirectory}/6Various Artists - Dan ~ REFORM ~ TechMap Pack (DDMythical) [The Bird's Midair Heatstroke ~ 6th ~ (Marathon)].osu",
            },
            new List<string>()
            {
                $"{JackDirectory}/7Various Artists - Dan ~ REFORM ~ JackMap Pack (DDMythical) [Log-IN ~ 7th ~ (Marathon)].osu",
                $"{SpeedDirectory}/7Various Artists - Dan ~ REFORM ~ SpeedMap Pack (DDMythical) [Hospital ~ 7th ~ (Marathon)].osu",
                $"{StaminaDirectory}/7Various Artists - Dan ~ REFORM ~ StaminaMap Pack (DDMythical) [Hymn ~ 7th ~ (Marathon)].osu",
                $"{TechDirectory}/7Various Artists - Dan ~ REFORM ~ TechMap Pack (DDMythical) [WAVE ~ 7th ~ (Marathon)].osu",
            },
            new List<string>()
            {
                $"{JackDirectory}/8Various Artists - Dan ~ REFORM ~ JackMap Pack (DDMythical) [To The Limit 1.1x ~ 8th ~ (Marathon)].osu",
                $"{SpeedDirectory}/8Various Artists - Dan ~ REFORM ~ SpeedMap Pack (DDMythical) [Omen ~ 8th ~ (Marathon)].osu",
                $"{StaminaDirectory}/8Various Artists - Dan ~ REFORM ~ StaminaMap Pack (DDMythical) [Anguish ~ 8th ~ (Marathon)].osu",
                $"{TechDirectory}/8Various Artists - Dan ~ REFORM ~ TechMap Pack (DDMythical) [RATO ~ 8th ~ (Marathon)].osu",
            },
            new List<string>()
            {
                $"{JackDirectory}/9Various Artists - Dan ~ REFORM ~ JackMap Pack (DDMythical) [ametsuchi 1.25x ~ 9th ~ (Marathon)].osu",
                $"{SpeedDirectory}/9Various Artists - Dan ~ REFORM ~ SpeedMap Pack (DDMythical) [Punishment ~ 9th ~ (Marathon)].osu",
                $"{StaminaDirectory}/9Various Artists - Dan ~ REFORM ~ StaminaMap Pack (DDMythical) [Firmament Castle Velier ~ 9th ~ (Marathon)].osu",
                $"{TechDirectory}/9Various Artists - Dan ~ REFORM ~ TechMap Pack (DDMythical) [Cicadidae ~ 9th ~ (Marathon)].osu",
            },
            new List<string>()
            {
                $"{JackDirectory}/9zYST - The Lost Dedicated (Zyph) [4K].osu",
                $"{StaminaDirectory}/9zVarious Artists - Dan ~ REFORM ~ StaminaMap Pack (DDMythical) [Chandelier ~ 10th ~ (Marathon)].osu",
                $"{TechDirectory}/9zVarious Artists - Dan ~ REFORM ~ TechMap Pack (DDMythical) [Rave 7.7 ~ 10th ~ (Marathon)].osu",
            },
            new List<string>()
            {
                $"{JackDirectory}/aVarious Artists - Dan ~ REFORM ~ JackMap Pack (DDMythical) [Toraburu Kuroneko 1.15x ~ Alpha ~ (Marathon)].osu",
                $"{SpeedDirectory}/aVarious Artists - Dan ~ REFORM ~ SpeedMap Pack (DDMythical) [Makiba ~ Alpha ~ (Marathon)].osu",
                $"{StaminaDirectory}/aVarious Artists - Dan ~ REFORM ~ StaminaMap Pack (DDMythical) [LazorBeamz ~ Alpha ~ (Marathon)].osu",
                $"{TechDirectory}/aVarious Artists - Dan ~ REFORM ~ TechMap Pack (DDMythical) [Odoru Mizushibuki ~ Alpha ~ (Marathon)].osu",
            },
            new List<string>()
            {
                $"{JackDirectory}/bVarious Artists - Dan ~ REFORM ~ JackMap Pack (DDMythical) [Paradigm Shift ~ Beta ~ (Marathon)].osu",
                $"{SpeedDirectory}/bVarious Artists - Dan ~ REFORM ~ SpeedMap Pack (DDMythical) [Edison 1.2x ~ Beta ~ (Marathon)].osu",
                $"{StaminaDirectory}/bVarious Artists - Dan ~ REFORM ~ StaminaMap Pack (DDMythical) [Time to Say Goodbye ~ Beta ~ (Marathon)].osu",
                $"{TechDirectory}/bVarious Artists - Dan ~ REFORM ~ TechMap Pack (DDMythical) [Blue Planet ~ Beta ~ (Marathon)].osu",
            },
            new List<string>()
            {
                $"{JackDirectory}/cVarious Artists - Dan ~ REFORM ~ JackMap Pack (DDMythical) [Star of Andromeda 1.1x ~ Gamma ~ (Marathon)].osu",
                $"{SpeedDirectory}/cVarious Artists - Dan ~ REFORM ~ SpeedMap Pack (DDMythical) [Reflec Streamz ~ Gamma ~ (Marathon)].osu",
                $"{StaminaDirectory}/cVarious Artists - Dan ~ REFORM ~ StaminaMap Pack (DDMythical) [We Luv Lama ~ Gamma ~ (Marathon)].osu",
                $"{TechDirectory}/cVarious Artists - Dan ~ REFORM ~ TechMap Pack (DDMythical) [Fastest Crash ~ Gamma ~ (Marathon)].osu",
            },
            new List<string>()
            {
                $"{JackDirectory}/dVarious Artists - Dan ~ REFORM ~ JackMap Pack (DDMythical) [Chaser 1.3x ~ Delta ~ (Marathon)].osu",
                $"{SpeedDirectory}/dVarious Artists - Dan ~ REFORM ~ SpeedMap Pack (DDMythical) [Volcanic ~ Delta ~ (Marathon)].osu",
                $"{StaminaDirectory}/dVarious Artists - Dan ~ REFORM ~ StaminaMap Pack (DDMythical) [Future Dominators ~ Delta ~ (Marathon)].osu",
                $"{TechDirectory}/dVarious Artists - Dan ~ REFORM ~ TechMap Pack (DDMythical) [Crescent Moon Boss Battle ~ Delta ~ (Marathon)].osu",
            },
            new List<string>()
            {
                $"{JackDirectory}/eVarious Artists - Dan ~ REFORM ~ JackMap Pack (DDMythical) [Rose Quartz 1.3x ~ Epsilon ~ (Marathon)].osu",
                $"{SpeedDirectory}/eVarious Artists - Dan ~ REFORM ~ SpeedMap Pack (DDMythical) [Mario Paint ~ Epsilon ~ (Marathon)].osu",
                $"{StaminaDirectory}/eVarious Artists - Dan ~ REFORM ~ StaminaMap Pack (DDMythical) [Hitsugi to Futago ~ Epsilon ~ (Marathon)].osu",
                $"{TechDirectory}/eVarious Artists - Dan ~ REFORM ~ TechMap Pack (DDMythical) [Forgotten ~ Epsilon ~ (Marathon)].osu",
            }
        };

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
            //Files = Directory.GetFiles(BaseFolder, "*.qua", SearchOption.AllDirectories).ToList();
            //Files.AddRange(Directory.GetFiles(BaseFolder, "*.osu", SearchOption.AllDirectories));
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
            double delta = 0;
            double sigma = 0;

            // Compute for overall difficulties
            var diffs = new double[DanCalcTest.Count];
            for (var i = 0; i < DanCalcTest.Count; i++)
            {
                // Get Difficulty Average for current Dan
                var alpha = new double[DanCalcTest[i].Count];
                for (var j = 0; j < DanCalcTest[i].Count; j++)
                {
                    alpha[j] = new OsuBeatmap(DanCalcTest[i][j]).ToQua().SolveDifficulty(Constants).OverallDifficulty;
                    diffs[i] += alpha[j];
                }
                diffs[i] /= DanCalcTest[i].Count;

                // Compute for Sigma
                double diff = 0;
                for (var j = 0; j < DanCalcTest[i].Count; j++)
                {
                    diff += Math.Pow(alpha[j] - diffs[i], 2);
                }
                diff /= alpha.Length;
                sigma += diff;
            }
            sigma /= DanCalcTest.Count;

            // Compute for mean
            for (var i = 0; i < diffs.Length - 1; i++)
            {
                xbar += diffs[i + 1] - diffs[i];
            }
            xbar /= ( diffs.Length - 2 );

            // Compute for deviation
            for (var i = 0; i < diffs.Length - 1; i++)
            {
                delta += Math.Pow((diffs[i + 1] - diffs[i]) - xbar, 2);
            }
            delta /= ( diffs.Length - 2 );


            // Log and terminate optimization if necessary
            var fx = delta * sigma;
            Console.WriteLine($"n = {N}, f(x) = {fx}");
            if (N >= Limit) GeneralConvergence.Cancel = true;
            return fx;
        }
    }
}
