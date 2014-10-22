using System;
using System.Collections.Generic;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Repository.Objects;
using Car = TME.CarConfigurator.Repository.Objects.Car;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface ISubModelMapper
    {
        SubModel MapSubModel(ModelGenerationSubModel modelGenerationSubModel, IEnumerable<Administration.Car> cars, Boolean isPreview);
    }
}