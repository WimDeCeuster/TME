using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces.Colours;

namespace TME.CarConfigurator
{
    public class ExteriorColourType : BaseObject<Repository.Objects.Colours.ExteriorColourType>, IExteriorColourType
    {
        public ExteriorColourType(Repository.Objects.Colours.ExteriorColourType repositoryColourType)
            : base(repositoryColourType)
        {

        }
    }
}
