using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.DI;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.Services.GivenAnAssetService
{
    public class WhenGetSubModelAssetsIsCalledFor3DAssets : TestBase
    {
        private IEnumerable<Repository.Objects.Assets.Asset> _actualAssets;
        private IEnumerable<Repository.Objects.Assets.Asset> _expectedAssets;
        private IAssetService _assetService;
        private Context _context;
        private string _view;
        private string _mode;

        protected override void Arrange()
        {
            _view = "the view";
            _mode = "the mode";

            _context = new ContextBuilder().Build();

            const string s3Key = "fake s3 key";
            const string serializedObject = "this object is serialized";

            _expectedAssets = new List<Repository.Objects.Assets.Asset>
            {
                new AssetBuilder().Build(),
                new AssetBuilder().Build(),
                new AssetBuilder().Build(),
                new AssetBuilder().Build(),
            };

            var serialiser = A.Fake<ISerialiser>();
            var service = A.Fake<IService>();
            var keyManager = A.Fake<IKeyManager>();

            A.CallTo(() => keyManager.GetSubModelAssetsKey(A<Guid>._, A<Guid>._, A<Guid>._, A<String>._, A<String>._)).Returns(s3Key);
            A.CallTo(() => service.GetObject(_context.Brand, _context.Country, s3Key)).Returns(serializedObject);
            A.CallTo(() => serialiser.Deserialise<IEnumerable<Repository.Objects.Assets.Asset>>(serializedObject)).Returns(_expectedAssets);

            var serviceFacade = new S3ServiceFacade()
                .WithService(service)
                .WithSerializer(serialiser)
                .WithKeyManager(keyManager);

            _assetService = serviceFacade.CreateAssetService();
        }

        protected override void Act()
        {
            _actualAssets = _assetService.GetSubModelAssets(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), _context,_view,_mode);
        }

        [Fact]
        public void ThenItShouldReturnTheCorrectListOfAssets()
        {
            _actualAssets.Should().BeSameAs(_expectedAssets);
        }
    }
}