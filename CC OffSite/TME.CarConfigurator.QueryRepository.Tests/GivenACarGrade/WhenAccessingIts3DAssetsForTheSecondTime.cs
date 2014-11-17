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
    public class WhenAccessingIts3DAssetsForTheSecondTime : TestBase
    {
        private IGrade _grade;
        private IReadOnlyList<IAsset> _secondAssets;
        private Asset _asset1;
        private Asset _asset2;
        private IAssetService _assetService;
        private string _mode;
        private string _view;
        private IReadOnlyList<IAsset> _firstAssets;

        protected override void Arrange()
        {
            _mode = "the mode";
            _view = "the view";

            _asset1 = new AssetBuilder().WithId(Guid.NewGuid()).Build();
            _asset2 = new AssetBuilder().WithId(Guid.NewGuid()).Build();

            var repoGrade = new GradeBuilder().WithId(Guid.NewGuid()).AddVisibleIn(_mode, _view).Build();

            var publicationTimeFrame = new PublicationTimeFrameBuilder().WithDateRange(DateTime.MinValue, DateTime.MaxValue).WithID(Guid.NewGuid()).Build();

            var publication = new PublicationBuilder().WithID(Guid.NewGuid()).AddTimeFrame(publicationTimeFrame).Build();

            var carID = Guid.NewGuid();

            var context = new ContextBuilder().Build();

            _assetService = A.Fake<IAssetService>();
            A.CallTo(() => _assetService.GetCarAssets(publication.ID, carID, repoGrade.ID, context, _view, _mode))
                .Returns(new[] { _asset1, _asset2 });

            var assetFactory = new AssetFactoryBuilder().WithAssetService(_assetService).Build();

            var gradeFactory = new GradeFactoryBuilder().WithAssetFactory(assetFactory).Build();

            _grade = gradeFactory.GetCarGrade(repoGrade, carID, publication, context);

            _firstAssets = _grade.VisibleIn.Single(visibleIn => visibleIn.Mode == _mode && visibleIn.View == _view).Assets;
        }

        protected override void Act()
        {
            _secondAssets = _grade.VisibleIn.Single(visibleIn => visibleIn.Mode == _mode && visibleIn.View == _view).Assets;
        }

        [Fact]
        public void ThenItShouldFetchTheAssetsFromTheService()
        {
            A.CallTo(() => _assetService.GetCarAssets(A<Guid>._, A<Guid>._, A<Guid>._, A<Context>._, A<String>._, A<String>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldHaveTheCorrectAssets()
        {
            _secondAssets.Should().BeSameAs(_firstAssets);
            _secondAssets.Should().HaveCount(2);
            _secondAssets.Should().Contain(asset => asset.ID == _asset1.ID);
            _secondAssets.Should().Contain(asset => asset.ID == _asset2.ID);
        }
    }
}