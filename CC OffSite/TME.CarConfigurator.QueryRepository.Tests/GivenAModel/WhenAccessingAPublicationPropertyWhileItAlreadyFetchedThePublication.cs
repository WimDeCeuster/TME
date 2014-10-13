using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.DI;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Enums;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders.RepositoryObjects;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAModel
{
    public class WhenAccessingAPublicationPropertyWhileItAlreadyFetchedThePublication : TestBase
    {
        private Guid _publicationID;
        private IModel _model;
        private string _firstSsn;
        private string _secondSsn;
        private string _expectedSsn;
        private IPublicationService _publicationService;
        private Context _context;

        protected override void Arrange()
        {
            _publicationID = Guid.NewGuid();
            _expectedSsn = "expected ssn";

            var generation = GenerationBuilder.Initialize()
                .WithSsn(_expectedSsn)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(_publicationID)
                .WithGeneration(generation)
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publicationInfo = new PublicationInfo(publication) { State = PublicationState.Activated };

            var repoModel = new ModelBuilder().AddPublication(publicationInfo).Build();

            _context = ContextBuilder.Initialize().Build();

            _publicationService = A.Fake<IPublicationService>();
            A.CallTo(() => _publicationService.GetPublication(_publicationID, _context)).Returns(publication);

            var modelService = A.Fake<IModelService>();
            A.CallTo(() => modelService.GetModels(A<Context>._)).Returns(new List<Repository.Objects.Model> { repoModel });

            var serviceFacade = new S3ServiceFacade()
                .WithModelService(modelService)
                .WithPublicationService(_publicationService);

            var modelFactory = new ModelFactoryFacade()
                .WithServiceFacade(serviceFacade)
                .Create();

            _model = modelFactory.GetModels(_context).Single();
            
            _firstSsn = _model.SSN; // call publication property a first time
        }

        protected override void Act()
        {
            _secondSsn = _model.SSN; // call publication property a second time
        }

        [Fact]
        public void ThenItShouldNotFetchThePublicationAgain()
        {
            A.CallTo(() => _publicationService.GetPublication(_publicationID, _context)).MustHaveHappened(Repeated.Exactly.Once); // only fetch the publication once, no matter how many times the properties are being called
        }

        [Fact]
        public void ThenItShouldStillGetTheCorrectValue()
        {
            _secondSsn.Should().BeSameAs(_firstSsn);
        }
    }
}