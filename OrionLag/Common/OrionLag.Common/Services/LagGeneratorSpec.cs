using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Common.Services
{
    public class LagGeneratorSpec
    {
        public int antallSkiver { get; set; }

        public int antallskyttereilaget { get; set; }
        public int antallHold { get; set; }
        public bool avbrekk { get; set; }
        public int StartLagNr { get; set; }

        public int MinutesEachTeam { get; set; }

        public DateTime? StartTime { get; set; }
        public int OrionHoldId { get; set; }
    }
}
