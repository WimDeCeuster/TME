using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using TME.CarConfigurator.Tests.Shared.TestBuilders.RepositoryObjects;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenABodyType
{
    public class WhenAccessing3DAssetsForAViewAndModeWhenTheyWereAlreadyFetchedForAnotherViewAndMode : TestBase
    {
        private string _view1;
        private string _mode1;
        private string _view2;
        private string _mode2;
        private Repository.Objects.Assets.Asset _asset1;
        private Repository.Objects.Assets.Asset _asset2;
        private Repository.Objects.Assets.Asset _asset3;
        private IAssetService _assetService;
        private IBodyType _bodyType;
        private IEnumerable<IAsset> _fetchedAssets1;
        private IEnumerable<IAsset> _fetchedAssets2;

        protected override void Arrange()
        {
            _view1 = "the first view";
            _mode1 = "the first mode";
            _view2 = "the second view";
            _mode2 = "the second mode";

            _asset1 = new AssetBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            _asset2 = new AssetBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            _asset3 = new AssetBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var repoBodyType = new BodyTypeBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var publicationTimeFrame = new PublicationTimeFrameBuilder()
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .AddTimeFrame(publicationTimeFrame)
                .Build();

            var context = new ContextBuilder().Build();

            var bodyTypeService = A.Fake<IBodyTypeService>();
            A.CallTo(() => bodyTypeService.GetBodyTypes(A<Guid>._, A<Guid>._, A<Context>._)).Returns(new List<Repository.Objects.BodyType> { repoBodyType });

            _assetService = A.Fake<IAssetService>();

            A.CallTo(() => _assetService.GetAssets(publication.ID, repoBodyType.ID, context, _view1, _mode1))
                .Returns(new List<Repository.Objects.Assets.Asset> { _asset1, _asset2 });
            A.CallTo(() => _assetService.GetAssets(publication.ID, repoBodyType.ID, context, _view2, _mode2))
                .Returns(new List<Repository.Objects.Assets.Asset> { _asset3 });

            var assetFactory = new AssetFactoryBuilder()
                .WithAssetService(_assetService)
                .Build();

            var bodyTypeFactory = new BodyTypeFactoryBuilder()
                .WithBodyTypeService(bodyTypeService)
                .WithAssetFactory(assetFactory)
                .Build();

            _bodyType = bodyTypeFactory.GetBodyTypes(publication, context).Single();

            _fetchedAssets1 = _bodyType.Get3DAssets(_view1, _mode1);
        }

        protected override void Act()
        {
            _fetchedAssets2 = _bodyType.Get3DAssets(_view2, _mode2);
        }

        [Fact]
        public void ThenItShouldFetchTheAssetsFromTheServiceForTheFirstModeAndView()
        {
            A.CallTo(() => _assetService.GetAssets(A<Guid>._, A<Guid>._, A<Context>._, _view1, _mode1)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldFetchTheAssetsFromTheServiceForTheSecondModeAndView()
        {
            A.CallTo(() => _assetService.GetAssets(A<Guid>._, A<Guid>._, A<Context>._, _view2, _mode2)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldHaveTheCorrectListOfAssetsForTheFirstModeAndView()
        {
            _fetchedAssets1.Should().HaveCount(2);

            _fetchedAssets1.Should().Contain(a => a.ID == _asset1.ID);
            _fetchedAssets1.Should().Contain(a => a.ID == _asset2.ID);
        }

        [Fact]
        public void ThenItShouldHaveTheCorrectListOfAssetsForTheSecondModeAndView()
        {
            _fetchedAssets2.Should().HaveCount(1);

            _fetchedAssets2.Should().Contain(a => a.ID == _asset3.ID);
        }
    }
}