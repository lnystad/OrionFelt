using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Utils
{
    public class Skiver
    {
      
        public Skiver()
        {
        }

        public Skiver(Skiver copy)
        {
            SkiveNummer = copy.SkiveNummer;
            Free = copy.Free;
            if (copy.Skytter != null)
            {
                Skytter = new Skytter(copy.Skytter);
            }
        }

        public Skiver(int skiveteller)
        {
            this.SkiveNummer = skiveteller;
        }

        public int SkiveNummer { get; set; }

        public Skytter Skytter { get; set; }

        public bool Free { get; set; }
    }
}
