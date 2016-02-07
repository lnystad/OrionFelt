namespace OrionLag.Common.DataModel
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlInclude(typeof(FeltVerdi))]
    [XmlInclude(typeof(BaneVerdi))]
    public class Skuddverdi
    {
        public int Nr { get; set; }

        public virtual int Sum()
        {
            return 0;
        }

        public virtual int InnerSum()
        {
            return 0;
        }

        public virtual string ToValue()
        {
            return "E";
        }

      
    }
}
