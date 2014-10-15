using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class ModelBuilder
    {
        private readonly Model _model;

        public ModelBuilder()
        {
            _model = new Model();
        }

        public static ModelBuilder Initialize()
        {
            return new ModelBuilder();
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