using System;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class SubModelBuilder
    {
        private readonly SubModel _subModel;

        public SubModelBuilder()
        {
            _subModel = new SubModel();
        }

        public SubModel Build()
        {
            return _subModel;
        }

        public SubModelBuilder WithID(Guid ID)
        {
            _subModel.ID = ID;
            return this;
        }
    }
}