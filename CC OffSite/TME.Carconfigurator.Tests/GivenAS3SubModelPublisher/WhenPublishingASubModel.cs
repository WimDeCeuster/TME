using System;
using System.Collections.Generic;
using System.Reflection;
using FakeItEasy;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.CommandServices;
using TME.CarConfigurator.S3.Shared;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.Carconfigurator.Tests.Builders;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAS3SubModelPublisher
{
    public class WhenPublishingASubModel : TestBase
    {
        private ISubModelPublisher _subModelPublisher;
        private IContext _context;
        private IService _s3Service;
        private const String LANGUAGE2 = "nl";
        private const String LANGUAGE1 = "nl";

        protected override void Arrange()
        {
            var submodels = new List<SubModel>()
            {
               new SubModelBuilder().WithID(Guid.NewGuid()).Build(), 
               new SubModelBuilder().WithID(Guid.NewGuid()).Build(), 
               new SubModelBuilder().WithID(Guid.NewGuid()).Build(), 
            };
                
            _context = ContextBuilder
                .InitialiseFakeContext()
                .WithLanguages(LANGUAGE1,LANGUAGE2)
                .WithSubModels(LANGUAGE1,submodels)
                .WithSubModels(LANGUAGE2,submodels)
                .Build();

            _s3Service = A.Fake<IService>();

            var serialiser = A.Fake<ISerialiser>();
            var keyManager = A.Fake<IKeyManager>();

            ISubModelService subModelService = new SubModelService(_s3Service,serialiser,keyManager);

            _subModelPublisher = new SubModelPublisherBuilder().WithService(subModelService).Build();
        }

        protected override void Act()
        {
            var result = _subModelPublisher.PublishGenerationSubModelsAsync(_context).Result;
        }

        [Fact]
        public void ThenGenerationSubModelsShouldBePutForEveryLanguageAndTimeFrames()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(null,null,null,null))
                .WithAnyArguments()
                .MustHaveHappened();
        }
    }
}