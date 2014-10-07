using System.Runtime.Serialization;

namespace TME.CarConfigurator.Repository.Objects
{
    
    public class Link
    {
        
        public short ID { get; set; }
        
        public string Name { get; set; }
        
        public string Label { get; set; }
        
        public string Url { get; set; }
    }
}
