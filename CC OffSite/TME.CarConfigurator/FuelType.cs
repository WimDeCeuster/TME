using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;

namespace TME.CarConfigurator
{
    public class FuelType : BaseObject<Repository.Objects.FuelType>, IFuelType
    {

        public FuelType(Repository.Objects.FuelType repositoryFuelType)
            : base(repositoryFuelType)
        {
        }
        
        public bool Hybrid { get { return RepositoryObject.Hybrid; } }
    }
}