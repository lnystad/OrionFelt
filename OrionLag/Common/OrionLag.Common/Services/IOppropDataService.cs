using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Common.Services
{
    using System.ServiceModel;

    using OrionLag.Common.DataModel;

    [ServiceContract]
    public interface IOppropDataService
    {
        [OperationContract]
        List<Lag> FetchAllLag();
    }
}
