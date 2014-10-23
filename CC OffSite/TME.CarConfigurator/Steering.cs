using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;

namespace TME.CarConfigurator
{
    public class Steering : BaseObject<Repository.Objects.Steering>, ISteering
    {
        
        public Steering(Repository.Objects.Steering repositorySteering)
            : base(repositorySteering)
        {
        }
    }
}
