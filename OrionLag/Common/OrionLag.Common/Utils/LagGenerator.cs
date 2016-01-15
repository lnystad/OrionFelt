namespace OrionLag.Common.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using OrionLag.Common.DataModel;
    using OrionLag.Common.Services;

    public class LagGenerator : ILagGenerator
    {
        public List<Lag> GenererLag(List<InputData> data )
        {
            throw new NotImplementedException();
        }

        public List<Lag> GenererLag(List<InputData> data, LagGeneratorSpec spec)
        {
            List<Lag> retVal = new List<Lag>();
            if (data == null)
            {
                return retVal;
            }

            if (data.Count == 0)
            {
                return retVal;
            }
           
            int StartSkytter = 0;
            foreach (var skytterIn in data)
            {
                int hold = 0;
                int LagNr = StartSkytter / spec.antallskyttereilaget + 1 ;
                int forsteSkive = StartSkytter % spec.antallskyttereilaget +1;
                while (hold < spec.antallHold)
                {
                    Lag lag = this.GetLagNr(retVal, LagNr, spec.antallSkiver);
                    int SkiveNr = (hold* spec.antallskyttereilaget)+forsteSkive;
                    var skive = lag.FinnLedigSkive(SkiveNr);
                    skive.Skytter = new Skytter()
                                        {
                                            Klasse = skytterIn.Klasse,
                                            Name = skytterIn.Name,
                                            Skytterlag = skytterIn.Skytterlag,
                                            SkytterNr = skytterIn.SkytterNr
                                        };
                    if (spec.avbrekk)
                    {
                        LagNr = LagNr + 2;
                    }
                    else
                    {
                        LagNr = LagNr + 1;
                    }

                    hold++;
                }

                StartSkytter++;

            }


            return retVal;
        }

        public LagGeneratorSpec GetSpecFromConfiguration()
        {
            LagGeneratorSpec spec = new LagGeneratorSpec()
                                        {
                                            antallHold = 6,
                                            antallSkiver = 12,
                                            antallskyttereilaget = 2,
                                            avbrekk = false,
                                            MinutesEachTeam = 6,
                                            StartLagNr = 1,
                                            StartTime = new DateTime(2016, 6, 25, 8, 0, 0)
                                        };

            return spec;
        }

        private Lag GetLagNr(List<Lag> retVal, int lagNr, int maxSkiveriLaget)
        {
            var foudLag = retVal.FirstOrDefault(x => x.LagNummer == lagNr);
            if (foudLag == null)
            {
                foudLag = new Lag(lagNr, maxSkiveriLaget);
                retVal.Add(foudLag);
            }

            return foudLag;
        }

       
    }
}
