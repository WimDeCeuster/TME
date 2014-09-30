using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.FrontEndViewer.Models
{
    public class ModelDTO : IModel
    {
        public Guid ID { get; private set; }
        public string InternalCode { get; private set; }
        public string LocalCode { get; private set; }
        public string Name { get; set; }
        public string Description { get; private set; }
        public string FootNote { get; private set; }
        public string ToolTip { get; private set; }
        public int SortIndex { get; private set; }
        public IEnumerable<ILabel> Labels { get; private set; }
        public string Brand { get; private set; }
        public string SSN { get; private set; }
        public bool Promoted { get; private set; }
        public ICarConfiguratorVersion CarConfiguratorVersion { get; private set; }
        public IEnumerable<ILink> Links { get; private set; }
        public IEnumerable<IAsset> Assets { get; private set; }
        public IEnumerable<IBodyType> BodyTypes { get; private set; }
        public IEnumerable<IEngine> Egnines { get; private set; }
        public IEnumerable<IFuelType> FuelTypes { get; private set; }
        public IEnumerable<ICar> Cars { get; private set; }
    }
}