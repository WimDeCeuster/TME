using System;

namespace TME.CarConfigurator.S3.Shared.Interfaces
{
    public interface ISerialiser
    {
        String Serialise(Object obj);
        T Deserialise<T>(String value);
    }
}