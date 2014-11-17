using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface ICarMapper
    {
        Car MapCar(Administration.Car car, BodyType bodyType, Engine engine, Transmission transmission, WheelDrive wheelDrive, Steering steering);
        CarInfo MapCarInfo(Administration.Car car);
    }
}
