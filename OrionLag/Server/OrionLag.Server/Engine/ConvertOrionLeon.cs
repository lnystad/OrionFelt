using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrionLag.Common.DataModel;

namespace OrionLag.Server.Engine
{
    using System.Windows;

    using OrionLag.Common.Configuration;
    using OrionLag.Common.Diagnosis;
    using OrionLag.Server.Parsers;

    public class ConvertOrionLeon
    {
       

        private int m_AntallHold;
        private int m_AntallSkiver;

        private bool m_Avbrekk;
        private int m_AntallSkyttereILaget;
        private SkuddVerdiParser m_SkuddVerdiParser = new SkuddVerdiParser(true);
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

        public List<HoldDefinition> m_definition;

        public List<HoldDefinition> Definition
        {
            get
            {
                return m_definition;
            }
            set
            {
                m_definition = value;
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
               
                var holdType = ConfigurationLoader.GetAppSettingsValue("HoldType");
                if (string.IsNullOrEmpty(holdType))
                {
                    m_definition = new List<HoldDefinition>();
                    m_definition.Add(new HoldDefinition(1,HoldType.BaneHold));
                    m_definition.Add(new HoldDefinition(2, HoldType.FeltHold));
                    m_definition.Add(new HoldDefinition(3, HoldType.FeltHold));
                    m_definition.Add(new HoldDefinition(4, HoldType.FeltHold));
                    m_definition.Add(new HoldDefinition(5, HoldType.FeltHold));
                    m_definition.Add(new HoldDefinition(6, HoldType.FeltHold));
                }

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
                newres.OrionHoldId = res.OrionHoldId;
                newres.LagTid = res.LagTid;

                foreach (var skive in res.SkiverILaget)
                {
                    if (skive.SkiveNummer <= 0)
                    {
                        Log.Error("Fant skive uten nr");
                    }
                    int StartLeonLag = res.LagNummer;
                    int Leonserienr = 0;
                    int LeonLagNr;
                    int leonSkivenr;
                    if (skive.SkiveNummer <= m_AntallSkyttereILaget)
                    {
                        StartLeonLag = res.LagNummer;
                        LeonLagNr = res.LagNummer;
                        leonSkivenr = skive.SkiveNummer;
                        Leonserienr = 1;
                    }
                    else
                    {
                        int brok = (skive.SkiveNummer -1) / m_AntallSkyttereILaget;
                        Leonserienr = brok + 1;
                        LeonLagNr = StartLeonLag - brok;
                        leonSkivenr = m_AntallSkyttereILaget - skive.SkiveNummer % this.m_AntallSkyttereILaget;

                    }


                    var funnetLag = OrionLag.FirstOrDefault(x => x.LagNummer == LeonLagNr);
                    if (funnetLag != null)
                    {
                        Log.Trace("Fant lagnr {0} ut fra {1}", LeonLagNr, res.LagNummer);
                    }
                    else
                    {
                        funnetLag = new Lag(LeonLagNr, res.OrionHoldId, m_AntallSkyttereILaget);
                        funnetLag.LagTid = res.LagTid;
                        OrionLag.Add(funnetLag);
                    }

                    var funnetSkive = funnetLag.SkiverILaget.FirstOrDefault(x => x.SkiveNummer == leonSkivenr);
                    if (funnetSkive == null)
                    {
                        funnetSkive = new Skiver { SkiveNummer = leonSkivenr };
                        if (skive.Skytter != null)
                        {
                            funnetSkive.Skytter = new Skytter(skive.Skytter);
                            funnetSkive.SkytterGuid = skive.Skytter.Id;
                        }
                        funnetLag.SkiverILaget.Add(funnetSkive);
                    }
                    else
                    {
                        if (funnetSkive.Skytter != null)
                        {
                            if (skive.Skytter != null)
                            {
                                if (skive.Skytter.SkytterNr != funnetSkive.Skytter.SkytterNr)
                                {
                                    Log.Error("Funnet feil i skytter fra lag {0}, skive {1} fant={2} har={3}", res.LagNummer, skive.SkiveNummer, funnetSkive.Skytter.SkytterNr, skive.Skytter.SkytterNr);
                                }
                            }
                        }
                        else
                        {
                            if (skive.Skytter != null)
                            {
                                funnetSkive.Skytter = new Skytter(skive.Skytter);
                                funnetSkive.SkytterGuid = skive.Skytter.Id;
                            }
                        }   
                    }

                }
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
                        if (skive.Skytter != null)
                        {
                            newSkive.Skytter = new Skytter(skive.Skytter);
                        }
                        
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

                int serieNr = 0;
                if (res.SkiveNr <= m_AntallSkyttereILaget)
                {
                    serieNr = 1;
                }
                else
                {
                    int test = res.SkiveNr % this.m_AntallSkyttereILaget;
                    serieNr = res.SkiveNr / this.m_AntallSkyttereILaget;
                    if (test != 0)
                    {
                        serieNr = serieNr + 1;
                    }
                }

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
                    var definition = this.m_definition.Find(defno => defno.HoldNr == serieNr);
                    if (definition != null)
                    {
                        if (definition.HoldType == HoldType.FeltHold)
                        {
                            var feltVerdier = new List<Skuddverdi>();
                            foreach (var value in nyserie.Verdier)
                            {
                                if (value.GetType() == typeof(BaneVerdi))
                                {
                                    var felt=m_SkuddVerdiParser.ParseSkudd(value.ToValue());
                                    feltVerdier.Add(felt);
                                }
                            }

                            nyserie.Verdier = feltVerdier;
                        }
                    }

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

        public List<int> GetLeonLagNumber(int lag)
        {
            List<int> retVal = new List<int>();
            //int start = lag / m_AntallHold;
            //int lag2 = (lag % m_AntallHold) ;
            
            //int StartLag=start *m_AntallHold;
            ////if (start == 0)
            ////{
            ////    StartLag = 0;
            ////}
            ////else
            ////{
            ////    StartLag = start * m_AntallHold;
            ////}

            //int LeonLag = StartLag + lag2;

            int i = 0;
            while (i < m_AntallHold)
            {
                var nyttleonLagnr = lag - i;
                if (nyttleonLagnr > 0)
                {
                    retVal.Add(nyttleonLagnr);
                }
                else
                {
                    break;
                }

                i++;
            }


            return retVal;
        }
    }
}
