using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Server.Parsers
{
    using OrionLag.Common.DataModel;
    using OrionLag.Common.Diagnosis;

    public class OrionInputParser
    {
        //1;1;1; Fornavn Skytter1  Fornavn Skytter1; Bodø Østre; R;0;1116
        private const int holdIdIndex = 0;

        private const int lagNrIndex = 1;

        private const int skiveNrIndex = 2;

        private const int NavnIndex = 3;

        private const int skytterlagIndex = 4;

        private const int klasseIndex = 5;

        private const int totalsumIndex = 6;

        private const int skytterNrIndex = 7;

        public List<Lag> ParseLeonOutputFormat(string[] inputLines)
        {
            var retColl = new List<Lag>();
            if (inputLines == null)
            {
                return retColl;
            }

           
            foreach (var line in inputLines)
            {
                string currentLine = line;
                if (currentLine != null)
                {
                    currentLine = currentLine.Trim();
                }

                if (!string.IsNullOrEmpty(currentLine))
                {
                    var elements = line.Split(new char[] { ';' }, StringSplitOptions.None);
                    Lag currentLag = new Lag();

                    if (elements.Length >= 6)
                    {

                        currentLag.OrionHoldId = Convert.ToInt32(elements[holdIdIndex]);
                        currentLag.LagNummer = Convert.ToInt32(elements[lagNrIndex]);
                        var Skive = new Skiver();
                        Skive.SkiveNummer = Convert.ToInt32(elements[skiveNrIndex]);
                        currentLag.SkiverILaget.Add(Skive);

                        var  currentSKytter = new Skytter();
                        currentSKytter.Name = elements[NavnIndex];
                        if (!string.IsNullOrEmpty(currentSKytter.Name))
                        {
                            currentSKytter.Name = currentSKytter.Name.Trim();
                        }

                        currentSKytter.Skytterlag = elements[skytterlagIndex];
                        if (!string.IsNullOrEmpty(currentSKytter.Skytterlag))
                        {
                            currentSKytter.Skytterlag = currentSKytter.Skytterlag.Trim();
                        }

                        currentSKytter.Klasse = elements[klasseIndex];
                        if (!string.IsNullOrEmpty(currentSKytter.Klasse))
                        {
                            currentSKytter.Klasse = currentSKytter.Klasse.Trim();
                        }

                        if (elements.Length > 6 && !string.IsNullOrEmpty(elements[totalsumIndex]))
                        {
                            currentSKytter.TotalSum = Convert.ToInt32(elements[totalsumIndex]);
                        }

                        if (elements.Length > skytterNrIndex && !string.IsNullOrEmpty(elements[skytterNrIndex]))
                        {
                            currentSKytter.SkytterNr = elements[skytterNrIndex];
                            if (!string.IsNullOrEmpty(currentSKytter.SkytterNr))
                            {
                                currentSKytter.SkytterNr = currentSKytter.SkytterNr.Trim();
                            }
                        }
                        if (!string.IsNullOrEmpty(currentSKytter.Name) && !string.IsNullOrEmpty(currentSKytter.SkytterNr))
                        {
                            Skive.Skytter = currentSKytter;
                        }

                        var funnetLag = retColl.FirstOrDefault(x => x.LagNummer == currentLag.LagNummer && x.OrionHoldId == currentLag.OrionHoldId);
                        if (funnetLag != null)
                        {
                            var funnetSkive = funnetLag.SkiverILaget.FirstOrDefault(x => x.SkiveNummer == Skive.SkiveNummer);
                            if (funnetSkive != null)
                            {
                                if (funnetSkive.Skytter != null)
                                {
                                    if (Skive.Skytter != null)
                                    {
                                        Log.Error("Skive already imported {0} Changing skytter from {1} to {2}", funnetSkive.SkiveNummer, funnetSkive.Skytter.Name, Skive.Skytter.Name);
                                        funnetSkive.Skytter = Skive.Skytter;
                                    }
                                    else
                                    {
                                        Log.Error("Skive already imported {0} Removing skytter {1} ", funnetSkive.SkiveNummer, funnetSkive.Skytter.Name);
                                        funnetSkive.Skytter = null;
                                    }
                                    
                                }
                                else
                                {
                                    if (Skive.Skytter != null)
                                    {
                                        Log.Error("Skive already imported {0} setting skytter from null to {1}", funnetSkive.SkiveNummer, Skive.Skytter.Name);
                                        funnetSkive.Skytter = Skive.Skytter;
                                    }
                                }

                            }
                            else
                            {
                                funnetLag.SkiverILaget.Add(Skive);
                            }
                            
                        }
                        else
                        {
                            retColl.Add(currentLag);
                        }
                    }
                }
            }
            return retColl;
        }


        public List<Lag> ParseOrionResultFormat(string[] inputLines)
        {
            //1;1;1;906;Per Wilhelmsen;Bodø Østre ;1;0;00000;00000;00000;0000000000

            var retColl = new List<Lag>();
            if (inputLines == null)
            {
                return retColl;
            }


            foreach (var line in inputLines)
            {
                string currentLine = line;
                if (currentLine != null)
                {
                    currentLine = currentLine.Trim();
                }

                if (!string.IsNullOrEmpty(currentLine))
                {
                    var elements = line.Split(new char[] { ';' }, StringSplitOptions.None);
                    Lag currentLag = new Lag();

                    if (elements.Length >= 6)
                    {

                        currentLag.OrionHoldId = Convert.ToInt32(elements[holdIdIndex]);
                        currentLag.LagNummer = Convert.ToInt32(elements[lagNrIndex]);
                        var Skive = new Skiver();
                        Skive.SkiveNummer = Convert.ToInt32(elements[skiveNrIndex]);
                        currentLag.SkiverILaget.Add(Skive);

                        var currentSKytter = new Skytter();
                        currentSKytter.Name = elements[NavnIndex];
                        currentSKytter.Skytterlag = elements[skytterlagIndex];
                        currentSKytter.Klasse = elements[klasseIndex];
                        if (elements.Length > 6 && !string.IsNullOrEmpty(elements[totalsumIndex]))
                        {
                            currentSKytter.TotalSum = Convert.ToInt32(elements[totalsumIndex]);
                        }

                        if (elements.Length > skytterNrIndex && !string.IsNullOrEmpty(elements[skytterNrIndex]))
                        {
                            currentSKytter.SkytterNr = elements[skytterNrIndex];
                        }
                        if (!string.IsNullOrEmpty(currentSKytter.Name))
                        {
                            Skive.Skytter = currentSKytter;
                        }

                        var funnetLag = retColl.FirstOrDefault(x => x.LagNummer == currentLag.LagNummer && x.OrionHoldId == currentLag.OrionHoldId);
                        if (funnetLag != null)
                        {
                            var funnetSkive = funnetLag.SkiverILaget.FirstOrDefault(x => x.SkiveNummer == Skive.SkiveNummer);
                            if (funnetSkive != null)
                            {
                                if (funnetSkive.Skytter != null)
                                {
                                    if (Skive.Skytter != null)
                                    {
                                        Log.Error("Skive already imported {0} Changing skytter from {1} to {2}", funnetSkive.SkiveNummer, funnetSkive.Skytter.Name, Skive.Skytter.Name);
                                        funnetSkive.Skytter = Skive.Skytter;
                                    }
                                    else
                                    {
                                        Log.Error("Skive already imported {0} Removing skytter {1} ", funnetSkive.SkiveNummer, funnetSkive.Skytter.Name);
                                        funnetSkive.Skytter = null;
                                    }

                                }
                                else
                                {
                                    if (Skive.Skytter != null)
                                    {
                                        Log.Error("Skive already imported {0} setting skytter from null to {1}", funnetSkive.SkiveNummer, Skive.Skytter.Name);
                                        funnetSkive.Skytter = Skive.Skytter;
                                    }
                                }

                            }
                            else
                            {
                                funnetLag.SkiverILaget.Add(Skive);
                            }

                        }
                        else
                        {
                            retColl.Add(currentLag);
                        }
                    }
                }
            }
            return retColl;
        }

        public string[] ParseToOrionInputFormat(List<Lag> orionOutLag)
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
                    string totalFeltsum = string.Empty;
                    string skytterNr = string.Empty;
                    skiveNr = skive.SkiveNummer.ToString();
                    if (skive.Skytter != null)
                    {
                        Navn = skive.Skytter.Name;
                        skytterlag = skive.Skytter.Skytterlag;
                        klasse = skive.Skytter.Klasse;
                        totalsum = skive.Skytter.TotalSum.ToString();
                        totalFeltsum = skive.Skytter.TotalFeltSum.ToString();
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
                        totalFeltsum,
                        skytterNr);
                    retColl.Add(line);
                }
            }


            return retColl.ToArray();
        }
    }
}
