namespace OrionLag.Common.DataModel
{
    using System;
    using System.Collections.Generic;

    public class SkytterResultat
    {
        public SkytterResultat()
        {
            Serier = new List<Serie>();
            Id = Guid.NewGuid();
        }
        public Guid SkytterId { get; set; }

        public Skytter Skytter { get; set; }

        public string SkytterNr { get; set; }

        public Guid Id { get; set; }
        
        public int OrionHoldId { get; set; }

        public int OrionTotalSum { get; set; }

        public int LagNummer { get; set; }

        public int SkiveNr { get; set; }

        public List<Serie> Serier { get; set; }

        public int TotalSum()
        {
            int tot = 0;
            foreach (var serie in Serier)
            {
                tot += serie.TotalSum();
            }

            return tot;
        }

        public int TotalFeltSum()
        {
            int tot = 0;
            foreach (var serie in Serier)
            {
                tot += serie.FeltSum();
            }

            return tot;
        }
    }
}
