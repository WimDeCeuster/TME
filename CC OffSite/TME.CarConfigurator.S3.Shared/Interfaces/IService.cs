using System;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Result;

namespace TME.CarConfigurator.S3.Shared.Interfaces
{
    public interface IService : IDisposable
    {
        Task<Result> PutObjectAsync(String brand, String country, String key, String item);
        String GetObject(String brand, String country, String key);
    }
}