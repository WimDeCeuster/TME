using TME.CarConfigurator.Repository.Objects.Equipment;
namespace TME.CarConfigurator.Repository.Objects.Packs
{
    public class CarPackOption : CarPackEquipmentItem
    {
        public bool TechnologyItem { get; set; }
        public bool PostProductionOption { get; set; }
        public bool SuffixOption { get; set; }
        public OptionInfo ParentOption { get; set; }
    }
}