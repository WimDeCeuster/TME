using System;
using TME.CarConfigurator.Publisher.Enums;
using TME.CarConfigurator.Publisher.S3;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IContextFactory
    {
        IContext Get(String brand, String country, Guid generationID, PublicationDataSubset dataSubset);
    }
}