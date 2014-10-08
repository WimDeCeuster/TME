using System;
using System.Runtime.Serialization;

namespace TME.CarConfigurator.Repository.Objects
{
    
    public class EngineType 
    {
        
        public string Code { get; set; }
        
        public string Name { get; set; }

        
        public FuelType FuelType { get; set; }
    }
}