namespace OrionLag.Common.DataModel
{
    using System;

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
                this.SkytterGuid = copy.Skytter.Id;
            }
        }

        public Skiver(int skiveteller)
        {
            this.SkiveNummer = skiveteller;
        }

        public int SkiveNummer { get; set; }

        private Skytter m_skytter;

        public Skytter Skytter
        {
            get
            {
                return m_skytter;
            }
            set
            {
                m_skytter = value;
                if (m_skytter != null)
                {
                    SkytterGuid = m_skytter.Id;
                }
            }
        }

        public Guid? SkytterGuid { get; set; }

        public bool Free { get; set; }
    }
}
