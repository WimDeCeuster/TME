using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.S3.Shared.Interfaces
{
    public interface ISerialiser
    {
        String Serialise(Publication publication);
        String Serialise(Languages languages);
        T Deserialise<T>(String value);
        String Serialise(IEnumerable<BodyType> bodyType);
        String Serialise(IEnumerable<Engine> engines);
        String Serialise(IEnumerable<Asset> assets);
    }
}