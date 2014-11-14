using System;
using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator.Equipment
{
    public class CarOption : CarEquipmentItem<Repository.Objects.Equipment.CarOption>, ICarOption
    {
        private readonly IOptionInfo _parentOptionInfo;

        public CarOption(Repository.Objects.Equipment.CarOption repositoryObject, IOptionInfo parentOptionInfo) 
            : base(repositoryObject)
        {
            _parentOptionInfo = parentOptionInfo;
        }

        public bool TechnologyItem { get { return RepositoryObject.TechnologyItem; } }

        public IOptionInfo ParentOption { get { return _parentOptionInfo; } }

        public bool PostProductionOption { get { throw new NotImplementedException(); } }
        public bool SuffixOption { get { throw new NotImplementedException(); } }
    }
}