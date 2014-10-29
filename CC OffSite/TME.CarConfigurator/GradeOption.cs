using System;
using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator
{
    public class GradeOption : GradeEquipmentItem<Repository.Objects.Equipment.GradeOption>, IGradeOption
    {
        readonly IOptionInfo _parentOptionInfo;

        public GradeOption(Repository.Objects.Equipment.GradeOption repoOption, IOptionInfo parentOptionInfo)
            : base(repoOption)
        {
            _parentOptionInfo = parentOptionInfo;
        }

        public bool TechnologyItem
        {
            get { return RepositoryObject.TechnologyItem; }
        }

        public IOptionInfo ParentOption
        {
            get { return _parentOptionInfo; }
        }
    }
}
