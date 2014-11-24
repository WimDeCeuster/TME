using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.Interfaces.TechnicalSpecifications;

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
        ISubModel SubModel { get; }
        IReadOnlyList<ICarPart> Parts { get; }

        ICarEquipment Equipment { get; }
        IReadOnlyList<ICarPack> Packs { get; }
        IReadOnlyList<ICarTechnicalSpecification> TechnicalSpecifications { get; }
        IReadOnlyList<IColourCombination> ColourCombinations { get; }
    }
}

