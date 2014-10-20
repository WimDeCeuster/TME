namespace TME.CarConfigurator.Repository.Objects.Equipment
{
    public class Option : EquipmentItem
    {
        public bool TechnologyItem { get; set; }
        public int ParentOptionShortID { get; set; }
    }
}