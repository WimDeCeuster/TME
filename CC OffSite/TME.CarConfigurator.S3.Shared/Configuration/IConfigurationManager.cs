namespace TME.CarConfigurator.S3.Shared.Configuration
{
    public interface IConfigurationManager
    {
        string AmazonAccessKeyId { get; }
        string AmazonSecretAccessKey { get; }
        string BucketNameTemplate { get; }
    }
}