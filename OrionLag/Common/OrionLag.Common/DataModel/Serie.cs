namespace OrionLag.Common.DataModel
{
    using System;
    using System.Collections.Generic;

    public class Serie
    {
        public Serie()
        {
            this.Verdier = new List<Skuddverdi>();
            Valid = true;
            ValidTime = DateTime.Now;
        }
        public Serie(Serie cpy)
        {
            this.Nr = cpy.Nr;
            this.Navn = cpy.Navn;
            this.AntallTellende = cpy.AntallTellende;
            this.Verdier = new List<Skuddverdi>();
            this.Valid = cpy.Valid;
            this.ValidTime = cpy.ValidTime;
            foreach (var foundSerir in cpy.Verdier)
            {
                this.Verdier.Add(foundSerir);
            }
        }
        public string Navn { get; set; }
        public int Nr { get; set; }

        public int AntallTellende { get; set; }

        public bool Valid { get; set; }

        public DateTime ValidTime { get; set; }

        public List<Skuddverdi> Verdier { get; set; }

        public int FeltSum()
        {
            int tot = 0;
            foreach (var verdi in Verdier)
            {
                if (verdi.GetType() == typeof(FeltVerdi))
                {
                    tot += verdi.Sum();
                }
            }
            return tot;
        }

        public int FeltInnerSum()
        {
            int tot = 0;
            foreach (var verdi in Verdier)
            {
                if (verdi.GetType() == typeof(FeltVerdi))
                {
                    tot += verdi.InnerSum();
                }
            }
            return tot;
        }

        

        public int TotalSum()
        {
            int tot = 0;
            foreach (var verdi in Verdier)
            {
                if (verdi.GetType() == typeof(BaneVerdi))
                {
                    tot += verdi.Sum();
                }
            }
            return tot;
        }
    }
}
