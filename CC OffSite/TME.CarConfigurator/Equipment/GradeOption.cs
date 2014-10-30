using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Equipment
{
    public class GradeOption : GradeEquipmentItem<Repository.Objects.Equipment.GradeOption>, IGradeOption
    {
        readonly IOptionInfo _parentOptionInfo;

        public GradeOption(Repository.Objects.Equipment.GradeOption repoOption, IOptionInfo parentOptionInfo, Publication publication, Context context, IColourFactory colourFactory)
            : base(repoOption, publication, context, colourFactory)
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
