using TME.CarConfigurator.Repository.Objects.Enums;

namespace TME.CarConfigurator.Repository.Objects.Rules
{
    public abstract class Rule
    {
        public int ShortID { get; set; }
        public string Name { get; set; }
        public RuleCategory Category { get; set; }
    }
}
