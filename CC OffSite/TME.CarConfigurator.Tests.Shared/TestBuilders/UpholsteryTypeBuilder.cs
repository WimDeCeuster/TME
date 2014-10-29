using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects.Colours;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class UpholsteryTypeBuilder
    {
        readonly UpholsteryType _upholsteryType;

        public UpholsteryTypeBuilder()
        {
            _upholsteryType = new UpholsteryType();
        }

        public UpholsteryTypeBuilder WithId(Guid id)
        {
            _upholsteryType.ID = id;

            return this;
        }

        public UpholsteryType Build()
        {
            return _upholsteryType;
        }
    }
}
