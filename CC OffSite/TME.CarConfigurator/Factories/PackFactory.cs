using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Extensions;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.Packs;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Factories
{
    public class PackFactory : IPackFactory
    {
        private readonly IPackService _packService;
        private readonly IAssetFactory _assetFactory;
        private readonly IEquipmentFactory _equipmentFactory;

        public PackFactory(IPackService packService, IAssetFactory assetFactory, IEquipmentFactory equipmentFactory)
        {
            if (packService == null) throw new ArgumentNullException("packService");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");
            if (equipmentFactory == null) throw new ArgumentNullException("equipmentFactory");

            _packService = packService;
            _assetFactory = assetFactory;
            _equipmentFactory = equipmentFactory;
        }

        public IReadOnlyList<IGradePack> GetGradePacks(Publication publication, Context context, Guid gradeId)
        {
            return TransformIntoGradePacks(_packService.GetGradePacks(publication.ID, publication.GetCurrentTimeFrame().ID, gradeId, context));
        }

        public IReadOnlyList<IGradePack> GetSubModelGradePacks(Publication publication, Context context, Guid subModelId, Guid gradeId)
        {
            return TransformIntoGradePacks(_packService.GetSubModelGradePacks(publication.ID, publication.GetCurrentTimeFrame().ID, gradeId, subModelId, context));
        }

        private static IReadOnlyList<IGradePack> TransformIntoGradePacks(IEnumerable<Repository.Objects.Packs.GradePack> repoPacks)
        {
            return repoPacks.Select(repoPack => new GradePack(repoPack)).ToList();
        }

        public IReadOnlyList<ICarPack> GetCarPacks(Publication publication, Context context, Guid carId)
        {
            return _packService.GetCarPacks(publication.ID, carId, context).Select(pack => new CarPack(pack, publication, carId, context, _assetFactory, _equipmentFactory)).ToList();
        }
    }
}