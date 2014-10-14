using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class FuelTypeMapper : IFuelTypeMapper
    {
        readonly ILabelMapper _labelMapper;

        public FuelTypeMapper(ILabelMapper labelMapper)
        {
            if (labelMapper == null) throw new ArgumentNullException("labelMapper");

            _labelMapper = labelMapper;
        }

        public FuelType MapFuelType(Administration.FuelType fuelType)
        {
            return new FuelType
            {
                Description = fuelType.Translation.Description,
                FootNote = fuelType.Translation.FootNote,
                Hybrid = fuelType.Code.ToUpper(System.Globalization.CultureInfo.InvariantCulture).StartsWith("H"),
                ID = fuelType.ID,
                InternalCode = fuelType.BaseCode,
                Labels = fuelType.Translation.Labels.Select(_labelMapper.MapLabel).ToList(),
                LocalCode = fuelType.LocalCode.DefaultIfEmpty(fuelType.BaseCode),
                Name = fuelType.Translation.Name.DefaultIfEmpty(fuelType.Name),
                SortIndex = 0,
                ToolTip = fuelType.Translation.ToolTip
            };
        }
    }
}
