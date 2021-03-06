﻿namespace OrionLag.Common.DataModel
{
    using System;

    using OrionLag.Common.Diagnosis;

    public class Skytter
    {
        public Skytter()
        {
            this.Id =Guid.NewGuid();
        }
        public Skytter(Skytter copy)
        {
            if (copy == null)
            {
                Log.Error("Copy Skytter is null ");
            }

            this.SkytterNr = copy.SkytterNr;
            this.Name = copy.Name;
            this.Skytterlag = copy.Skytterlag;
            this.Klasse = copy.Klasse;
            this.Id = copy.Id;
            this.SkytterlagNr = copy.SkytterlagNr;
            this.TotalSum = copy.TotalSum;
            this.TotalFeltSum = copy.TotalFeltSum;
        }

        public Skytter(string skytterNr, string name, int orionId, string skytterlag, int SkytterlagNr, string klasse)
        {
            this.SkytterNr = skytterNr;
            this.Name = name;
            this.Skytterlag = skytterlag;
            this.Klasse = klasse;
            this.SkytterlagNr = SkytterlagNr;
        }

        public Guid Id { get; set; }

       
        public string SkytterNr { get; set; }

        public string Name
        {
            get
            {
                var name = string.Empty;
                if (!string.IsNullOrEmpty(this.Fornavn))
                {
                    name = this.Fornavn;
                }

                if (!string.IsNullOrEmpty(this.EtterNavn))
                {
                    name = name + ' ' + this.EtterNavn;
                }

                return name;
            }
            set
            {
                string tmpname = string.Empty;
                if (value != null)
                {
                    tmpname = value.Trim();
                }
               
                
                if (string.IsNullOrEmpty(tmpname))
                {
                    this.EtterNavn = string.Empty;
                    this.Fornavn = string.Empty;
                }
                else
                {
                    
                    var nameelements = tmpname.Split(new char[] { ' ', ',' },StringSplitOptions.RemoveEmptyEntries);
                    if (nameelements.Length > 0)
                    {
                        int cx = 0;
                        string tmpFornavn = string.Empty;
                        foreach (var nameel in nameelements)
                        {

                            if (cx == nameelements.Length - 1)
                            {
                                this.EtterNavn = nameelements[cx];
                                if (!string.IsNullOrEmpty(this.EtterNavn))
                                {
                                    this.EtterNavn = this.EtterNavn.Trim();
                                }
                            }
                            else
                            {
                                tmpFornavn = tmpFornavn + ' ' + nameelements[cx];
                            }

                            cx++;
                        }

                        this.Fornavn = tmpFornavn;
                        if (!string.IsNullOrEmpty(this.Fornavn))
                        {
                            this.Fornavn = this.Fornavn.Trim();
                        }
                    }
                    else
                    {
                        this.EtterNavn = string.Empty;
                        this.Fornavn = string.Empty;
                    }
                }
            }
        }
        public string Fornavn { get; set; }
        public string EtterNavn { get; set; }
        public string Skytterlag { get; set; }

        public int SkytterlagNr { get; set; }

        public string Klasse { get; set; }
        public int TotalSum { get; set; }
        public int TotalFeltSum { get; set; }
    }
}
