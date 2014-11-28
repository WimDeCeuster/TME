using System;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Equipment
{
    public class CarOption : CarEquipmentItem<Repository.Objects.Equipment.CarOption>, ICarOption
    {
        private readonly IOptionInfo _parentOptionInfo;

        public CarOption(Repository.Objects.Equipment.CarOption repositoryObject, IOptionInfo parentOptionInfo, Guid carID, Publication publication, Context context, IAssetFactory assetFactory, IRuleFactory ruleFactory) 
            : base(repositoryObject,publication,carID,context,assetFactory, ruleFactory)
        {
            _parentOptionInfo = parentOptionInfo;
        }

        public override IPrice Price { get { return new Price(RepositoryObject.Price); }}

        public bool TechnologyItem { get { return RepositoryObject.TechnologyItem; } }

        public IOptionInfo ParentOption { get { return _parentOptionInfo; } }

        public bool PostProductionOption { get { return RepositoryObject.PostProductionOption; } }
        public bool SuffixOption { get { return RepositoryObject.SuffixOption; } }
    }
}