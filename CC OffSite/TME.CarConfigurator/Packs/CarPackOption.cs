using System;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Packs
{
    public class CarPackOption : CarPackEquipmentItem<Repository.Objects.Packs.CarPackOption>, ICarPackOption
    {
        public CarPackOption(Repository.Objects.Packs.CarPackOption repositoryCarPackOption, Publication publication, Guid carId, Context context, IAssetFactory assetFactory)
            : base(repositoryCarPackOption, publication, carId, context, assetFactory)
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
            get { throw new NotImplementedException(); }
        }
    }
}
