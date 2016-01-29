namespace OrionLag.Common.DataModel
{
    using System;

    public class Skytter
    {
        public Skytter()
        {
            this.Id =Guid.NewGuid();
        }
        public Skytter(Skytter copy)
        {
            this.SkytterNr = copy.SkytterNr;
            this.Name = copy.Name;
            this.Skytterlag = copy.Skytterlag;
            this.Klasse = copy.Klasse;
            this.Id = copy.Id;
            this.SkytterlagNr = copy.SkytterlagNr;
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
                return this.Fornavn + ' ' + this.EtterNavn;
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
                            }
                            else
                            {
                                tmpFornavn = tmpFornavn + ' ' + nameelements[cx];
                            }

                            cx++;
                        }

                        this.Fornavn = tmpFornavn;
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
    }
}
