namespace TME.CarConfigurator.Interfaces.Configuration
{
    public interface IConfigurationManager
    {
        string Environment { get; }
        string DataSubset { get; }
    }
}