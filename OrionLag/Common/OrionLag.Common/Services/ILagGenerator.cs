using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Common.Services
{
    using OrionLag.Common.DataModel;

    public interface ILagGenerator
    {
        List<Lag> GenererLag(List<InputData> data, LagGeneratorSpec spec);

        LagGeneratorSpec GetSpecFromConfiguration();
    }
}
