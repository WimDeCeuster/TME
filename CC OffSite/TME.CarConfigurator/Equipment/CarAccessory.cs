using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator.Equipment
{
    public class CarAccessory : CarEquipmentItem<Repository.Objects.Equipment.CarAccessory>, ICarAccessory
    {
        public CarAccessory(Repository.Objects.Equipment.CarAccessory repositoryObject)
            : base(repositoryObject)
        {
        }

        public IPrice BasePrice { get { return new Price(RepositoryObject.BasePrice); } }

        public IMountingCosts MountingCostsOnNewVehicle { get { return new MountingCosts(RepositoryObject.MountingCostsOnNewVehicle); } }

        public IMountingCosts MountingCostsOnUsedVehicle { get { return new MountingCosts(RepositoryObject.MountingCostsOnUsedVehicle); } }
    }
}