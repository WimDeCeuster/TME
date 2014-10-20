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
        ILabelMapper _labelMapper;
        IAssetSetMapper _assetSetMapper;

        public WheelDriveMapper(ILabelMapper labelMapper, IAssetSetMapper assetSetMapper)
        {
            if (labelMapper == null) throw new ArgumentNullException("labelMapper");
            if (assetSetMapper == null) throw new ArgumentNullException("assetSetMapper");

            _labelMapper = labelMapper;
            _assetSetMapper = assetSetMapper;
        }

        public WheelDrive MapWheelDrive(Administration.ModelGenerationWheelDrive generationWheelDrive)
        {
            var crossModelWheelDrive = Administration.WheelDrives.GetWheelDrives()[generationWheelDrive.ID];

            return new WheelDrive
            {
                Brochure = generationWheelDrive.Brochure,
                Description = generationWheelDrive.Translation.Description,
                FootNote = generationWheelDrive.Translation.FootNote,
                ID = generationWheelDrive.ID,
                InternalCode = crossModelWheelDrive.BaseCode,
                KeyFeature = generationWheelDrive.KeyFeature,
                Labels = generationWheelDrive.Translation.Labels.Select(_labelMapper.MapLabel).ToList(),
                LocalCode = crossModelWheelDrive.LocalCode.DefaultIfEmpty(crossModelWheelDrive.BaseCode),
                Name = generationWheelDrive.Translation.Name.DefaultIfEmpty(generationWheelDrive.Name),
                SortIndex = generationWheelDrive.Index,
                ToolTip = generationWheelDrive.Translation.ToolTip,
                VisibleIn = _assetSetMapper.GetVisibility(generationWheelDrive.AssetSet).ToList()
            };
        }
    }
}
