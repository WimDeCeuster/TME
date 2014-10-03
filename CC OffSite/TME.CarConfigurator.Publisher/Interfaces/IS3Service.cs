using System;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IS3Service
    {
        void PutObject(String key, String item);
    }
}