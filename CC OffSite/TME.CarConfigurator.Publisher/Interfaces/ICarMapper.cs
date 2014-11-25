using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface ICarMapper
    {
        Car MapCar(Administration.Car car, BodyType bodyType, Engine engine, Transmission transmission, WheelDrive wheelDrive, Steering steering, bool isPreview);
        CarInfo MapCarInfo(Administration.Car car);
        BodyType CopyBodyType(BodyType bodyType);
        Engine CopyEngine(Engine engine);
        Transmission CopyTransmission(Transmission transmission);
        WheelDrive CopyWheelDrive(WheelDrive wheeldrive);
        Grade CopyGrade(Grade grade);
    }
}
