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

namespace TME.CarConfigurator.Query.Tests.GivenASubModelGrade
{
    public class WhenAccessingItsAssetsForTheFirstTime : TestBase
    {
        private IEnumerable<IAsset> _assets;
        private IAssetService _assetService;
        private Repository.Objects.Assets.Asset _asset1;
        private Repository.Objects.Assets.Asset _asset2;
        private IGrade _grade;

        protected override void Arrange()
        {
            _asset1 = new AssetBuilder().WithId(Guid.NewGuid()).Build();
            _asset2 = new AssetBuilder().WithId(Guid.NewGuid()).Build();

            var repositoryGrade = new GradeBuilder()
                .WithId(Guid.NewGuid())
                .Build();

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
            A.CallTo(() => _assetService.GetSubModelAssets(publication.ID, repositorySubModel.ID,repositoryGrade.ID, context))
                .Returns(new List<Repository.Objects.Assets.Asset> { _asset1, _asset2 });

            var gradeService = A.Fake<IGradeService>();
            A.CallTo(() => gradeService.GetSubModelGrades(publication.ID, publicationTimeFrame.ID, repositorySubModel.ID, context))
                .Returns(new List<Repository.Objects.Grade> {repositoryGrade});

            var assetFactory = new AssetFactoryBuilder()
                .WithAssetService(_assetService)
                .Build();

            var gradeFactory = new GradeFactoryBuilder()
                .WithAssetFactory(assetFactory)
                .WithGradeService(gradeService)
                .Build();

            _grade = gradeFactory.GetSubModelGrades(repositorySubModel.ID,publication, context).Single();
        }

        protected override void Act()
        {
            _assets = _grade.Assets;
        }

        [Fact]
        public void ThenItShouldFetchTheAssetsFromTheService()
        {
            A.CallTo(() => _assetService.GetSubModelAssets(A<Guid>._, A<Guid>._, A<Guid>._, A<Context>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldHaveTheCorrectAssets()
        {
            _assets.Should().HaveCount(2);
            _assets.Should().Contain(asset => asset.ID == _asset1.ID);
            _assets.Should().Contain(asset => asset.ID == _asset2.ID);
        }
    }
}