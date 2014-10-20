using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.S3.Shared.Interfaces
{
    public interface ISerialiser
    {
        String Serialise(Object obj);
        T Deserialise<T>(String value);
    }
}