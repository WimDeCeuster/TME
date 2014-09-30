using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Assets.Enums;

namespace TME.CarConfigurator.CommandRepository
{
    public interface IEngineRepository
    {
        Result.Result Create(Repository.Objects.Context.PublicationTimeFrame context, IEnumerable<Engine> engines);
        Result.Result CreateAssetsForModel(Repository.Objects.Context.Publication context, Guid engineID, Scope scope, IEnumerable<Asset> assets);
        Result.Result CreateAssetsForCar(Repository.Objects.Context.Publication context, Guid engineID, Guid carID, Scope scope, IEnumerable<Asset> assets);
    }
}
