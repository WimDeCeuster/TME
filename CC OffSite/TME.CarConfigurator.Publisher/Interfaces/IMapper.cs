using System;
using TME.CarConfigurator.Publisher.Common.Interfaces;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IMapper
    {
        IContext Map(String brand, String country, Guid generationID, ICarDbModelGenerationFinder generationFinder, IContext context);
    }
}