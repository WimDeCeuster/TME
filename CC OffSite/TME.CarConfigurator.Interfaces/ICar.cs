using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator.Interfaces
{
    public interface ICar : IBaseObject
    {
        int ShortID { get; }
        bool Promoted { get; }

        bool WebVisible { get; }
        bool ConfigVisible { get; }
        bool FinanceVisible { get; }
        IPrice BasePrice { get; }
        IPrice StartingPrice { get; }
        
        IBodyType BodyType { get; }
        IEngine Engine { get; }
        ITransmission Transmission { get; }
        IWheelDrive WheelDrive { get; }
        ISteering Steering { get; }
        IGrade Grade { get; }
    }
}
