using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Core;
using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.CarConfigurator.Repository.Objects
{
    public class SubModel : BaseObject
    {
        public Price StartingPrice { get; set; }
        
        public List<Link> Links { get; set; }
        public List<Asset> Assets { get; set; }
        public List<GradeEquipmentItem> Equipment { get; set; }
        public List<Grade> Grades { get; set; } 
    }
}