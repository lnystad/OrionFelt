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
        }

        public Skytter(int skytterNr, string name, string skytterlag, string klasse)
        {
            this.SkytterNr = skytterNr;
            this.Name = name;
            this.Skytterlag = skytterlag;
            this.Klasse = klasse;
        }
        public Guid Id { get; set; }
        public int SkytterNr { get; set; }
        public string Name { get; set; }

        public string Skytterlag { get; set; }

        public string Klasse { get; set; }
    }
}
