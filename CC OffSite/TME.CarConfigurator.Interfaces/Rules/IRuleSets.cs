namespace TME.CarConfigurator.Interfaces.Rules
{
    public interface IRuleSets
    {
        IRuleSet Include { get; }
        IRuleSet Exclude { get; }
    }
}
