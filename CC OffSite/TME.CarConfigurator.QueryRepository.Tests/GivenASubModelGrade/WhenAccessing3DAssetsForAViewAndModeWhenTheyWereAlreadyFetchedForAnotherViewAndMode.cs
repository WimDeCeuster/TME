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
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenASubModelGrade
{
    public class WhenAccessing3DAssetsForAViewAndModeWhenTheyWereAlreadyFetchedForAnotherViewAndMode : TestBase
    {
        private IEnumerable<IAsset> _secondAssets;
        private IAssetService _assetService;
        private Repository.Objects.Assets.Asset _asset1;
        private Repository.Objects.Assets.Asset _asset2;
        private IGrade _grade;
        private string _mode1;
        private string _view1;
        private string _view2;
        private string _mode2;
        private Asset _asset3;
        private IEnumerable<IAsset> _firstAssets;

        protected override void Arrange()
        {
            _view1 = "the view";
            _view2 = "the new view";
            _mode1 = "the mode";
            _mode2 = "the new mode";

            _asset1 = new AssetBuilder().WithId(Guid.NewGuid()).Build();
            _asset2 = new AssetBuilder().WithId(Guid.NewGuid()).Build();
            _asset3 = new AssetBuilder().WithId(Guid.NewGuid()).Build();

            var repositoryGrade = new GradeBuilder()
                .WithId(Guid.NewGuid())
                .AddVisibleIn(_mode1, _view1)
                .AddVisibleIn(_mode2,_view2)
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

            _assetService = A.Fake<IAssetService>();
            A.CallTo(() => _assetService.GetSubModelAssets(publication.ID, repositorySubModel.ID, repositoryGrade.ID, context, _view1, _mode1))
                .Returns(new List<Asset> { _asset1, _asset2 });
            A.CallTo(() => _assetService.GetSubModelAssets(publication.ID, repositorySubModel.ID, repositoryGrade.ID, context, _view2, _mode2))
                .Returns(new List<Asset> { _asset3 });

            var gradeService = A.Fake<IGradeService>();
            A.CallTo(() => gradeService.GetSubModelGrades(publication.ID, publicationTimeFrame.ID, repositorySubModel.ID, context))
                .Returns(new List<Repository.Objects.Grade> { repositoryGrade });

            var assetFactory = new AssetFactoryBuilder()
                .WithAssetService(_assetService)
                .Build();

            var gradeFactory = new GradeFactoryBuilder()
                .WithAssetFactory(assetFactory)
                .WithGradeService(gradeService)
                .Build();

            _grade = gradeFactory.GetSubModelGrades(repositorySubModel.ID, publication, context).Single();

            _firstAssets = _grade.VisibleIn.Single(v => v.Mode == _mode1 && v.View == _view1).Assets;
        }

        protected override void Act()
        {
            _secondAssets = _grade.VisibleIn.Single(v => v.Mode == _mode2 && v.View == _view2).Assets;
        }

        [Fact]
        public void ThenItShouldFetchTheAssetsFromTheServiceForTheFirstModeAndView()
        {
            A.CallTo(() => _assetService.GetSubModelAssets(A<Guid>._, A<Guid>._, A<Guid>._, A<Context>._, _view1, _mode1))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldFetchTheAssetsFromTheServiceForTheSecondModeAndView()
        {
            A.CallTo(() => _assetService.GetSubModelAssets(A<Guid>._, A<Guid>._, A<Guid>._, A<Context>._, _view2, _mode2))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldHaveTheCorrectListOfAssetsForTheFirstModeAndView()
        {
            _firstAssets.Should().HaveCount(2);

            _firstAssets.Should().Contain(asset => asset.ID == _asset1.ID);
            _firstAssets.Should().Contain(asset => asset.ID == _asset2.ID);
        }

        [Fact]
        public void ThenItShouldHaveTheCorrectListOfAssetsForTheSecondModeAndView()
        {
            _secondAssets.Should().HaveCount(1);

            _secondAssets.Should().Contain(a => a.ID == _asset3.ID);
        }
    }
}