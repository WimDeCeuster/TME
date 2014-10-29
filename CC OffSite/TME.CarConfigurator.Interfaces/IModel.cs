using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator.Interfaces
{
    public interface IModel : IBaseObject
    {
        string Brand { get; }
        string SSN { get; }
        bool Promoted { get; }

        ICarConfiguratorVersion CarConfiguratorVersion { get; }
        IEnumerable<ILink> Links { get; }
        IEnumerable<IAsset> Assets { get; }
        IEnumerable<IBodyType> BodyTypes { get; }
        IEnumerable<IEngine> Engines { get; }
        IEnumerable<ITransmission> Transmissions { get; }
        IEnumerable<IWheelDrive> WheelDrives { get; }
        IEnumerable<ISteering> Steerings { get; }
        IEnumerable<IGrade> Grades { get; }
        IEnumerable<IFuelType> FuelTypes { get; }
        IEnumerable<ICar> Cars { get; }
        IEnumerable<ISubModel> SubModels { get; }

        IEnumerable<IColourCombination> ColourCombinations { get; }

    }
}
