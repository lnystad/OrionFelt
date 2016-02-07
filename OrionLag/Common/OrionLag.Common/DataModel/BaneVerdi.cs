namespace OrionLag.Common.DataModel
{
    using System;

    public class BaneVerdi: Skuddverdi
    {
        public bool? InnerTier;
        public int? Verdi;
        public int? DecimalVerdi;


        public override int InnerSum()
        {
            if (InnerTier.HasValue)
            {
                if(InnerTier.Value) 
                {
                    return 1;
                }
            }

            return 0;
        }

        public override int Sum()
        {
            if (Verdi.HasValue)
            {
                return Verdi.Value;
            }

            if (DecimalVerdi.HasValue)
            {
                return Convert.ToInt32(DecimalVerdi.Value);
            }

            return 0;
        }

        public override string ToValue()
        {
            if (InnerTier.HasValue)
            {
                if (InnerTier.Value)
                {
                    return "*";
                }
            }

            if (!Verdi.HasValue)
            {
                return "0";
            }

            if (Verdi.Value == 10 )
            {
                return "X";
            }

            return Verdi.Value.ToString();
        }
    }
}
