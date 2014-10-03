using System.Collections.Generic;
using System.Runtime.Serialization;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Repository.Objects
{
    public class Model : BaseObject
    {
        public string Brand { get; set; }
        public bool Promoted { get; set; }
        public List<PublicationInfo> Publications { get; set; }

        public Model()
        {
            Publications = new List<PublicationInfo>();
        }
    }
}