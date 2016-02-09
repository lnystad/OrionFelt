using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Server.Services
{
    using OrionLag.Common.DataModel;
    using OrionLag.Common.Services;
    using OrionLag.Server.Database;

    public class ResultDataService :IResultDataService
    {
        DatabaseEngine dbEngine ;

        public ResultDataService()
        {
            dbEngine = new DatabaseEngine();
            dbEngine.Init();
        }

        public Lag GetLag(int lagNr)
        {
            return dbEngine.GetLag(lagNr);
        }

        public List<Lag> GetLag()
        {
            return dbEngine.GetLag();
        }

        public List<Skytter> GetSkytter()
        {
            throw new NotImplementedException();
        }

        public List<Resultat> GetResultsForSkytter(Guid skytterId)
        {
            return dbEngine.GetResultsForSkytter(skytterId);
        }
    }
}
