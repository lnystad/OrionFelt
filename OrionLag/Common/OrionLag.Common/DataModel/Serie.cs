namespace OrionLag.Common.DataModel
{
    using System.Collections.Generic;

    public class Serie
    {
        public Serie()
        {
            this.Verdier = new List<Skuddverdi>();
        }
        public Serie(Serie cpy)
        {
            this.Nr = cpy.Nr;
            this.Navn = cpy.Navn;
            this.AntallTellende = cpy.AntallTellende;
            this.Verdier = new List<Skuddverdi>();
            foreach(var foundSerir in cpy.Verdier)
            {
                this.Verdier.Add(foundSerir);
            }
        }
        public string Navn { get; set; }
        public int Nr { get; set; }

        public int AntallTellende { get; set; }

        public List<Skuddverdi> Verdier { get; set; }

        public int TotalSum()
        {
            int tot = 0;
            foreach (var verdi in Verdier)
            {
                tot += verdi.Sum();
            }
            return tot;
        }
    }
}
