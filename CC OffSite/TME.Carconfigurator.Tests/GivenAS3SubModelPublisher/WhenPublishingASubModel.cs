using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.S3.Shared;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.S3.Shared.Result;
using TME.CarConfigurator.Tests.Shared;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAS3SubModelPublisher
{
    public class WhenPublishingASubModel : TestBase
    {
        private ISubModelPublisher _subModelPublisher;
        private IContext _context;
        private IService _s3Service;

        protected override void Arrange()
        {
            _s3Service = A.Fake<IService>();

            var serialiser = A.Fake<ISerialiser>();
            var keyManager = A.Fake<IKeyManager>();

            ISubModelService subModelService = new SubModelService(_s3Service,serialiser,keyManager);

            _subModelPublisher = new SubModelPublisherBuilder().WithService(subModelService).Build();
        }

        protected override void Act()
        {
            var result = _subModelPublisher.PublishGenerationSubModels(_context).Result;
        }

        [Fact]
        public void ThenGenerationSubModelsShouldBePutForEveryLanguageAndTimeFrames()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(null,null,null,null))
                .WithAnyArguments()
                .MustHaveHappened();
        }
    }

    public class SubModelService : ISubModelService
    {
        private readonly IService _s3Service;
        private readonly ISerialiser _serialiser;
        private readonly IKeyManager _keyManager;

        public SubModelService(IService s3Service, ISerialiser serialiser, IKeyManager keyManager)
        {
            if (s3Service == null) throw new ArgumentNullException("s3Service");
            if (serialiser == null) throw new ArgumentNullException("serialiser");
            if (keyManager == null) throw new ArgumentNullException("keyManager");

            _s3Service = s3Service;
            _serialiser = serialiser;
            _keyManager = keyManager;
        }
    }

    public class SubModelPublisherBuilder
    {
        private ISubModelService _subModelService = A.Fake<ISubModelService>();

        public SubModelPublisherBuilder WithService(ISubModelService subModelService)
        {
            _subModelService = subModelService;
            return this;
        }

        public ISubModelPublisher Build()
        {
            return new SubModelPublisher(_subModelService);
        }
    }

    public class SubModelPublisher : ISubModelPublisher
    {
        private readonly ISubModelService _subModelService;

        public SubModelPublisher(ISubModelService subModelService)
        {
            if (subModelService == null) throw new ArgumentNullException("subModelService");

            _subModelService = subModelService;
        }

        public Task<IEnumerable<Result>> PublishGenerationSubModels(IContext context)
        {
            throw new System.NotImplementedException();
        }
    }

    public interface ISubModelService
    {
    }

    public interface ISubModelPublisher
    {
        Task<IEnumerable<Result>> PublishGenerationSubModels(IContext context);
    }
}