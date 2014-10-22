using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Repository.Objects
{
    public class SubModel : BaseObject
    {
        public Price StartingPrice { get; set; }
        public List<Link> Links { get; set; }
        public List<Asset> Assets { set; get; }

        public SubModel()
        {
            Links = new List<Link>();
            Assets = new List<Asset>();
        }
    }
}