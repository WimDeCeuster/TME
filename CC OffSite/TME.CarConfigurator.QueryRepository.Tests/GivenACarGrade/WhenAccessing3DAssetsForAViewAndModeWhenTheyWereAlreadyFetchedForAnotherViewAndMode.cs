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

namespace TME.CarConfigurator.Query.Tests.GivenACarGrade
{
    public class WhenAccessing3DAssetsForAViewAndModeWhenTheyWereAlreadyFetchedForAnotherViewAndMode : TestBase
    {
        private IGrade _grade;
        private IReadOnlyList<IAsset> _secondAssets;
        private Asset _asset1;
        private Asset _asset2;
        private IAssetService _assetService;
        private string _mode1;
        private string _mode2;
        private string _view1;
        private string _view2;
        private Asset _asset3;
        private IReadOnlyList<IAsset> _firstAssets;

        protected override void Arrange()
        {
            _mode1 = "the mode";
            _mode2 = "the new mode";
            _view1 = "the view";
            _view2 = "the new view";

            _asset1 = new AssetBuilder().WithId(Guid.NewGuid()).Build();
            _asset2 = new AssetBuilder().WithId(Guid.NewGuid()).Build();
            _asset3 = new AssetBuilder().WithId(Guid.NewGuid()).Build();

            var repoGrade = new GradeBuilder().WithId(Guid.NewGuid()).AddVisibleIn(_mode1, _view1).AddVisibleIn(_mode2,_view2).Build();

            var publicationTimeFrame = new PublicationTimeFrameBuilder().WithDateRange(DateTime.MinValue, DateTime.MaxValue).WithID(Guid.NewGuid()).Build();

            var publication = new PublicationBuilder().WithID(Guid.NewGuid()).AddTimeFrame(publicationTimeFrame).Build();

            var carID = Guid.NewGuid();

            var context = new ContextBuilder().Build();

            _assetService = A.Fake<IAssetService>();
            A.CallTo(() => _assetService.GetCarAssets(publication.ID, carID, repoGrade.ID, context, _view1, _mode1))
                .Returns(new[] { _asset1, _asset2 });
            A.CallTo(() => _assetService.GetCarAssets(publication.ID, carID, repoGrade.ID, context, _view2, _mode2))
                .Returns(new[] { _asset3});

            var assetFactory = new AssetFactoryBuilder().WithAssetService(_assetService).Build();

            var gradeFactory = new GradeFactoryBuilder().WithAssetFactory(assetFactory).Build();

            _grade = gradeFactory.GetCarGrades(repoGrade, carID, publication, context);

            _firstAssets =
                _grade.VisibleIn.Single(visibleIn => visibleIn.Mode == _mode1 && visibleIn.View == _view1).Assets;
        }

        protected override void Act()
        {
            _secondAssets = _grade.VisibleIn.Single(visibleIn => visibleIn.Mode == _mode2 && visibleIn.View == _view2).Assets;
        }

        [Fact]
        public void ThenItShouldFetchTheAssetsFromTheServiceForTheFirstModeAndView()
        {
            A.CallTo(() => _assetService.GetCarAssets(A<Guid>._, A<Guid>._, A<Guid>._, A<Context>._, _view1,_mode1))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
        
        [Fact]
        public void ThenItShouldFetchTheAssetsFromTheServiceForTheSecondModeAndView()
        {
            A.CallTo(() => _assetService.GetCarAssets(A<Guid>._, A<Guid>._, A<Guid>._, A<Context>._, _view2, _mode2))
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