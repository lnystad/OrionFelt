using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Server.Parsers
{
    using OrionLag.Common.DataModel;
    using OrionLag.Common.Diagnosis;

    public class OrionResultParser
    {
        // 1;1;1;906;Per Wilhelmsen; Bodø Østre;1;0;00000;00000;00000;0000000000
        private const int holdIdIndex = 0;

        private const int lagNrIndex = 1;

        private const int skiveNrIndex = 2;
        private const int skytterNrIndex = 3;

        private const int NavnIndex = 4;

        private const int skytterlagIndex = 5;

        private const int klasseIndex = 6;

        private const int totalsumIndex = 7;

        private const int serieStartIndex = 8;

        SkuddVerdiParser m_skuddParser;

        private bool m_feltResultater;

        public OrionResultParser(bool feltResultater)
        {
            m_feltResultater = feltResultater;
            m_skuddParser = new SkuddVerdiParser(feltResultater);
        }

        public List<SkytterResultat> ParseOrionResultOutputFormat(string[] inputLines)
        {
            var retColl = new List<SkytterResultat>();
            if (inputLines == null)
            {
                return retColl;
            }

           
            foreach (var line in inputLines)
            {
                string currentLine = line;
                try
                {
                   
                    if (currentLine != null)
                    {
                        currentLine = currentLine.Trim();
                    }


                    if (!string.IsNullOrEmpty(currentLine))
                    {
                        var elements = line.Split(new char[] { ';' }, StringSplitOptions.None);
                        SkytterResultat currentRes = new SkytterResultat();

                        if (elements.Length >= 6)
                        {

                            currentRes.OrionHoldId = Convert.ToInt32(elements[holdIdIndex]);
                            currentRes.LagNummer = Convert.ToInt32(elements[lagNrIndex]);
                            currentRes.SkiveNr = Convert.ToInt32(elements[skiveNrIndex]);

                            var currentSKytter = new Skytter();
                            currentSKytter.Name = elements[NavnIndex];
                            currentSKytter.Skytterlag = elements[skytterlagIndex];
                            currentSKytter.Klasse = elements[klasseIndex];
                            if (elements.Length > totalsumIndex && !string.IsNullOrEmpty(elements[totalsumIndex]))
                            {
                                int totres;
                                if(int.TryParse(elements[totalsumIndex], out totres))
                                {
                                    currentSKytter.TotalSum = totres;
                                }
                                else
                                {
                                    throw new FormatException(string.Format("{0} not able to parse totsum index {1} {2}", line, totalsumIndex, elements[totalsumIndex]));
                                }
                               
                               
                            }

                            if (elements.Length > skytterNrIndex && !string.IsNullOrEmpty(elements[skytterNrIndex]))
                            {
                                currentSKytter.SkytterNr = elements[skytterNrIndex];
                                currentRes.SkytterNr = currentSKytter.SkytterNr;
                            }

                            if (!string.IsNullOrEmpty(currentSKytter.Name))
                            {
                                currentRes.Skytter = currentSKytter;
                            }


                            var funnetSkytterRes = retColl.FirstOrDefault(x => x.LagNummer == currentRes.LagNummer && x.OrionHoldId == currentRes.OrionHoldId && x.SkytterNr == currentRes.SkytterNr);
                            if (funnetSkytterRes != null)
                            {




                            }
                            else
                            {
                                funnetSkytterRes = currentRes;

                                retColl.Add(currentRes);
                            }

                            int nesteSerieNr = 0;
                            foreach (var serie in funnetSkytterRes.Serier)
                            {
                                if (serie.Nr > nesteSerieNr)
                                {
                                    nesteSerieNr = serie.Nr;
                                }
                            }

                            nesteSerieNr = nesteSerieNr + 1;
                            ParseSerier(elements, serieStartIndex, nesteSerieNr, funnetSkytterRes.Serier);

                        }
                    }
                }
                catch(Exception e)
                {
                    Log.Error(e, currentLine);
                    throw;
                }
            }
            return retColl;
        }

        private void ParseSerier(string[] elements, int serieStartIndex, int nesteSerieNr, List<Serie> serier)
        {
            if(elements.Length< serieStartIndex)
            {
                return;
            }
            int serieNr = nesteSerieNr;
            int currentSerie = serieStartIndex;
            while (currentSerie<elements.Length)
            {
                Serie next = new Serie()
                {
                    Nr = serieNr
                };

                int skuddnr = 1;
                foreach (char c in elements[currentSerie])
                {
                    var verdire=m_skuddParser.ParseSkudd(c.ToString());
                    verdire.Nr = skuddnr;
                    skuddnr++;
                    next.Verdier.Add(verdire);
                }


                if(next.Verdier.Count>0)
                {
                    serier.Add(next);
                }

                currentSerie++;
                serieNr++;
            }
        }

        public string[] ParseToLeonInputFormat(List<Lag> orionOutLag)
        {
            List<string> retColl = new List<string>();

            foreach (var inputLag in orionOutLag)
            {
                string holdId = string.Empty;
                string lagNr = string.Empty;
                holdId = inputLag.OrionHoldId.ToString();
                lagNr = inputLag.LagNummer.ToString();
                foreach (var skive in inputLag.SkiverILaget)
                {
                    string skiveNr = string.Empty;
                    string Navn = string.Empty;
                    string skytterlag = string.Empty;
                    string klasse = string.Empty;
                    string totalsum = string.Empty;
                    string skytterNr = string.Empty;
                    skiveNr = skive.SkiveNummer.ToString();
                    if (skive.Skytter != null)
                    {
                        Navn = skive.Skytter.Name;
                        skytterlag = skive.Skytter.Skytterlag;
                        klasse = skive.Skytter.Klasse;
                        totalsum = skive.Skytter.TotalSum.ToString();
                        skytterNr = skive.Skytter.SkytterNr.ToString();
                    }

                    string line = string.Format(
                        "{0};{1};{2};{3};{4};{5};{6};{7}",
                        holdId,
                        lagNr,
                        skiveNr,
                        Navn,
                        skytterlag,
                        klasse,
                        totalsum,
                        skytterNr);
                    retColl.Add(line);
                }
            }


            return retColl.ToArray();
        }

        public string[] ParseResultToLeonInputFormat(List<SkytterResultat> results)
        {
            List<string> retColl = new List<string>();
            var sortedresults = results.OrderBy(x => x.LagNummer).ToList();
            foreach(var rs in sortedresults)
            {
                // 1; 1; 1; 1116; Fornavn Skytter1 Fornavn Skytter1; Bodø Østre; R; 0; X9 * *87
               if(rs.Skytter==null)
                {
                    continue;
                }
                var sortedserie = rs.Serier.OrderBy(x => x.Nr).ToList();
                int total = 0;
                int felttotal = 0;
                foreach (var ser in sortedserie)
                {
                    total = total + ser.TotalSum();
                }

                foreach (var ser in sortedserie)
                {
                    total = total + ser.FeltSum();
                }

                string line = string.Format(
                       "{0};{1};{2};{3};{4};{5};{6};{7};",
                       rs.OrionHoldId,
                       rs.LagNummer,
                       rs.SkiveNr,
                       rs.SkytterNr,
                       rs.Skytter.Name,
                       rs.Skytter.Skytterlag,
                       rs.Skytter.Klasse,
                       felttotal);

                int count = 0;
                foreach (var ser in rs.Serier)
                {
                    count++;
                    foreach (var verdi in ser.Verdier)
                    {
                        line = line + verdi.ToValue();
                    }
                    if (count < rs.Serier.Count)
                    {
                        line = line + ";";
                    }
                }
                retColl.Add(line);

            }

            return retColl.ToArray();
        }
    }
}
