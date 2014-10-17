using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Extensions;
using TME.CarConfigurator.Interfaces.Factories;

namespace TME.CarConfigurator
{
    public class Steering : BaseObject, ISteering
    {
        readonly Repository.Objects.Steering _repositorySteering;
        
        public Steering(Repository.Objects.Steering repositorySteering)
            : base(repositorySteering)
        {
            if (repositorySteering == null) throw new ArgumentNullException("repositoryWheelDrive");
            
            _repositorySteering = repositorySteering;
        }
    }
}
