using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Input.ViewModel
{
    using OrionLag.Common.DataModel;

    public class SkiverViewModel
    {
        private Skiver m_skive ;
        

        public SkiverViewModel(int lagNummer, Skiver skive)
        {
            m_skive = skive;
            this.LagNummer = lagNummer;
            this.SkiveNummer = skive.SkiveNummer;
            if (skive.Skytter != null)
            {
                SkytterNr = skive.Skytter.SkytterNr;
                Name = skive.Skytter.Name;
                Skytterlag = skive.Skytter.Skytterlag;
                Klasse = skive.Skytter.Klasse;
            }
            
        }

        public int LagNummer { get; set; }
        public int SkiveNummer { get; set; }

        public string SkytterNr { get; set; }
        public string Name { get; set; }

        public string Skytterlag { get; set; }

        public string Klasse { get; set; }
    }
}
