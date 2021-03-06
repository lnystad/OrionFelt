﻿namespace OrionLag.Common.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract]
    public class Lag
    {
        private int maxSkiveriLaget;
       
        public Lag()
        {
            this.SkiverILaget = new List<Skiver>();
        }

        public Lag(Lag copy)
        {
            this.SkiverILaget = new List<Skiver>();
            this.LagNummer = copy.LagNummer;
            this.LagTid = copy.LagTid;
            this.MaxSkiveNummer = copy.MaxSkiveNummer;
            this.OrionHoldId = copy.OrionHoldId;
            foreach (var skive in copy.SkiverILaget)
            {
                this.SkiverILaget.Add(new Skiver(skive));
            }
        }

        public Lag(int lagNummer,int orionHoldId,int maxSkiveriLaget)
        {
            this.SkiverILaget  = new List<Skiver>();
            this.LagNummer = lagNummer;
            this.maxSkiveriLaget = maxSkiveriLaget;
            this.OrionHoldId = orionHoldId;
            //int skiveteller = 1;
                this.SkiverILaget = new List<Skiver>();
            //while (skiveteller <= maxSkiveriLaget)
            //{
            //    this.SkiverILaget.Add(new Skiver(skiveteller));
            //    skiveteller++;
            //}
        }
        [DataMember]
        public int OrionHoldId { get; set; }

        [DataMember]
        public int LagNummer { get; set; }
        [DataMember]
        public DateTime? LagTid { get; set; }

        [DataMember]
        public string LagNavn => "Lag " + this.LagNummer;
        [DataMember]
        public int MaxSkiveNummer { get; set; }

        [DataMember]
        public List<Skiver> SkiverILaget { get; set; }

        public Skiver FinnLedigSkive( int  forsteSkive)
        {
            if (this.SkiverILaget.Count == 0)
            {
                var skive = new Skiver(forsteSkive);
                this.SkiverILaget.Add(skive);
                return skive;
            }

            
            foreach (var skive in this.SkiverILaget)
            {
                if(skive.SkiveNummer >= forsteSkive)
                {
                    if (skive.Skytter == null)
                    {
                        return skive;
                    }
                }
            }

            if (forsteSkive <= this.maxSkiveriLaget)
            {
                var skive = new Skiver(forsteSkive);
                this.SkiverILaget.Add(skive);
                return skive;
            }

            return null;
        }

        public Skiver GetSkiveNr(int skiveNr)
        {
            foreach (var skive in this.SkiverILaget)
            {
                if (skive.SkiveNummer == skiveNr)
                {
                        return skive;
                }
            }
            Skiver sk= new Skiver(skiveNr);

            this.SkiverILaget.Add(sk);
            this.SkiverILaget = this.SkiverILaget.OrderBy(x => x.SkiveNummer).ToList();
            
            return sk;
        }
    }
}