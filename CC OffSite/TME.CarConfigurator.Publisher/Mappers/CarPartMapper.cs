using System;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class CarPartMapper : ICarPartMapper
    {
        public CarPart MapCarPart(Administration.ModelGenerationCarPart generationCarPart)
        {
            return new CarPart
            {
                Code = generationCarPart.Code,
                Name = generationCarPart.Name
            };
        }
    }
}