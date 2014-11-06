using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.S3.Publisher;
using TME.Carconfigurator.Tests.Builders;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit.Extensions;

namespace TME.Carconfigurator.Tests.GivenAS3AssetPublisher
{
    public class WhenPublishingCarAssets
    {
        [Theory,
            InlineData("ext", "day"),
            InlineData("int", "night")]

        public async Task ItShouldGroupTheAssetsCorrectly(string view, string mode)
        {
            // arrange
            var assetService = A.Fake<IAssetService>();
            var assetPublisher = new AssetPublisher(assetService);

            var carId = Guid.NewGuid();
            var objectId = Guid.NewGuid();

            var assetType = new AssetTypeBuilder().WithMode(mode).WithView(view).Build();
            var asset1 = new AssetBuilder().WithId(Guid.NewGuid()).WithAssetType(assetType).Build();
            var asset2 = new AssetBuilder().WithId(Guid.NewGuid()).WithAssetType(assetType).Build();

            var incorrectAssetType = new AssetTypeBuilder().WithMode("incorrect").WithView("incorrect").Build();
            var incorrectAsset = new AssetBuilder().WithId(Guid.NewGuid()).WithAssetType(incorrectAssetType).Build();

            var defaultAssetType = new AssetTypeBuilder().Build();
            var defaultAsset = new AssetBuilder().WithId(Guid.NewGuid()).WithAssetType(defaultAssetType).Build();

            const string language = "NL";

            var context = new ContextBuilder()
                .WithLanguages(language)
                .WithPublication(language, PublicationBuilder.Initialize().Build())
                .AddCarAsset(language, carId, objectId, asset1)
                .AddCarAsset(language, carId, objectId, incorrectAsset)
                .AddCarAsset(language, carId, objectId, defaultAsset)
                .AddCarAsset(language, carId, objectId, asset2)
                .Build();

            // act
            await assetPublisher.PublishAsync(context);

            // assert
            A.CallTo(() => assetService.PutCarAssetsByModeAndView(A<string>._, A<string>._, A<Guid>._, A<Guid>._, A<Guid>._, A<string>._, A<string>._, A<IEnumerable<Asset>>._))
                .WhenArgumentsMatch(args =>
                {
                    var passedMode = (string)args[5];
                    var passedView = (string)args[6];
                    var passedAssets = ((IEnumerable<Asset>)args[7]).ToList();

                    return passedMode == mode
                        && passedView == view
                        && passedAssets.Count == 2
                        && passedAssets.Contains(asset1)
                        && passedAssets.Contains(asset2)
                        && !passedAssets.Contains(incorrectAsset);
                })
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => assetService.PutCarAssetsByModeAndView(A<string>._, A<string>._, A<Guid>._, A<Guid>._, A<Guid>._, incorrectAssetType.Mode, incorrectAssetType.View, A<IEnumerable<Asset>>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => assetService.PutDefaultCarAssets(A<string>._, A<string>._, A<Guid>._, A<Guid>._, A<Guid>._, A<IEnumerable<Asset>>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}