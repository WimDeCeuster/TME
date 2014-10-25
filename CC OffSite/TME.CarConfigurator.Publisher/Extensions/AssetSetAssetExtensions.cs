using System.Collections.Generic;
using System.Linq;
using System.Text;
using TME.CarConfigurator.Administration.Assets;
using TME.CarConfigurator.Publisher.Exceptions;

namespace TME.CarConfigurator.Publisher.Extensions
{
    public static class AssetSetAssetExtensions
    {
        public static IEnumerable<AssetSetAsset> GetGenerationAssets(this IEnumerable<AssetSetAsset> assets)
        {
            return assets.Where(asset => !asset.IsDeviation() && asset.AlwaysInclude);
        }

        public static IEnumerable<AssetSetAsset> Filter(this IList<AssetSetAsset> assets, Administration.Car car)
        {
            var carAssets = new List<AssetSetAsset>();
            var possibleCarAssets = new Dictionary<string, IList<AssetSetAsset>>();

            foreach (var asset in assets)
            {
                if (!IsAssetOfCar(car, asset) || HasGradeException(car, asset, assets)) continue;

                if (asset.AlwaysInclude)
                {
                    carAssets.Add(asset);
                    continue;
                }

                TryAddAssetToPossibleCarAssets(asset, possibleCarAssets);
            }

            AddPossibleCarAssetsThatAreBetterMatches(carAssets, possibleCarAssets);

            return carAssets;
        }

        private static bool IsAssetOfCar(Administration.Car car, AssetSetAsset asset)
        {
            return asset.IsDeviation()
                   && (asset.BodyType.IsEmpty() || asset.BodyType.Equals(car.BodyType))
                   && (asset.Engine.IsEmpty() || asset.Engine.Equals(car.Engine))
                   && (asset.Transmission.IsEmpty() || asset.Transmission.Equals(car.Transmission))
                   && (asset.WheelDrive.IsEmpty() || asset.WheelDrive.Equals(car.WheelDrive))
                   && (asset.Steering.IsEmpty() || asset.Steering.Equals(car.Steering))
                   && (asset.Grade.IsEmpty() || asset.Grade.Equals(car.Grade));
        }

        /// <summary>
        /// Try to find another asset that fits equally well, but that also fits on the grade of the car
        /// </summary>
        /// <param name="car">The car on which the asset should match</param>
        /// <param name="asset">The current asset</param>
        /// <param name="assets">The list of assets in which to look for a better fitting asset</param>
        /// <returns></returns>
        private static bool HasGradeException(Administration.Car car, AssetSetAsset asset, IEnumerable<AssetSetAsset> assets)
        {
            if (asset.Grade.ID == car.GradeID) return false; // There is no asset that is the same with a better grade than the car, because this asset already has the same grade. There can (potentially/theoretically) be other assets with the exact same deviation properties, but those shouldn't stop this one from being added. 

            return assets.Any(otherAsset => IsAssetOfCar(car, otherAsset)
                                            && otherAsset.AssetType.Equals(asset.AssetType)
                                            && (asset.BodyType.IsEmpty() || otherAsset.BodyType.Equals(asset.BodyType)) // Only check if bodytype is the same if the current asset has a bodytype, otherwise it can be ignored
                                            && otherAsset.Engine.Equals(asset.Engine)
                                            && otherAsset.Transmission.Equals(asset.Transmission)
                                            && otherAsset.WheelDrive.Equals(asset.WheelDrive)
                                            && otherAsset.Steering.Equals(asset.Steering)
                                            && otherAsset.ExteriorColour.Equals(asset.ExteriorColour)
                                            && otherAsset.Upholstery.Equals(asset.Upholstery)
                                            && otherAsset.EquipmentItem.Equals(asset.EquipmentItem)
                                            && otherAsset.Grade.Equals(car.Grade)); // if it has all other properties the same, and in addition has the same grade as the car, it is the asset we are looking for
        }

        private static void TryAddAssetToPossibleCarAssets(AssetSetAsset asset, IDictionary<string, IList<AssetSetAsset>> possibleCarAssets)
        {
            var key = GetKey(asset);

            if (!possibleCarAssets.ContainsKey(key))
            {
                possibleCarAssets.Add(key, new List<AssetSetAsset> { asset });
                return;
            }

            var existingAssets = possibleCarAssets[key];

            var existingAsset = existingAssets.First(); // all assets for this key the same number of deviation axes

            var numberOfDeviationAxesOfExistingAsset = GetNumberOfDeviationAxes(existingAsset);
            var numberOfDeviationAxesOfCurrentAsset = GetNumberOfDeviationAxes(asset);

            if (numberOfDeviationAxesOfCurrentAsset < numberOfDeviationAxesOfExistingAsset)
                return;

            if (numberOfDeviationAxesOfCurrentAsset == numberOfDeviationAxesOfExistingAsset)
            {
                existingAssets.Add(asset);

                return;
            }

            // more deviation axes than existing assets => only keep the current asset
            possibleCarAssets[key] = new List<AssetSetAsset> { asset };
        }

        private static string GetKey(AssetSetAsset asset)
        {
            var assetType = asset.AssetType;
            var splitAssetTypeName = assetType.Name.Split('_');

            var mode = string.Empty;
            var view = string.Empty;
            var side = string.Empty;
            var type = string.Empty;

            FillModeViewSideAndType(assetType, splitAssetTypeName, ref mode, ref view, ref side, ref type);

            if (string.IsNullOrEmpty(view))
                return assetType.Code;

            var exteriourColourCode = asset.ExteriorColour.Code;
            var upholsteryCode = asset.Upholstery.Code;
            var equipmentCode = asset.EquipmentItem.ID.ToString(); 

            return GetKey(mode, view, side, type, exteriourColourCode, upholsteryCode, equipmentCode);
        }

        private static void FillModeViewSideAndType(AssetType assetType, IList<string> splitAssetTypeName, ref string mode, ref string view, ref string side, ref string type)
        {
            var sections = splitAssetTypeName.Count;

            if (!string.IsNullOrEmpty(assetType.Details.Mode))
            {
                const int leastAmountOfDefinedSections = 3;

                if (sections < leastAmountOfDefinedSections)
                    throw new CorruptDataException(string.Format("At least {0} parts should be definied in the type {1} for the {2} mode", leastAmountOfDefinedSections, assetType.Name, assetType.Details.Mode));

                if (sections == leastAmountOfDefinedSections) return; // even if there are only leastAmountOfDefinedSections, it is still not enough, but there should not be an exception (code copied from old reader library, where it is the same)

                mode = splitAssetTypeName[0];
                view = splitAssetTypeName[1];
                side = splitAssetTypeName[2];
                type = splitAssetTypeName[3];

                return;
            }

            if (sections <= 2) return;

            view = splitAssetTypeName[0];
            side = splitAssetTypeName[1];
            type = splitAssetTypeName[2];
        }

        private static string GetKey(string mode, string view, string side, string type, string exteriourColourCode, string upholsteryCode, string equipmentCode)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(mode);

            if (string.IsNullOrEmpty(view))
                return stringBuilder.ToString();

            if (stringBuilder.Length != 0)
                stringBuilder.Append("_"); // if a mode was appended before, we need the underscore

            stringBuilder.Append(view);

            if (string.IsNullOrEmpty(side))
                return stringBuilder.ToString();

            stringBuilder.Append(string.Format("_{0}", side));

            if (string.IsNullOrEmpty(type))
                return stringBuilder.ToString();

            stringBuilder.Append(string.Format("_{0}", type));

            if (string.IsNullOrEmpty(exteriourColourCode))
                return stringBuilder.ToString();

            stringBuilder.Append(string.Format("_{0}", exteriourColourCode));

            if (string.IsNullOrEmpty(upholsteryCode))
                return stringBuilder.ToString();

            stringBuilder.Append(string.Format("_{0}", upholsteryCode));

            if (string.IsNullOrEmpty(equipmentCode))
                return stringBuilder.ToString();

            stringBuilder.Append(string.Format("_{0}", equipmentCode));

            return stringBuilder.ToString();
        }

        private static int GetNumberOfDeviationAxes(AssetSetAsset asset)
        {
            var i = 0;

            i += asset.BodyType.IsEmpty() ? 0 : 1;
            i += asset.Engine.IsEmpty() ? 0 : 1;
            i += asset.Grade.IsEmpty() ? 0 : 1;
            i += asset.Transmission.IsEmpty() ? 0 : 1;
            i += asset.WheelDrive.IsEmpty() ? 0 : 1;
            i += asset.Steering.IsEmpty() ? 0 : 1;

            return i;
        }

        private static void AddPossibleCarAssetsThatAreBetterMatches(List<AssetSetAsset> carAssets, Dictionary<string, IList<AssetSetAsset>> possibleCarAssets)
        {
            RemovePossibleCarAssetsWhenThereIsABetterAssetInTheCarAssetsList(carAssets, possibleCarAssets);

            var remainingPossibleCarAssets = possibleCarAssets.Values.SelectMany(v => v);

            carAssets.AddRange(remainingPossibleCarAssets);
        }

        private static void RemovePossibleCarAssetsWhenThereIsABetterAssetInTheCarAssetsList(IEnumerable<AssetSetAsset> carAssets, Dictionary<string, IList<AssetSetAsset>> possibleCarAssets)
        {
            foreach (var carAsset in carAssets)
            {
                var key = GetKey(carAsset);

                if (!possibleCarAssets.ContainsKey(key))
                    continue;

                var existingAsset = possibleCarAssets[key].First(); // all assets for this key have the same number of deviation axes

                var numberOfDeviationAxesOfPossibleAsset = GetNumberOfDeviationAxes(existingAsset);
                var numberOfDeviationAxesOfCarAsset = GetNumberOfDeviationAxes(carAsset);

                if (numberOfDeviationAxesOfPossibleAsset >= numberOfDeviationAxesOfCarAsset) // optional assets are more specific than the car asset, so we can keep them
                    continue;

                possibleCarAssets.Remove(key);
            }
        }
    }
}