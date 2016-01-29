namespace OrionLag.Common.DataModel
{
    public class FeltVerdi : Skuddverdi
    {
        public bool? FeltTreff;
        public bool? FeltInnerTreff;

        public override int Sum()
        {
            if (FeltTreff.HasValue)
            {
                return 1;
            }

            if (FeltInnerTreff.HasValue)
            {
                return 1;
            }

            return 0;
        }

        public override string ToValue()
        {
            if (FeltInnerTreff.Value)
            {
                return "*";
            }

            if (FeltTreff.HasValue)
            {
                    return "X";
            }

           
            return "0";
        }
    }
}
