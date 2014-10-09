using System;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Common.Interfaces;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IContextFactory
    {
        IContext Get(String brand, String country, Guid generationID, PublicationDataSubset dataSubset);
    }
}