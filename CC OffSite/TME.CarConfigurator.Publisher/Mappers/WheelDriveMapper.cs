using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class WheelDriveMapper : IWheelDriveMapper
    {
        IBaseMapper _baseMapper;
        IAssetSetMapper _assetSetMapper;

        public WheelDriveMapper(IBaseMapper baseMapper, IAssetSetMapper assetSetMapper)
        {
            if (baseMapper == null) throw new ArgumentNullException("baseMapper");
            if (assetSetMapper == null) throw new ArgumentNullException("assetSetMapper");

            _baseMapper = baseMapper;
            _assetSetMapper = assetSetMapper;
        }

        public WheelDrive MapWheelDrive(Administration.ModelGenerationWheelDrive generationWheelDrive)
        {
            var crossModelWheelDrive = Administration.WheelDrives.GetWheelDrives()[generationWheelDrive.ID];

            var mappedWheelDrive = new WheelDrive
            {
                Brochure = generationWheelDrive.Brochure,
                KeyFeature = generationWheelDrive.KeyFeature,
                VisibleIn = _assetSetMapper.GetVisibility(generationWheelDrive.AssetSet).ToList()
            };

            return _baseMapper.MapDefaultsWithSort(mappedWheelDrive, crossModelWheelDrive, generationWheelDrive, generationWheelDrive.Name);
        }
    }
}
