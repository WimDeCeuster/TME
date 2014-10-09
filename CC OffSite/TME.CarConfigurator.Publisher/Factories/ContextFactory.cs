using System;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;

namespace TME.CarConfigurator.Publisher.Factories
{
    public class ContextFactory : IContextFactory
    {
        public IContext Get(String brand, String country, Guid generationID, PublicationDataSubset dataSubset)
        {
            return new Context(brand, country, generationID, dataSubset);
        }
    }
}
