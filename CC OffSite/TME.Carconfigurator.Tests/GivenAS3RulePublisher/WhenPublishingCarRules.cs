using System;
using System.Collections.Generic;
using FakeItEasy;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects.Rules;
using TME.CarConfigurator.S3.CommandServices;
using TME.CarConfigurator.S3.Publisher;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.Carconfigurator.Tests.Builders;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAS3RulePublisher
{
    public class WhenPublishingCarRules : TestBase
    {
        IRulePublisher _publisher;
        IContext _context;
        IRuleService _ruleService;
        IService _s3Service;
        const string CarRulesKey = "Car rules Key";
        const string SerialisedData = "Serialised Data";
        const string Language = "de";

        protected override void Arrange()
        {
            var publication = new PublicationBuilder().WithID(Guid.NewGuid()).Build();

            var carRules = new RuleSets
            {
                Exclude = new RuleSetBuilder().Build(),
                Include = new RuleSetBuilder().Build()
            };

            var carItemID = Guid.NewGuid();

            var carID = Guid.NewGuid();

            _context = new ContextBuilder()
                .WithLanguages(Language)
                .WithPublication(Language, publication)
                .WithCarRules(Language, carItemID, carID, carRules)
                .Build();

            _s3Service = A.Fake<IService>();
            var serialiser = A.Fake<ISerialiser>();
            A.CallTo(
                () =>
                    serialiser.Serialise(
                        A<IDictionary<Guid, RuleSets>>.That.IsSameSequenceAs(_context.ContextData[Language].CarRules[carID]))).Returns(SerialisedData);
            
            var keymanager = A.Fake<IKeyManager>();
            A.CallTo(() => keymanager.GetCarRulesKey(publication.ID, carID)).Returns(CarRulesKey);

            _ruleService = new RuleService(_s3Service, serialiser, keymanager);
            _publisher = new RulePublisher(_ruleService);
        }

        protected override void Act()
        {
            _publisher.PublishCarRulesAsync(_context).Wait();
        }

        [Fact]
        public void ThenItShouldPublishTheCarRules()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(A<String>._, A<String>._, CarRulesKey, SerialisedData)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}