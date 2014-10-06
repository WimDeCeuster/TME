using System;
using FakeItEasy;
using TME.CarConfigurator.QueryRepository.Interfaces;

namespace TME.CarConfigurator.QueryRepository.Tests.TestBuilders
{
    internal class ModelRepositoryBuilder
    {
        private readonly IModelRepository _modelRepository;

        private ModelRepositoryBuilder(IModelRepository modelRepository)
        {
            if (modelRepository == null) throw new ArgumentNullException("modelRepository");
            _modelRepository = modelRepository;
        }

        public static ModelRepositoryBuilder InitializeFakeRepository()
        {
            var modelRepository = A.Fake<IModelRepository>();

            return new ModelRepositoryBuilder(modelRepository);
        }

        public IModelRepository Build()
        {
            return _modelRepository;
        }
    }
}