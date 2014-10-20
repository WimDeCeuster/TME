using System;
using System.Linq;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class SubModelBuilder
    {
        private readonly SubModel _subModel;

        public SubModelBuilder()
        {
            _subModel = new SubModel();
        }

        public SubModelBuilder WithID(Guid ID)
        {
            _subModel.ID = ID;
            return this;
        }

        public SubModelBuilder WithLabels(params Label[] labels)
        {
            _subModel.Labels = labels.ToList();
            return this;
        }

        public SubModelBuilder WithAssets(params Asset[] assets)
        {
            _subModel.Assets = assets.ToList();
            return this;
        }

        public SubModelBuilder WithGeneration(Generation generation)
        {
            _subModel.Generation = generation;
            return this;
        }

        public SubModel Build()
        {
            return _subModel;
        }
    }
}