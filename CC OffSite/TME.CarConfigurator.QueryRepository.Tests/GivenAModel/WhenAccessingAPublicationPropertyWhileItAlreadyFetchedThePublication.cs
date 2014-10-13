using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.DI;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Configuration;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders.RepositoryObjects;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAModel
{
    public class WhenAccessingAPublicationPropertyWhileItAlreadyFetchedThePublication : TestBase
    {
        private Guid _publicationID;
        private IModel _model;
        private string _actualSsn;
        private string _expectedSsn;
        private IPublicationFactory _publicationFactory;
        private IPublicationService publicationService;
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
            publicationService = A.Fake<IPublicationService>();

            A.CallTo(() => publicationService.GetPublication(_publicationID, _context))
                .Returns(
                    PublicationBuilder.Initialize()
                        .WithGeneration(GenerationBuilder.Initialize()
                            .WithSsn(_expectedSsn)
                            .Build()
                        )
                        .Build());

            _publicationFactory = PublicationFactoryBuilder.Initialize()
                .WithPublicationService(publicationService)
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

            var modelService = A.Fake<IModelService>();
            A.CallTo(() => modelService.GetModels(A<Context>._)).Returns(new List<Repository.Objects.Model> { repoModel });

            var configurationManager = new ConfigurationManagerBuilder().Build();

            var serviceFacade = new S3ServiceFacade(configurationManager)
                .WithModelService(modelService);

            return new ModelFactoryFacade()
                .WithServiceFacade(serviceFacade)
                .WithPublicationFactory(_publicationFactory)
                .Create();
        }

        protected override void Act()
        {
            _actualSsn = _model.SSN; // call publication property a second time
        }

        [Fact]
        public void ThenItShouldNotFetchThePublicationAgain()
        {
            A.CallTo(() => publicationService.GetPublication(_publicationID, _context)).MustHaveHappened(Repeated.Exactly.Once); // only fetch the publication once, no matter how many times the properties are being called
        }

        [Fact]
        public void ThenItShouldStillGetTheCorrectValue()
        {
            _actualSsn.Should().Be(_expectedSsn);
        }
    }

    internal class ConfigurationManagerBuilder
    {
        private readonly IConfigurationManager _configurationManager;

        public ConfigurationManagerBuilder()
        {
            _configurationManager = A.Fake<IConfigurationManager>();

            A.CallTo(() => _configurationManager.DataSubset).Returns("preview");
            A.CallTo(() => _configurationManager.Environment).Returns("development");
            A.CallTo(() => _configurationManager.AmazonAccessKeyId).Returns("access key id");
            A.CallTo(() => _configurationManager.AmazonSecretAccessKey).Returns("secret access key");
        }

        public IConfigurationManager Build()
        {
            return _configurationManager;
        }
    }
}