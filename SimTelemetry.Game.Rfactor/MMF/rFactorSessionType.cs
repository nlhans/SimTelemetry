using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimTelemetry.Game.Rfactor.MMF
{
    public enum rFactorSessionType
    {
        TESTDAY,
        PRACTICE,
        QUALIFY,
        WARMUP,
        RACE

    }


    // Use structlayout + fieldoffset because C# is retarDed and can't handle fixed size fields.
}
