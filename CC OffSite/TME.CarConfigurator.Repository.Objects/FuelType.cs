using System;
using System.Runtime.Serialization;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Repository.Objects
{
    
    public class FuelType : BaseObject
    {   
        public bool Hybrid { get; set; }
    }
}