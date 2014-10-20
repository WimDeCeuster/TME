using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Repository.Objects
{
    public class Grade : BaseObject
    {
        public string FeatureText { get; set; }

        public string LongDescription { get; set; }

        public bool Special { get; set; }

        public Grade BasedUpon { get; set; }

        public Price StartingPrice { get; set; }

        public IEnumerable<Asset> Assets { get; set; }
    }
}
