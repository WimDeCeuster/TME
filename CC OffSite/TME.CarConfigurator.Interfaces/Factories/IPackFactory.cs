using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface IPackFactory
    {
        IReadOnlyList<IGradePack> GetGradePacks(Publication publication, Context context, Guid gradeId);
        IReadOnlyList<IGradePack> GetSubModelGradePacks(Publication publication, Context context, Guid subModelID, Guid gradeId);
        IReadOnlyList<ICarPack> GetCarPacks(Publication publication, Context context, Guid carId);

    }
}