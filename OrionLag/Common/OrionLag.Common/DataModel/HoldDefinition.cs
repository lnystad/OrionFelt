using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Common.DataModel
{
    public class HoldDefinition
    {
        public HoldDefinition(int nr, HoldType type)
        {
            this.HoldNr = nr;
            this.HoldType = type;
        }

        public int HoldNr { get; set; }
        public HoldType HoldType { get; set; }
    }
}
