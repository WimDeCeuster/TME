using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.Shared
{
    public class Serialiser : ISerialiser
    {
        public String Serialise(Object obj)
        {
            return JsonConvert.SerializeObject(obj,Formatting.Indented);
        }

        public T Deserialise<T>(String value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}
