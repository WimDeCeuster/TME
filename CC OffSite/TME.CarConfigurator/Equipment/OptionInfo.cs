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

        public int ShortID { get; private set; }

        public string Name { get; private set; }
    }
}
