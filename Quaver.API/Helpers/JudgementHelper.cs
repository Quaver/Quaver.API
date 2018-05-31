using Quaver.API.Enums;

namespace Quaver.API.Helpers
{
    public class JudgementHelper
    {
        public static string JudgementToShortName(Judgement j)
        {
            switch (j)
            {
                case Judgement.Marvelous:
                    return "MV";
                case Judgement.Perfect:
                    return "PF";
                case Judgement.Great:
                    return "GR";
                case Judgement.Good:
                    return "GD";
                case Judgement.Okay:
                    return "OK";
                case Judgement.Miss:
                    return "MS";
            }

            return "";
        }
    }
}