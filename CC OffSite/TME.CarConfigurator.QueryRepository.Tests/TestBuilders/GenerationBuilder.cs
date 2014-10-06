using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.QueryRepository.Tests.TestBuilders
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

        public Generation Build()
        {
            return _generation;
        }
    }
}