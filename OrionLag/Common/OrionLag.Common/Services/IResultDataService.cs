using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Common.Services
{
    using OrionLag.Common.DataModel;

    public interface IResultDataService
    {
        List<Lag> GetLag();
        List<Skytter> GetSkytter();

        List<Resultat> GetResultsForSkytter(Guid skytterId);
    }
}
