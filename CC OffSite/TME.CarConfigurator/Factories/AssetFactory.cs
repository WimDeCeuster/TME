using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Assets;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Factories
{
    public class AssetFactory : IAssetFactory
    {
        private readonly IAssetService _assetService;
        private readonly Dictionary<String, Dictionary<Guid, List<Repository.Objects.Assets.Asset>>> _carEquipmentAssets = new Dictionary<string, Dictionary<Guid, List<Repository.Objects.Assets.Asset>>>();
        private readonly Dictionary<String, Dictionary<Guid, List<Repository.Objects.Assets.Asset>>>  _carPartAssets = new Dictionary<string, Dictionary<Guid, List<Repository.Objects.Assets.Asset>>>();
        private decimal _counter;
        private Dictionary<Guid, List<Repository.Objects.Assets.Asset>> _defaultCarEquipmentAssets;

        public AssetFactory(IAssetService assetService)
        {
            if (assetService == null) throw new ArgumentNullException("assetService");

            _assetService = assetService;
        }

        public IReadOnlyList<IAsset> GetAssets(Publication publication, Guid objectId, Context context)
        {
            var repoAssets = _assetService.GetAssets(publication.ID, objectId, context);

            return TransformIntoNonRepoAssets(repoAssets);
        }

        public IReadOnlyList<IAsset> GetAssets(Publication publication, Guid objectId, Context context, string view, string mode)
        {
            var repoAssets = _assetService.GetAssets(publication.ID, objectId, context, view, mode);

            return TransformIntoNonRepoAssets(repoAssets);
        }

        public IReadOnlyList<IAsset> GetCarAssets(Publication publication, Guid carId, Guid objectId, Context context)
        {
            var repoAssets = _assetService.GetCarAssets(publication.ID, carId, objectId, context);

            return TransformIntoNonRepoAssets(repoAssets);
        }

        public IReadOnlyList<IAsset> GetCarAssets(Publication publication, Guid carId, Guid objectId, Context context, string view, string mode)
        {
            var repoAssets = _assetService.GetCarAssets(publication.ID, carId, objectId, context, view, mode);

            return TransformIntoNonRepoAssets(repoAssets);
        }

        public IReadOnlyList<IAsset> GetCarEquipmentAssets(Publication publication, Guid carID, Guid objectID, Context context)
        {
            _defaultCarEquipmentAssets = _defaultCarEquipmentAssets ?? _assetService.GetCarEquipmentAssets(publication.ID, carID, context);

            var filteredRepoAssets = FilterDictionaryItemsPerObjectID(objectID, _defaultCarEquipmentAssets);

            return TransformIntoNonRepoAssets(filteredRepoAssets);
        }

        public IReadOnlyList<IAsset> GetCarEquipmentAssets(Publication publication, Guid carID, Guid objectID, Context context, string view, string mode)
        {
            var key = string.Format("{0}{1}", view, mode);
            if (!_carEquipmentAssets.ContainsKey(key))
                _carEquipmentAssets.Add(key,_assetService.GetCarEquipmentAssets(publication.ID, carID, context,view, mode));

            var filteredRepoAssets = FilterDictionaryItemsPerObjectID(objectID, _carEquipmentAssets[key]);

            return TransformIntoNonRepoAssets(filteredRepoAssets);
        }

        public IReadOnlyList<IAsset> GetCarPartAssets(Publication publication, Guid carID, Guid objectID, Context context, string view,
            string mode)
        {
            var key = string.Format("{0}{1}", view, mode);
            if (!_carPartAssets.ContainsKey(key))
                _carPartAssets.Add(key, _assetService.GetCarPartsAssets(publication.ID, carID, context, view, mode));

            var filteredRepoAssets = FilterDictionaryItemsPerObjectID(objectID, _carPartAssets[key]);

            return TransformIntoNonRepoAssets(filteredRepoAssets);
        }

        private static IEnumerable<Repository.Objects.Assets.Asset> FilterDictionaryItemsPerObjectID(Guid objectID, Dictionary<Guid, List<Repository.Objects.Assets.Asset>> repoAssets)
        {
            return repoAssets.Where(entry => entry.Key == objectID).SelectMany(entry => entry.Value);
        }

        public IReadOnlyList<IAsset> GetSubModelAssets(Publication publication, Guid subModelID, Guid objectID, Context context)
        {
            var repoAssets = _assetService.GetSubModelAssets(publication.ID, subModelID, objectID, context);

            return TransformIntoNonRepoAssets(repoAssets);
        }

        public IReadOnlyList<IAsset> GetSubModelAssets(Publication publication, Guid subModelID, Guid objectID, Context context, string view,
            string mode)
        {
            var repoAssets = _assetService.GetSubModelAssets(publication.ID, subModelID, objectID, context, view, mode);

            return TransformIntoNonRepoAssets(repoAssets);
        }

        private static IReadOnlyList<Asset> TransformIntoNonRepoAssets(IEnumerable<Repository.Objects.Assets.Asset> repoAssets)
        {
            return repoAssets.Select(a => new Asset(a)).ToArray();
        }
    }
}