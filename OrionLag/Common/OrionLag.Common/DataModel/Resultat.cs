namespace OrionLag.Common.DataModel
{
    using System;
    using System.Collections.Generic;

    public class Resultat
    {
        public Resultat()
        {
            this.Serier = new List<Serie>();
            this.Id = Guid.NewGuid();
        }
        public Resultat(string skytterNr, Guid skytterId, List<Serie> serier)
        {
            this.Serier = new List<Serie>();
            this.Id = Guid.NewGuid();
            this.SkytterId = skytterId;
            this.SkytterNr = skytterNr;
            if (serier != null)
            {
                foreach (var ser in serier)
                {
                    var newser = new Serie(ser);
                    this.Serier.Add(newser);
                }
            }
        }
        public Guid SkytterId { get; set; }

      
        public string SkytterNr { get; set; }

        public Guid Id { get; set; }
        public Guid StevneId { get; set; }

       

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

        public void Update(SkytterResultat resultat)
        {
            foreach (var ser in resultat.Serier)
            {
                this.Serier.Add(new Serie(ser));
            }
        }
    }
}
