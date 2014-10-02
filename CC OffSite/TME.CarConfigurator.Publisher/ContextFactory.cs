using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TME.CarConfigurator.Publisher
{
    public interface IContextFactory
    {
        IContext Get(String brand, String country, Guid generationID, ICarDbModelGenerationFinder generationFinder, IMapper mapper, PublicationDataSubset dataSubset);
    }

    public class ContextFactory : IContextFactory
    {
        public IContext Get(String brand, String country, Guid generationID, ICarDbModelGenerationFinder generationFinder, IMapper mapper, PublicationDataSubset dataSubset)
        {
            return new Context(brand, country, generationID, generationFinder, mapper, dataSubset);
        }
    }
}
