using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Factories.Interfaces;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.QueryRepository.Interfaces;
using TME.CarConfigurator.QueryRepository.Tests.TestBuilders;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders.RepositoryObjects;
using Xunit;

namespace TME.CarConfigurator.QueryRepository.Tests.GivenAModel
{
    public class WhenAccessingAPublicationPropertyWhileItDidNotFetchThePublicationYet : TestBase
    {
        private Guid _publicationID;
        private IModel _model;
        private string _actualSsn;
        private string _expectedSsn;
        private IPublicationFactory _publicationFactory;
        private IPublicationRepository _publicationRepository;
        private Context _context;

        protected override void Arrange()
        {
            _publicationID = Guid.NewGuid();
            _expectedSsn = "expected ssn";

            _context = ContextBuilder.InitializeFakeContext().Build();

            ArrangePublicationFactory();

            var modelFactory = ArrangeModelFactory();

            _model = modelFactory.GetModels(_context).Single();
        }

        private void ArrangePublicationFactory()
        {
            _publicationRepository = A.Fake<IPublicationRepository>();

            A.CallTo(() => _publicationRepository.GetPublication(_publicationID, _context))
                .Returns(
                    PublicationBuilder.Initialize()
                        .WithGeneration(GenerationBuilder.Initialize()
                        .WithSsn(_expectedSsn)
                        .Build()
                    )
                    .Build());

            _publicationFactory = PublicationFactoryBuilder.Initialize()
                .WithPublicationRepository(_publicationRepository)
                .Build();
        }

        private IModelFactory ArrangeModelFactory()
        {
            var publicationInfo = PublicationInfoBuilder.Initialize()
                .WithID(_publicationID)
                .WithGeneration(GenerationBuilder.Initialize().Build())
                .CurrentlyActive()
                .Build();

            var repoModel = ModelBuilder.Initialize().AddPublication(publicationInfo).Build();

            var modelRepository = A.Fake<IModelRepository>();
            A.CallTo(() => modelRepository.GetModels(A<Context>._)).Returns(new List<Repository.Objects.Model> { repoModel });

            return ModelFactoryBuilder.Initialize()
                .WithModelRepository(modelRepository)
                .WithPublicationFactory(_publicationFactory)
                .Build();
        }

        protected override void Act()
        {
            _actualSsn = _model.SSN;
        }

        [Fact]
        public void ThenItShouldGetThePublicationFromThePublicationFactory()
        {
            A.CallTo(() => _publicationRepository.GetPublication(_publicationID, _context)).MustHaveHappened(Repeated.Exactly.Once);

        }

        [Fact]
        public void ThenItShouldHaveThePropertyFilledCorrectly()
        {
            _actualSsn.Should().Be(_expectedSsn);
        }
    }
}