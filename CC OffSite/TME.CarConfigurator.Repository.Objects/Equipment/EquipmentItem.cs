using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects.Colours;
using TME.CarConfigurator.Repository.Objects.Core;
using TME.CarConfigurator.Repository.Objects.Enums;

namespace TME.CarConfigurator.Repository.Objects.Equipment
{
    public abstract class EquipmentItem : BaseObject
    {
        public int ShortID { get; set; }
        public string InternalName { get; set; }
        public string PartNumber { get; set; }
        public string Path { get; set; }

        public bool KeyFeature { get; set; }
        public bool GradeFeature { get; set; }
        public bool OptionalGradeFeature { get; set; }

        public Visibility Visibility { get; set; }

        public CategoryInfo Category { get; set; }
        public ExteriorColour ExteriorColour { get; set; }
        
        public List<Link> Links { get; set; }
    }
}
