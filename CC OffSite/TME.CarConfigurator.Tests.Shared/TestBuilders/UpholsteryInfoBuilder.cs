using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects.Colours;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class UpholsteryInfoBuilder
    {
        private UpholsteryInfo _upholsteryInfo;

        public UpholsteryInfoBuilder()
        {
            _upholsteryInfo = new UpholsteryInfo();
        }

        public UpholsteryInfoBuilder WithId(Guid id)
        {
            _upholsteryInfo.ID = id;
            return this;
        }

        public UpholsteryInfo Build()
        {
            return _upholsteryInfo;
        }
    }
}
