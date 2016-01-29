using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Common.Services
{
    using System.Collections.ObjectModel;

    using OrionLag.Common.DataModel;

    public interface ILagOppsettDataService
    {
        LagOppsettConfig GetOppsettConfig();
        Collection<Lag> GetAllFromDatabase(string baseName);
        void StoreToDatabase(Collection<Lag> allinfo, string baseName);
    }
}
