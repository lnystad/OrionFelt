namespace OrionLag.Common.DataModel
{
    public class FeltVerdi : Skuddverdi
    {

        public bool? FeltInnerTreff;

        public bool? FeltTreff;

        public override int Sum()
        {
            if (this.FeltInnerTreff.HasValue)
            {
                if (this.FeltInnerTreff.Value)
                {
                    return 1;
                }
            }

            if (this.FeltTreff.HasValue)
            {
                if (this.FeltTreff.Value)
                {
                    return 1;
                }
            }

            return 0;
        }

        public override int InnerSum()
        {
            if (this.FeltInnerTreff.HasValue)
            {
                if (this.FeltInnerTreff.Value)
                {
                    return 1;
                }
            }

            return 0;
        }

        public override string ToValue()
        {
            if (this.FeltInnerTreff.HasValue)
            {
                if (this.FeltInnerTreff.Value)
                {
                    return "*";
                }
            }

            if (this.FeltTreff.HasValue)
            {
                if (this.FeltTreff.Value)
                {
                    return "X";
                }
            }

            return "0";
        }
    }
}