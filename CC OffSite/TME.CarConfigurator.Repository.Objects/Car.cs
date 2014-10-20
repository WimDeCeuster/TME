using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Repository.Objects
{
    
    public class Car : BaseObject
    {
        
        public int ShortID { get; set; }
        
        public bool Promoted { get; set; }

        
        public bool WebVisible { get; set; }
        
        public bool ConfigVisible { get; set; }
        
        public bool FinanceVisible { get; set; }

        
        public Price BasePrice { get; set; }
        
        public Price StartingPrice { get; set; }

        
        public BodyType BodyType { get; set; }
        
        public Engine Engine { get; set; }

        public Transmission Transmission { get; set; }

        public WheelDrive WheelDrive { get; set; }

        public Steering Steering { get; set; }

        public Grade Grade { get; set; }
        
        public SubModel SubModel { get; set; }
    }
}
