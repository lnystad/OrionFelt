using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Common.Services
{
    using OrionLag.Common.DataModel;

    public interface IInputPatitionerService
    {

        List<InputData> GetAllComptitiors(string path, string filename);
    }
}
