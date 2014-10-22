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
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenASubModel
{
    public class WhenAccessingItsAssetsForTheSecondTime : TestBase
    {
        private ISubModel _subModel;
        private IEnumerable<IAsset> _secondAssets;
        private IAssetService _assetService;
        private Repository.Objects.Assets.Asset _asset1;
        private Repository.Objects.Assets.Asset _asset2;
        private IEnumerable<IAsset> _firstAssets;

        protected override void Arrange()
        {
            _asset1 = new AssetBuilder().WithId(Guid.NewGuid()).Build();
            _asset2 = new AssetBuilder().WithId(Guid.NewGuid()).Build();

            var repositorySubModel = new SubModelBuilder()
                .WithID(Guid.NewGuid())
                .Build();

            var publicationTimeFrame =
                new PublicationTimeFrameBuilder()
                .WithID(Guid.NewGuid())
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .AddTimeFrame(publicationTimeFrame)
                .Build();

            var context = new ContextBuilder().Build();

            var subModelService = A.Fake<ISubModelService>();
            A.CallTo(() => subModelService.GetSubModels(A<Guid>._, A<Guid>._, A<Context>._))
                .Returns(new List<Repository.Objects.SubModel>() { repositorySubModel });

            _assetService = A.Fake<IAssetService>();
            A.CallTo(() => _assetService.GetAssets(publication.ID, repositorySubModel.ID, context))
                .Returns(new List<Repository.Objects.Assets.Asset> { _asset1, _asset2 });

            var assetFactory = new AssetFactoryBuilder()
                .WithAssetService(_assetService)
                .Build();

            var subModelFactory = new SubModelFactoryBuilder()
                .WithAssetFactory(assetFactory)
                .WithSubModelService(subModelService)
                .Build();

            _subModel = subModelFactory.GetSubModels(publication, context).Single();

            _firstAssets = _subModel.Assets;
        }

        protected override void Act()
        {
            _secondAssets = _subModel.Assets;
        }

        [Fact]
        public void ThenItShouldNotRecalculateTheAssets()
        {
            A.CallTo(() => _assetService.GetAssets(A<Guid>._, A<Guid>._, A<Context>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldReferToTheSameListOfAssets()
        {
            _secondAssets.Should().BeSameAs(_firstAssets);
        }
    }   
}