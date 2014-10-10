using System;
using System.Threading.Tasks;
using TME.CarConfigurator.S3.Shared.Result;

namespace TME.CarConfigurator.CommandServices
{
    public interface IAssetService
    {
        Task<Result> PutGenerationAsset(string brand, string country, Guid id, object o, object value);
    }
}