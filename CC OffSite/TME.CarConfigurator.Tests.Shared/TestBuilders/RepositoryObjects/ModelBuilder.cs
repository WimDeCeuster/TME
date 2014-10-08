using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders.RepositoryObjects
{
    public class ModelBuilder
    {
        private readonly Model _model;

        private ModelBuilder(Model model)
        {
            _model = model;
        }

        public static ModelBuilder Initialize()
        {
            var model = new Model();

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

        public Model Build()
        {
            return _model;
        }
    }
}