using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrionLag.Common.DataModel;

namespace OrionLag.Server.Engine
{
    using OrionLag.Common.Configuration;
    using OrionLag.Common.Diagnosis;

    public class ConvertOrionLeon
    {
        private int m_AntallHold;
        private int m_AntallSkiver;

        private bool m_Avbrekk;
        private int m_AntallSkyttereILaget;

        public int AntallHold
        {
            get
            {
                return m_AntallHold;
            }
            set
            {
                m_AntallHold = value;
            }
        }

        public int AntallSkiver
        {
            get
            {
                return m_AntallSkiver;
            }
            set
            {
                m_AntallSkiver = value;
            }
        }
        public int AntallSkyttereILaget
        {
            get
            {
                return m_AntallSkyttereILaget;
                ;
            }
            set
            {
                m_AntallSkyttereILaget = value;
            }
        }

        private int m_OrionHoldId;
        public int OrionHoldId
        {
            get
            {
                return m_OrionHoldId;
                ;
            }
            set
            {
                m_OrionHoldId = value;
            }
        }

        public ConvertOrionLeon()
        {

        }

        public void InitConverter()
        {

            try
            {
                m_AntallHold = Intvalue("AntallHold");
                m_OrionHoldId = Intvalue("OrionHoldId");

                m_AntallSkiver = Intvalue("AntallSkiver");
                m_AntallSkyttereILaget = Intvalue("AntallSkyttereILaget");
                var temp = ConfigurationLoader.GetAppSettingsValue("Avbrekk");
                m_Avbrekk = false;
                if (!string.IsNullOrEmpty(temp))
                {
                    bool resVal;
                    if (bool.TryParse(temp, out resVal))
                    {
                        m_Avbrekk = resVal;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "InitConverter()");
                throw;
            }

        }

        private static int Intvalue(string paramName)
        {
            var strVal = ConfigurationLoader.GetAppSettingsValue(paramName);
            if (string.IsNullOrEmpty(strVal))
            {
                throw new ArgumentNullException(paramName, "Parameter can not be null");
            }

            int intvalue = 0;
            if (!int.TryParse(strVal, out intvalue))
            {
                throw new ArgumentNullException(paramName, string.Format("Parameter must be int{0}", strVal));
            }
            return intvalue;
        }

        public List<Lag> ConvertToLeonLag(List<Lag> inputFomLeon)
        {
            List<Lag> OrionLag = new List<Lag>();

            var inputFomLeonSorted = inputFomLeon.OrderBy(x => x.LagNummer);
            foreach (var res in inputFomLeonSorted)
            {
                res.SkiverILaget = res.SkiverILaget.OrderBy(x => x.SkiveNummer).ToList();
            }

            foreach (var res in inputFomLeonSorted)
            {
                Lag newres = new Lag();
                int StartleonLagnr = res.LagNummer / m_AntallSkyttereILaget + 1;
                
                newres.OrionHoldId = res.OrionHoldId;
                newres.LagTid = res.LagTid;

                foreach (var skive in res.SkiverILaget)
                {
                    if (skive.SkiveNummer <= 0)
                    {
                        Log.Error("Fant skive uten nr");
                    }

                    int LeonLagNr;

                    if (skive.SkiveNummer > m_AntallSkyttereILaget)
                    {
                        //Disse skal vi allerede ha inne
                        int lagtilegg = skive.SkiveNummer % m_AntallSkyttereILaget;

                    }
                    else
                    {
                        newres.LagNummer = StartleonLagnr;
                        if (skive.SkiveNummer == 1)
                        {
                            // forventer nytt lag
                        }
                        else
                        {
                            // forventer finne lag
                        }

                    }
                }
                //if (res.Skytter == null)
                //{
                //    continue;
                //}

                //newres.Skytter = new Skytter(res.Skytter);
                //newres.SkytterNr = res.Skytter.SkytterNr;
                //// her må startholdet stå
                //int serieNr = res.SkiveNr / m_AntallSkyttereILaget + 1;

                //newres.SkiveNr = res.SkiveNr % m_AntallSkyttereILaget;
                //if (newres.SkiveNr == 0)
                //{
                //    newres.SkiveNr = m_AntallSkyttereILaget;
                //};

                //var foundSkytter = resColl.FirstOrDefault(x => x.SkytterNr == res.SkytterNr);
                //if (foundSkytter != null)
                //{

                //}
                //else
                //{
                //    newres.Skytter = res.Skytter;
                //    resColl.Add(newres);
                //    foundSkytter = newres;
                //}

                //foreach (var ser in res.Serier)
                //{

                //    var nyserie = new Serie(ser);
                //    nyserie.Nr = serieNr;
                //    foundSkytter.Serier.Add(nyserie);
                //}
            }

            return OrionLag;
        }

        public List<Lag> ConvertToOrionLag(List<Lag> inputFomLeon)
        {
            List<Lag> OrionLag = new List<Lag>();


            int StartSkytter = 0;
            foreach (var leonLag in inputFomLeon)
            {
                int orionHoldId = leonLag.OrionHoldId;
                //var orionLagnr = leonLag.LagNummer % (this.m_AntallSkiver / this.m_AntallSkyttereILaget);
                //var foundOrionLag = OrionLag.FirstOrDefault(x => x.LagNummer == orionLagnr);

                //if (foundOrionLag == null)
                //{
                //    foundOrionLag = new Lag();
                //    foundOrionLag.LagNummer = orionLagnr;
                //    foundOrionLag.OrionHoldId = leonLag.OrionHoldId;
                //    OrionLag.Add(foundOrionLag);
                //}

                //var skytterNr = foundOrionLag.LagNummer * this.m_AntallSkyttereILaget;

                //var tmp = foundOrionLag.LagNummer % this.m_AntallHold;
                //var tmp2 = skytterNr % this.m_AntallHold;
                foreach (var skive in leonLag.SkiverILaget)
                {
                    var skivenr = skive.SkiveNummer - 1;

                    int skytterNr = (leonLag.LagNummer - 1) * m_AntallSkyttereILaget + skivenr;

                    int StartLagNr = skytterNr / m_AntallSkyttereILaget + 1;
                    int forsteSkive = (skytterNr) % m_AntallSkyttereILaget;
                    int hold = 0;
                    while (hold < m_AntallHold)
                    {
                        int LagNr = StartLagNr + hold;
                        Lag lag = this.GetLagNr(OrionLag, LagNr, orionHoldId, m_AntallSkiver);
                        int SkiveNr = (hold * m_AntallSkyttereILaget) + skive.SkiveNummer;
                        //SkiveNr = SkiveNr+ skive.SkiveNummer -1;
                        var newSkive = lag.GetSkiveNr(SkiveNr);
                        newSkive.Skytter = new Skytter(skive.Skytter);
                        hold++;
                        Log.Info("Convert from {0} {1} to {2} {3}", leonLag.LagNummer, skive.SkiveNummer, lag.LagNummer, SkiveNr);
                    }

                }
            }

            return OrionLag;

        }

        public List<SkytterResultat> ConvertToLeonLag(List<SkytterResultat> inputFomOrion)
        {
            List<SkytterResultat> resColl = new List<SkytterResultat>();

            foreach (var res in inputFomOrion)
            {
                SkytterResultat newres = new SkytterResultat();
                int leonLagnr = res.LagNummer % m_AntallSkiver;
                newres.LagNummer = leonLagnr;
                newres.OrionHoldId = res.OrionHoldId;
                newres.Id = res.Id;
                if(res.Skytter==null)
                {
                    continue;
                }

                newres.Skytter = new Skytter(res.Skytter);
                newres.SkytterNr = res.Skytter.SkytterNr;
                // her må startholdet stå
                int serieNr = res.SkiveNr / m_AntallSkyttereILaget +1;

                newres.SkiveNr = res.SkiveNr % m_AntallSkyttereILaget;
                if(newres.SkiveNr == 0)
                {
                    newres.SkiveNr = m_AntallSkyttereILaget;
                };

                var foundSkytter = resColl.FirstOrDefault(x => x.SkytterNr == res.SkytterNr);
                if (foundSkytter != null)
                {

                }
                else
                {
                    newres.Skytter = res.Skytter;
                    resColl.Add(newres);
                    foundSkytter = newres;
                }

                foreach(var ser in res.Serier)
                {

                    var nyserie = new Serie(ser);
                    nyserie.Nr = serieNr;
                    foundSkytter.Serier.Add(nyserie);
                }
            }

            return resColl;
        }

        private Lag GetLagNr(List<Lag> retVal, int lagNr, int orionHoldId, int maxSkiveriLaget)
        {
            var foudLag = retVal.FirstOrDefault(x => x.LagNummer == lagNr);
            if (foudLag == null)
            {
                foudLag = new Lag(lagNr, orionHoldId, maxSkiveriLaget);
                retVal.Add(foudLag);
            }

            return foudLag;
        }
    }
}
