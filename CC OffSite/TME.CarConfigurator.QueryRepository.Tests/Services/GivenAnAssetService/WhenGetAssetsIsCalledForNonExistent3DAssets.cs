using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Assets;
using TME.CarConfigurator.DI;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;
using TME.CarConfigurator.S3.Shared.Exceptions;

namespace TME.CarConfigurator.Query.Tests.Services.GivenAnAssetService
{
    public class WhenGetAssetsIsCalledForNonExistent3DAssets : TestBase
    {
        private Context _context;
        private IEnumerable<Repository.Objects.Assets.Asset> _assets;
        private IAssetService _assetService;
        private ISerialiser _serialiser;
        private string _view;
        private string _mode;

        protected override void Arrange()
        {
            _view = "a view";
            _mode = "a mode";

            _context = new ContextBuilder().Build();

            const string s3Key = "fake s3 key";

            _serialiser = A.Fake<ISerialiser>();
            var service = A.Fake<IService>();
            var keyManager = A.Fake<IKeyManager>();

            A.CallTo(() => keyManager.GetAssetsKey(A<Guid>._, A<Guid>._, _view, _mode)).Returns(s3Key);
            A.CallTo(() => service.GetObject(_context.Brand, _context.Country, s3Key)).Throws(new ObjectNotFoundException(null, s3Key));

            var serviceFacade = new S3ServiceFacade()
                .WithService(service)
                .WithSerializer(_serialiser)
                .WithKeyManager(keyManager);

            _assetService = serviceFacade.CreateAssetService();
        }

        protected override void Act()
        {
            _assets = _assetService.GetAssets(Guid.NewGuid(), Guid.NewGuid(), _context, _view, _mode);
        }

        [Fact]
        public void ThenItShouldReturnTheCorrectListOfAssets()
        {
            _assets.Should().BeEmpty();
        }

        [Fact]
        public void ThenDeserialiseShouldNotHappen()
        {
            A.CallTo(() => _serialiser.Deserialise<IEnumerable<Asset>>(A<String>._)).MustNotHaveHappened();
        }
    }
}