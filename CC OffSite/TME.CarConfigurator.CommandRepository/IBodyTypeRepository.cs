using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Assets.Enums;

namespace TME.CarConfigurator.CommandRepository
{
    public interface IBodyTypeRepository
    {
        Result.Result Create(Repository.Objects.Context.PublicationTimeFrame context, IEnumerable<BodyType> bodyTypes);
        Result.Result CreateAssetsForModel(Repository.Objects.Context.Publication context, Guid bodyTypeID, Scope scope, IEnumerable<Asset> assets);
        Result.Result CreateAssetsForCar(Repository.Objects.Context.Publication context, Guid bodyTypeID, Guid carID, Scope scope, IEnumerable<Asset> assets);
    }
}
