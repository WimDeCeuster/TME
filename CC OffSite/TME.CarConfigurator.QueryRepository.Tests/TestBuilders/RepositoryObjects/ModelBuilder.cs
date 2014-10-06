using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.QueryRepository.Tests.TestBuilders.RepositoryObjects
{
    internal class ModelBuilder
    {
        private readonly Repository.Objects.Model _model;

        private ModelBuilder(Repository.Objects.Model model)
        {
            _model = model;
        }

        public static ModelBuilder Initialize()
        {
            var model = new Repository.Objects.Model();

            return new ModelBuilder(model);
        }

        public ModelBuilder WithName(string modelName)
        {
            _model.Name = modelName;

            return this;
        }

        public ModelBuilder AddPublication(PublicationInfo publication)
        {
            _model.Publications.Add(publication);

            return this;
        }

        public Repository.Objects.Model Build()
        {
            return _model;
        }
    }
}