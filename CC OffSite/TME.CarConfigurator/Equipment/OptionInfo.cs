using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator.Equipment
{
    public class OptionInfo : IOptionInfo
    {
        public OptionInfo(int shortId, string name)
        {
            ShortID = shortId;
            Name = name;
        }

        public OptionInfo(Repository.Objects.Equipment.OptionInfo optionInfo)
        {
            ShortID = optionInfo.ShortID;
            Name = optionInfo.Name;
        }

        public int ShortID { get; private set; }

        public string Name { get; private set; }
    }
}
