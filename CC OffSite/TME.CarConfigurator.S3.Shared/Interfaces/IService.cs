using System;
using System.Threading.Tasks;

namespace TME.CarConfigurator.S3.Shared.Interfaces
{
    public interface IService : IDisposable
    {
        Task PutObjectAsync(String brand, String country, String key, String item);
        String GetObject(String brand, String country, String key);
    }
}