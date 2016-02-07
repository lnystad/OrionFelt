using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Common.DataModel
{
    using System.Runtime.Serialization;

    public class LagDefinition
    {
        public int OrionHoldId { get; set; }

        public int LagNummer { get; set; }
        public DateTime? LagTid { get; set; }

        public string LagNavn => "Lag " + this.LagNummer;
    }
}
