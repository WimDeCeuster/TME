namespace TME.CarConfigurator.Interfaces.Configuration
{
    public interface IConfigurationManager
    {
        string AmazonAccessKeyId { get; }
        string AmazonSecretAccessKey { get; }
        string BucketNameTemplate { get; }
    }
}