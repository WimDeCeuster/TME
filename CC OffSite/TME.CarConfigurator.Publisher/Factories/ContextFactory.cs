using System;
using TME.CarConfigurator.Publisher.Enums;
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
