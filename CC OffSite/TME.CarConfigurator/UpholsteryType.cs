using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces.Colours;

namespace TME.CarConfigurator
{
    public class UpholsteryType : BaseObject<Repository.Objects.Colours.UpholsteryType>, IUpholsteryType
    {
        public UpholsteryType(Repository.Objects.Colours.UpholsteryType repositoryType)
            : base(repositoryType)
        {

        }
    }
}
