using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Result.ViewModel
{
    using OrionLag.Common.DataModel;
    using OrionLag.Common.Services;
    using OrionLag.WpfBase;

    public class ResultRowViewModel : TargetWindowBase
    {

        public ResultRowViewModel(int lagNummer , int skiveNummer,Skytter coSkytter=null,List<Resultat> result=null )
        {
            this.LagNummer = lagNummer;
            this.SkiveNummer = skiveNummer;
            if (coSkytter != null)
            {
                this.Name = coSkytter.Name;
                this.SkytterNr = coSkytter.SkytterNr;
                this.Klasse = coSkytter.Klasse;
                this.Skytterlag = coSkytter.Skytterlag;
            }

            if (result != null && result.Count > 0)
            {
                
                foreach (var res in result)
                {
                    int i = 0;
                   
                    foreach (var serie in res.Serier)
                    {
                        if (!serie.Valid)
                        {
                            continue;
                        }

                        i++;
                        if (i == 1)
                        {
                            int sum = res.TotalSum();
                            this.Res1 = sum.ToString();
                        }
                        else
                        {
                            int sum = res.FeltSum(i);
                            int sum2 = res.FeltInnerSum(i);
                            if (i == 2)
                            {
                                this.Res2 = string.Format("{0}/{1}", sum, sum2);
                            }
                            else if (i == 3)
                            {
                                this.Res3 = string.Format("{0}/{1}", sum, sum2);
                            }
                            else if (i == 4)
                            {
                                this.Res4 = string.Format("{0}/{1}", sum, sum2);
                            }
                            else if (i == 5)
                            {
                                this.Res5 = string.Format("{0}/{1}", sum, sum2);
                            }
                            else if (i == 6)
                            {
                                this.Res6 = string.Format("{0}/{1}", sum, sum2);
                            }
                        }
                    }
                    int totSum = 0;
                    int totSumInner = 0;
                    totSum = totSum + res.FeltSum();
                    totSumInner = totSumInner + res.FeltInnerSum();
                    this.Total = string.Format("{0}/{1}", totSum, totSumInner);
                }
            }

        }
       
        public int LagNummer { get; set; }
        public int SkiveNummer { get; set; }
        public string SkytterNr { get; set; }
        public string Name { get; set; }
        public string Klasse { get; set; }
        public string Skytterlag { get; set; }
        public string Res1 { get; set; }
        public string Res2 { get; set; }
        public string Res3 { get; set; }
        public string Res4 { get; set; }
        public string Res5 { get; set; }
        public string Res6 { get; set; }
        public string Total { get; set; }
        
    }
}
