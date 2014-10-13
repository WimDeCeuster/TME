using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator
{
    public class BodyType : IBodyType
    {
        private readonly Repository.Objects.BodyType _bodyType;

        public BodyType(Repository.Objects.BodyType bodyType)
        {
            if (bodyType == null) throw new ArgumentNullException("bodyType");
            _bodyType = bodyType;
        }

        public Guid ID { get { return _bodyType.ID; } }
        public string InternalCode { get { return _bodyType.InternalCode; } }
        public string LocalCode { get { return _bodyType.LocalCode; } }
        public string Name { get { return _bodyType.Name; } }
        public string Description { get { return _bodyType.Description; } }
        public string FootNote { get { return _bodyType.FootNote; } }
        public string ToolTip { get { return _bodyType.ToolTip; } }
        public int SortIndex { get { return _bodyType.SortIndex; } }
        public IEnumerable<ILabel> Labels { get { throw new NotImplementedException(); } }
        public int NumberOfDoors { get { return _bodyType.NumberOfDoors; } }
        public int NumberOfSeats { get { return _bodyType.NumberOfSeats; } }
        public bool VisibleInExteriorSpin { get { return _bodyType.VisibleInExteriorSpin; } }
        public bool VisibleInInteriorSpin { get { return _bodyType.VisibleInInteriorSpin; } }
        public bool VisibleInXRay4X4Spin { get { return _bodyType.VisibleInXRay4X4Spin; } }
        public bool VisibleInXRayHybridSpin { get { return _bodyType.VisibleInXRayHybridSpin; } }
        public bool VisibleInXRaySafetySpin { get { return _bodyType.VisibleInXRaySafetySpin; } }
        public IEnumerable<IAsset> Assets { get { throw new NotImplementedException(); } }
    }
}