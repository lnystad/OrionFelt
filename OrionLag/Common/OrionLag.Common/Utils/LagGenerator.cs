namespace OrionLag.Common.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    using OrionLag.Common.DataModel;
    using OrionLag.Common.Diagnosis;
    using OrionLag.Common.Services;

    public class LagGenerator : ILagGenerator
    {
        public List<Lag> GenererLag(List<InputData> data )
        {
            throw new NotImplementedException();
        }

        public List<Lag> GenererLag(List<InputData> data, LagGeneratorSpec spec)
        {

            try
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
           int linje = 0;  
            foreach (var skytterIn in data)
            {
                linje++;
                int hold = 0;
                int LagNr = StartSkytter / spec.antallskyttereilaget + 1 ;
                int forsteSkive = StartSkytter % spec.antallskyttereilaget +1;
                while (hold < spec.antallHold)
                {
                    Lag lag = this.GetLagNr(retVal, LagNr, spec.OrionHoldId, spec.antallSkiver);

                    int SkiveNr = (hold* spec.antallskyttereilaget)+forsteSkive;
                    if (lag == null)
                    {
                        Log.Error(string.Format(" Fant ikke Lag {0} linje={1}", LagNr, linje));
                        break;
                    }   
                    var skive = lag.FinnLedigSkive(SkiveNr);
                    if (skive == null)
                    {
                        Log.Error(string.Format(" Fant ikke ledig skive ={0} Lag={1} linje={2}", SkiveNr, LagNr, linje));
                            break;
                    }
                    skive.Skytter = new Skytter()
                                        {
                                            Klasse = skytterIn.Klasse,
                                            Name = skytterIn.Name,
                                            Skytterlag = skytterIn.Skytterlag,
                                            SkytterNr = skytterIn.SkytterNr.ToString()
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
            catch (Exception e)
            {
                Log.Error(e,"Error");
                throw;
            }
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
                                            StartTime = new DateTime(2016, 6, 25, 8, 0, 0),
                                            OrionHoldId = 1
            };

            return spec;
        }

        private Lag GetLagNr(List<Lag> retVal, int lagNr,int orionHoldId, int maxSkiveriLaget)
        {
            var foudLag = retVal.FirstOrDefault(x => x.LagNummer == lagNr && x.OrionHoldId == orionHoldId);
            if (foudLag == null)
            {
                foudLag = new Lag(lagNr, orionHoldId, maxSkiveriLaget);
                retVal.Add(foudLag);
            }

            return foudLag;
        }

       
    }
}
