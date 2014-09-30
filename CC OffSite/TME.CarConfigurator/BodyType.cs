using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator
{
    public class BodyType : IBodyType
    {
        public Guid ID { get; private set; }
        public string InternalCode { get; private set; }
        public string LocalCode { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string FootNote { get; private set; }
        public string ToolTip { get; private set; }
        public int SortIndex { get; private set; }
        public IEnumerable<ILabel> Labels { get; private set; }
        public int NumberOfDoors { get; private set; }
        public int NumberOfSeats { get; private set; }
        public bool VisibleInExteriorSpin { get; private set; }
        public bool VisibleInInteriorSpin { get; private set; }
        public bool VisibleInXRay4X4Spin { get; private set; }
        public bool VisibleInXRayHybridSpin { get; private set; }
        public bool VisibleInXRaySafetySpin { get; private set; }
        public IEnumerable<IAsset> Assets { get; private set; }
    }
}