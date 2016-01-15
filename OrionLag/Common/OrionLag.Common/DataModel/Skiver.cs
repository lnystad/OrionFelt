namespace OrionLag.Common.DataModel
{
    public class Skiver
    {
      
        public Skiver()
        {
        }

        public Skiver(Skiver copy)
        {
            this.SkiveNummer = copy.SkiveNummer;
            this.Free = copy.Free;
            if (copy.Skytter != null)
            {
                this.Skytter = new Skytter(copy.Skytter);
            }
        }

        public Skiver(int skiveteller)
        {
            this.SkiveNummer = skiveteller;
        }

        public int SkiveNummer { get; set; }

        public Skytter Skytter { get; set; }

        public bool Free { get; set; }
    }
}
