using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces.Colours;

namespace TME.CarConfigurator.Colours
{
    public class UpholsteryType : BaseObject<Repository.Objects.Colours.UpholsteryType>, IUpholsteryType
    {
        public UpholsteryType(Repository.Objects.Colours.UpholsteryType repositoryType)
            : base(repositoryType)
        {

        }
    }
}
