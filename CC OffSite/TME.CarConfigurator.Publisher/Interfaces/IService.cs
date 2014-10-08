using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Enums.Result;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IS3Service : IDisposable
    {
        Task<Result> PutObjectAsync(String key, String item);
        String GetObject(String key);
    }
}