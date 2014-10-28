using System;
using TME.CarConfigurator.Repository.Objects.Packs;

namespace TME.CarConfigurator.Tests.Shared
{
    public class GradePackBuilder
    {
        private Guid _id;

        public GradePackBuilder WithID(Guid id)
        {
            _id = id;

            return this;
        }

        public GradePack Build()
        {
            return new GradePack { ID = _id };
        }
    }
}