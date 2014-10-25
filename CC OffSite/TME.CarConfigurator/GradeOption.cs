using System;
using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator
{
    public class GradeOption : GradeEquipmentItem, IGradeOption
    {
        public GradeOption(Repository.Objects.Equipment.GradeOption repoOption)
            : base(repoOption)
        {

        }

        public bool TechnologyItem
        {
            get { throw new NotImplementedException(); }
        }

        public IOptionInfo ParentOption
        {
            get { throw new NotImplementedException(); }
        }
    }
}
