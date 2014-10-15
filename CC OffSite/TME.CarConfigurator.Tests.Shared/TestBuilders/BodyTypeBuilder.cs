using System;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class BodyTypeBuilder
    {
        private readonly Repository.Objects.BodyType _bodyType;

        public BodyTypeBuilder()
        {
            _bodyType = new Repository.Objects.BodyType();
        }

        public BodyTypeBuilder WithId(Guid id)
        {
            _bodyType.ID = id;

            return this;
        }

        public Repository.Objects.BodyType Build()
        {
            return _bodyType;
        }
    }
}