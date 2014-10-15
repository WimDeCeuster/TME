using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAnEngineCategory
{
    public class WhenAccessingItsAssetsForTheSecondTime : TestBase
    {
        IEngineCategory _engineCategory;
        IEnumerable<IAsset> _secondAssets;
        IEnumerable<IAsset> _firstAssets;
        Repository.Objects.Assets.Asset _asset1;
        Repository.Objects.Assets.Asset _asset2;

        protected override void Arrange()
        {
            _asset1 = new AssetBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            _asset2 = new AssetBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var repoEngineCategory = new CarConfigurator.Tests.Shared.TestBuilders.EngineCategoryBuilder()
                .WithAssets(_asset1, _asset2)
                .Build();

            var context = new ContextBuilder().Build();

            var publication = new PublicationBuilder().Build();

            _engineCategory = new TestBuilders.EngineCategoryBuilder()
                .WithEngineCategory(repoEngineCategory)
                .Build();

            _firstAssets = _engineCategory.Assets;
        }

        protected override void Act()
        {
            _secondAssets = _engineCategory.Assets;
        }

        [Fact]
        public void ThenItShouldNotRecalculateTheLabels()
        {
            _secondAssets.Should().BeSameAs(_firstAssets);
        }

        [Fact]
        public void ThenItShouldHaveTheLabels()
        {
            _secondAssets.Count().Should().Be(2);

            _secondAssets.Should().Contain(asset => asset.ID == _asset1.ID);
            _secondAssets.Should().Contain(asset => asset.ID == _asset2.ID);
        }
    }
}
