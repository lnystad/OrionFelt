using OrionLag.Common.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Server.Parsers
{
    public class SkuddVerdiParser
    {
        private bool m_parseasFelt;

        public SkuddVerdiParser(bool felt)
        {
            this.m_parseasFelt = felt;

        }

        public Skuddverdi ParseSkudd(string input)
        {
            if (!this.m_parseasFelt)
            {
                return ParseBaneSkudd(input);
            }

            return ParseFeltSkudd(input);
        }

        private Skuddverdi ParseFeltSkudd(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            switch (input.Trim().ToUpper())
            {
                case "X":
                    return new FeltVerdi() { FeltTreff = true, FeltInnerTreff = false };

                case "*":
                    return new FeltVerdi() { FeltTreff = true, FeltInnerTreff = true };
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    return new FeltVerdi() { FeltTreff = false, FeltInnerTreff = false };
                default:
                    return null;
            }


            return null;
        }

        private Skuddverdi ParseBaneSkudd(string input)
        {
            if(string.IsNullOrEmpty(input))
            {
                return null;
            }
            
            switch(input.Trim().ToUpper())
            {
                case "X":
                    return new BaneVerdi()
                    {
                         InnerTier = false,
                        Verdi=10
                    };
                   
                case "*":
                    return new BaneVerdi()
                    {
                        InnerTier = true,
                        Verdi = 10
                    };
                case "0":
                    return new BaneVerdi()
                    {
                        InnerTier = false,
                        Verdi = 0
                    };
                case "1":
                    return new BaneVerdi()
                    {
                        InnerTier = false,
                        Verdi = 1
                    };
                case "2":
                    return new BaneVerdi()
                    {
                        InnerTier = false,
                        Verdi = 2
                    };
                case "3":
                    return new BaneVerdi()
                    {
                        InnerTier = false,
                        Verdi = 3
                    };
                case "4":
                    return new BaneVerdi()
                    {
                        InnerTier = false,
                        Verdi = 4
                    };
                case "5":
                    return new BaneVerdi()
                    {
                        InnerTier = false,
                        Verdi = 5
                    };
                case "6":
                    return new BaneVerdi()
                    {
                        InnerTier = false,
                        Verdi = 6
                    };
                case "7":
                    return new BaneVerdi()
                    {
                        InnerTier = false,
                        Verdi = 7
                    };
                case "8":
                    return new BaneVerdi()
                    {
                        InnerTier = false,
                        Verdi = 8
                    };
                case "9":
                    return new BaneVerdi()
                    {
                        InnerTier = false,
                        Verdi = 9
                    };
                default:
                    return null;
            }


            return null;
        }
    }
}
