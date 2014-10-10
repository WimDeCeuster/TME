namespace TME.CarConfigurator.Interfaces.Configuration
{
    public interface IConfigurationManager
    {
        string Environment { get; }
        string DataSubset { get; }
        string AmazonAccessKeyId { get; }
        string AmazonSecretAccessKey { get; }
    }
}