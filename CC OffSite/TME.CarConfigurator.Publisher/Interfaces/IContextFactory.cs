using System;
using TME.CarConfigurator.Publisher.Enums;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IContextFactory
    {
        IContext Get(String brand, String country, Guid generationID, PublicationDataSubset dataSubset);
    }
}