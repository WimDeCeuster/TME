namespace TME.CarConfigurator.Repository.Objects.Rules
{
    public class RuleSets
    {
        public RuleSets()
        {
            Include = new RuleSet();
            Exclude = new RuleSet();
        }

        public RuleSet Include { get; set; }
        public RuleSet Exclude { get; set; }
    }
}