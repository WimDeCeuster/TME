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
        readonly IPackService _packService;
        readonly IAssetFactory _assetFactory;
        readonly IEquipmentFactory _equipmentFactory;
        readonly IRuleFactory _ruleFactory;
        readonly IColourFactory _colourFactory;

        public PackFactory(IPackService packService, IAssetFactory assetFactory, IEquipmentFactory equipmentFactory, IRuleFactory ruleFactory, IColourFactory colourFactory)
        {
            if (packService == null) throw new ArgumentNullException("packService");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");
            if (equipmentFactory == null) throw new ArgumentNullException("equipmentFactory");
            if (ruleFactory == null) throw new ArgumentNullException("ruleFactory");
            if (colourFactory == null) throw new ArgumentNullException("colourFactory");

            _packService = packService;
            _assetFactory = assetFactory;
            _equipmentFactory = equipmentFactory;
            _ruleFactory = ruleFactory;
            _colourFactory = colourFactory;
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
            return _packService.GetCarPacks(publication.ID, carId, context).Select(pack => new CarPack(pack, publication, carId, context, _assetFactory, _equipmentFactory, _ruleFactory, _colourFactory)).ToList();
        }
    }
}