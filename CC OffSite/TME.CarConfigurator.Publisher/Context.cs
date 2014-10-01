using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.Publisher
{
    public interface IContext
    {

    }

    public class Context : IContext
    {
        public readonly Guid GenerationId;
        public readonly PublicationDataSubset DataSubset;

        public Context(Guid generationId, PublicationDataSubset dataSubset)
        {
            GenerationId = generationId;
            DataSubset = dataSubset;
        }
    }
}
