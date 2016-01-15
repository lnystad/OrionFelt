namespace OrionLag.Common.DataModel
{
    using System.Collections.Generic;

    public class Serie
    {
        public Serie()
        {
            this.Verdier = new List<Skuddverdi>();
        }
        public string Navn { get; set; }

        public int AntallTellende { get; set; }

        public List<Skuddverdi> Verdier { get; set; } 
    }
}
