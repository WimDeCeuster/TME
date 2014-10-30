using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces.Colours;

namespace TME.CarConfigurator.Colours
{
    public class ExteriorColourType : BaseObject<Repository.Objects.Colours.ExteriorColourType>, IExteriorColourType
    {
        public ExteriorColourType(Repository.Objects.Colours.ExteriorColourType repositoryColourType)
            : base(repositoryColourType)
        {

        }
    }
}
