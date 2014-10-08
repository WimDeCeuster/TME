using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.S3.Shared.Result;

namespace TME.CarConfigurator.S3.Shared.Interfaces
{
    public interface IService : IDisposable
    {
        Task<Result.Result> PutObjectAsync(String brand, String country, String key, String item);
        String GetObject(String brand, String country, String key);
    }
}