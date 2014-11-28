using System;
using TME.CarConfigurator.Equipment;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Packs
{
    public class CarPackOption : CarPackEquipmentItem<Repository.Objects.Packs.CarPackOption>, ICarPackOption
    {
        private OptionInfo _parentOption;

        public CarPackOption(Repository.Objects.Packs.CarPackOption repositoryCarPackOption, Publication publication, Guid carId, Context context, IAssetFactory assetFactory, IRuleFactory ruleFactory)
            : base(repositoryCarPackOption, publication, carId, context, assetFactory, ruleFactory)
        {

        }

        public bool PostProductionOption
        {
            get { return RepositoryObject.PostProductionOption; }
        }

        public bool SuffixOption
        {
            get { return RepositoryObject.SuffixOption; }
        }

        public bool TechnologyItem
        {
            get { return RepositoryObject.TechnologyItem; }
        }

        public IOptionInfo ParentOption
        {
            get { return _parentOption = _parentOption ?? (RepositoryObject.ParentOption == null ? null : new OptionInfo(RepositoryObject.ParentOption)); }
        }
    }
}
