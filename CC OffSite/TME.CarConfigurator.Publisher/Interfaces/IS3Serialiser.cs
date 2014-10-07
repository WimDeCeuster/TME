using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IS3Serialiser
    {
        String Serialise(Publication publication);
        String Serialise(Languages languages);
        T Deserialise<T>(String value);
        String Serialise(List<Asset> publication);
    }
}