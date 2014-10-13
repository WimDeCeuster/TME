using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders.RepositoryObjects
{
    public class GenerationBuilder
    {
        private readonly Generation _generation;

        public GenerationBuilder()
        {
            _generation = new Generation();
        }

        public static GenerationBuilder Initialize()
        {
            return new GenerationBuilder();
        }

        public GenerationBuilder WithID(Guid generationID)
        {
            _generation.ID = generationID;

            return this;
        }

        public GenerationBuilder WithSsn(string ssn)
        {
            _generation.SSN = ssn;

            return this;
        }

        public GenerationBuilder AddAsset(Asset asset)
        {
            if (_generation.Assets == null) 
                _generation.Assets = new List<Asset>();

            _generation.Assets.Add(asset);

            return this;
        }

        public Generation Build()
        {
            return _generation;
        }
    }
}