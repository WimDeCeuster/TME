using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.S3.CommandServices;
using TME.CarConfigurator.S3.Publisher;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.Carconfigurator.Tests.Builders;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit.Extensions;

namespace TME.Carconfigurator.Tests.GivenAS3AssetPublisher
{
    public class WhenPublishing3DCarItemAssets
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
            var asset3 = new AssetBuilder().WithId(Guid.NewGuid()).WithAssetType(assetType).Build();
            var asset4 = new AssetBuilder().WithId(Guid.NewGuid()).WithAssetType(assetType).Build();

            var incorrectAssetType = new AssetTypeBuilder().WithMode("incorrect").WithView("incorrect").Build();
            var incorrectAsset = new AssetBuilder().WithId(Guid.NewGuid()).WithAssetType(incorrectAssetType).Build();

            var defaultAssetType = new AssetTypeBuilder().Build();
            var defaultAsset = new AssetBuilder().WithId(Guid.NewGuid()).WithAssetType(defaultAssetType).Build();

            const string language = "NL";

            var context = new ContextBuilder()
                .WithLanguages(language) 
                .WithPublication(language, new PublicationBuilder().Build())
                .AddCarEquipmentAssets(language, carId, objectId, new[] { asset1, asset2, incorrectAsset, defaultAsset })
                .AddCarPartAssets(language, carId, objectId, new[] { asset3, asset4, incorrectAsset })
                .Build();

            // act
            await assetPublisher.PublishAsync(context);

            // assert
            A.CallTo(() => assetService.PutCarEquipmentAssetsByModeAndView(A<string>._, A<string>._, A<Guid>._, A<Guid>._, A<string>._, A<string>._, A<Dictionary<Guid, IList<Asset>>>._))
                .WhenArgumentsMatch(args =>
                {
                    var passedMode = (string)args[4];
                    var passedView = (string)args[5];
                    var passedAssets = (((Dictionary<Guid, IList<Asset>>)args[6])[objectId]).ToList();

                    return passedMode == mode
                        && passedView == view
                        && passedAssets.Count == 2
                        && passedAssets.Contains(asset1)
                        && passedAssets.Contains(asset2)
                        && !passedAssets.Contains(incorrectAsset);
                })
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => assetService.PutCarPartsAssetsByModeAndView(A<string>._, A<string>._, A<Guid>._, A<Guid>._, A<string>._, A<string>._, A<Dictionary<Guid, IList<Asset>>>._))
                .WhenArgumentsMatch(args =>
                {
                    var passedMode = (string)args[4];
                    var passedView = (string)args[5];
                    var passedAssets = (((Dictionary<Guid, IList<Asset>>)args[6])[objectId]).ToList();

                    return passedMode == mode
                        && passedView == view
                        && passedAssets.Count == 2
                        && passedAssets.Contains(asset3)
                        && passedAssets.Contains(asset4)
                        && !passedAssets.Contains(incorrectAsset);
                })
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => assetService.PutCarEquipmentAssetsByModeAndView(A<string>._, A<string>._, A<Guid>._, A<Guid>._, incorrectAssetType.Mode, incorrectAssetType.View, A<Dictionary<Guid, IList<Asset>>>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => assetService.PutDefaultCarEquipmentAssets(A<string>._, A<string>._, A<Guid>._, A<Guid>._, A<Dictionary<Guid, IList<Asset>>>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}