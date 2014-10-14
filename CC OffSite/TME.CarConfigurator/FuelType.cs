using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator
{
    public class FuelType : IFuelType
    {
        private readonly Repository.Objects.FuelType _fuelType;
        private IEnumerable<Label> _labels;

        public FuelType(Repository.Objects.FuelType fuelType)
        {
            if (fuelType == null) throw new ArgumentNullException("fuelType");
            _fuelType = fuelType;
        }
        public Guid ID { get { return _fuelType.ID; } }
        public string InternalCode { get { return _fuelType.InternalCode; } }
        public string LocalCode { get { return _fuelType.LocalCode; } }
        public string Name { get { return _fuelType.Name; } }
        public string Description { get { return _fuelType.Description; } }
        public string FootNote { get { return _fuelType.FootNote; } }
        public string ToolTip { get { return _fuelType.ToolTip; } }
        public int SortIndex { get { return _fuelType.SortIndex; } }
        public IEnumerable<ILabel> Labels { get { return _labels = _labels ?? _fuelType.Labels.Select(label => new Label(label)); } }
        public bool Hybrid { get { return _fuelType.Hybrid; } }
    }
}