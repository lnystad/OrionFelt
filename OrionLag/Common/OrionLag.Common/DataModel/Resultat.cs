namespace OrionLag.Common.DataModel
{
    using System;
    using System.Collections.Generic;

    using OrionLag.Common.Diagnosis;

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
                if (serie.Valid)
                {
                    tot += serie.TotalSum();
                }
                
            }

            return tot;
        }
        public int FeltSum(int serieNr)
        {
            int tot = 0;
            foreach (var serie in Serier)
            {
                if (serie.Valid && serie.Nr == serieNr)
                {
                    tot += serie.FeltSum();
                }

            }

            return tot;
        }

        public int FeltInnerSum(int serieNr)
        {
            int tot = 0;
            foreach (var serie in Serier)
            {
                if (serie.Valid && serie.Nr == serieNr)
                {
                    tot += serie.FeltInnerSum();
                }

            }

            return tot;
        }

        public int FeltSum()
        {
            int tot = 0;
            foreach (var serie in Serier)
            {
                if (serie.Valid)
                {
                    tot += serie.FeltSum();
                }

            }

            return tot;
        }

        public int FeltInnerSum()
        {
            int tot = 0;
            foreach (var serie in Serier)
            {
                if (serie.Valid)
                {
                    tot += serie.FeltInnerSum();
                }

            }

            return tot;
        }

        public void Update(SkytterResultat resultat)
        {

            foreach (var ser in resultat.Serier)
            {
                var foundSerie = this.Serier.Find(x => x.Nr == ser.Nr && x.Valid);
                if (foundSerie!=null)
                {
                    Log.Warning("Fant tidligere Serie {0} {1} Skytternr={1}", foundSerie.Nr, foundSerie.ValidTime, SkytterNr);
                    foundSerie.Valid = false;
                }
                ser.Valid = true;
                ser.ValidTime=DateTime.Now;
                this.Serier.Add(new Serie(ser));
            }
        }
    }
}
