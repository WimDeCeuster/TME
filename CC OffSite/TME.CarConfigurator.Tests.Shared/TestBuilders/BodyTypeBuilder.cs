using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects.Assets;

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

        public BodyTypeBuilder AddVisibleIn(string mode, string view)
        {
            if (_bodyType.VisibleIn == null)
                _bodyType.VisibleIn = new List<VisibleInModeAndView>();

            _bodyType.VisibleIn.Add(new VisibleInModeAndView{Mode = mode, View = view, CanHaveAssets = true});

            return this;
        }

        public Repository.Objects.BodyType Build()
        {
            return _bodyType;
        }
    }
}