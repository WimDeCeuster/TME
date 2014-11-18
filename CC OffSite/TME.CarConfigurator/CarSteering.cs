using System;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator
{
    public class CarSteering : Steering
    {
        private readonly Guid _carID;

        public CarSteering(Repository.Objects.Steering steering, Guid carID, Publication publication, Context context) 
            : base(steering)
        {
            _carID = carID;
        }
    }
}