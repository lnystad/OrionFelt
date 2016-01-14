namespace OrionLag.Utils
{
    using System;
    using System.Collections.Generic;

    public class Lag
    {
        private int maxSkiveriLaget;
       
        public Lag()
        {
            SkiverILaget = new List<Skiver>();
        }

        public Lag(Lag copy)
        {
            SkiverILaget = new List<Skiver>();
            LagNummer = copy.LagNummer;
            LagTid = copy.LagTid;
            MaxSkiveNummer = copy.MaxSkiveNummer;

            foreach (var skive in copy.SkiverILaget)
            {
                SkiverILaget.Add(new Skiver(skive));
            }
        }

        public Lag(int lagNummer, int maxSkiveriLaget)
        {
            SkiverILaget  = new List<Skiver>();
            this.LagNummer = lagNummer;
            this.maxSkiveriLaget = maxSkiveriLaget;
            int skiveteller = 1;
            while (skiveteller <= maxSkiveriLaget)
            {
                SkiverILaget.Add(new Skiver(skiveteller));
                skiveteller++;
            }
        }

        public int LagNummer { get; set; }

        public DateTime? LagTid { get; set; }

       
        public string LagNavn => "Lag " + this.LagNummer;

        public int MaxSkiveNummer { get; set; }

        public List<Skiver> SkiverILaget { get; set; }

        public Skiver FinnLedigSkive( int  forsteSkive)
        {
            if (SkiverILaget.Count == 0)
            {
                var skive = new Skiver(forsteSkive);
                SkiverILaget.Add(skive);
                return skive;
            }

            
            foreach (var skive in SkiverILaget)
            {
                if(skive.SkiveNummer >= forsteSkive)
                {
                    if (skive.Skytter == null)
                    {
                        return skive;
                    }
                }
            }

            return null;
        }
    }
}