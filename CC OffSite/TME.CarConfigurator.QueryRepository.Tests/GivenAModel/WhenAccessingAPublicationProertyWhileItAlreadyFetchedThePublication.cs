using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Factories.Interfaces;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.QueryRepository.Tests.TestBuilders;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.QueryServices.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders.RepositoryObjects;
using Xunit;

namespace TME.CarConfigurator.QueryRepository.Tests.GivenAModel
{
    public class WhenAccessingAPublicationProertyWhileItAlreadyFetchedThePublication : TestBase
    {
        private Guid _publicationID;
        private IModel _model;
        private string _actualSsn;
        private string _expectedSsn;
        private IPublicationFactory _publicationFactory;
        private IPublicationService _publicationRepository;
        private Context _context;

        protected override void Arrange()
        {
            _publicationID = Guid.NewGuid();
            _expectedSsn = "expected ssn";

            _context = ContextBuilder.Initialize().Build();

            ArrangePublicationFactory();

            var modelFactory = ArrangeModelFactory();

            _model = modelFactory.GetModels(_context).Single();

            var dumnmy = _model.SSN; // call publication property a first time
        }

        private void ArrangePublicationFactory()
        {
            _publicationRepository = A.Fake<IPublicationService>();

            A.CallTo(() => _publicationRepository.GetPublication(_publicationID, _context))
                .Returns(
                    PublicationBuilder.Initialize()
                        .WithGeneration(GenerationBuilder.Initialize()
                            .WithSsn(_expectedSsn)
                            .Build()
                        )
                        .Build());

            _publicationFactory = PublicationFactoryBuilder.Initialize()
                .WithPublicationService(_publicationRepository)
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

            var modelRepository = A.Fake<IModelService>();
            A.CallTo(() => modelRepository.GetModels(A<Context>._)).Returns(new List<Repository.Objects.Model> { repoModel });

            return ModelFactoryBuilder.Initialize()
                .WithModelService(modelRepository)
                .WithPublicationFactory(_publicationFactory)
                .Build();
        }

        protected override void Act()
        {
            _actualSsn = _model.SSN; // call publication property a second time
        }

        [Fact]
        public void ThenItShouldNotFetchThePublicationAgain()
        {
            A.CallTo(() => _publicationRepository.GetPublication(_publicationID, _context)).MustHaveHappened(Repeated.Exactly.Once); // only fetch the publication once, no matter how many times the properties are being called
        }

        [Fact]
        public void ThenItShouldStillGetTheCorrectValue()
        {
            _actualSsn.Should().Be(_expectedSsn);
        }
    }
}