using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.QueryRepository.Tests.TestBuilders.RepositoryObjects
{
    internal class GenerationBuilder
    {
        private readonly Generation _generation;

        private GenerationBuilder(Generation generation)
        {
            _generation = generation;
        }

        public static GenerationBuilder Initialize()
        {
            var generation = new Generation();

            return new GenerationBuilder(generation);
        }

        public GenerationBuilder WithSsn(string ssn)
        {
            _generation.SSN = ssn;

            return this;
        }

        public Generation Build()
        {
            return _generation;
        }
    }
}