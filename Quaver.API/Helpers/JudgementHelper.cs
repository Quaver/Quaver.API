/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

using Quaver.API.Enums;

namespace Quaver.API.Helpers
{
    public class JudgementHelper
    {
        public static string JudgementToShortName(Judgement j)
        {
            switch (j)
            {
                case Judgement.Marv:
                    return "MV";
                case Judgement.Perf:
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
