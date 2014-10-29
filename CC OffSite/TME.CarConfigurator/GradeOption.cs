using System;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Factories;

namespace TME.CarConfigurator
{
    public class GradeOption : GradeEquipmentItem<Repository.Objects.Equipment.GradeOption>, IGradeOption
    {
        readonly IOptionInfo _parentOptionInfo;

        public GradeOption(Repository.Objects.Equipment.GradeOption repoOption, IOptionInfo parentOptionInfo, IColourFactory colourFactory)
            : base(repoOption, colourFactory)
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
