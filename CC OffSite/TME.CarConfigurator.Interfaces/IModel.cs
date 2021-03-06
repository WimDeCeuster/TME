﻿using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.TechnicalSpecifications;

namespace TME.CarConfigurator.Interfaces
{
    public interface IModel : IBaseObject
    {
        string Brand { get; }
        string SSN { get; }
        bool Promoted { get; }

        ICarConfiguratorVersion CarConfiguratorVersion { get; }
        IReadOnlyList<ILink> Links { get; }
        IReadOnlyList<IAsset> Assets { get; }
        IReadOnlyList<IBodyType> BodyTypes { get; }
        IReadOnlyList<IEngine> Engines { get; }
        IReadOnlyList<ITransmission> Transmissions { get; }
        IReadOnlyList<IWheelDrive> WheelDrives { get; }
        IReadOnlyList<ISteering> Steerings { get; }
        IReadOnlyList<IGrade> Grades { get; }
        IReadOnlyList<IFuelType> FuelTypes { get; }
        IReadOnlyList<ICar> Cars { get; }
        IReadOnlyList<ISubModel> SubModels { get; }
        IReadOnlyList<IColourCombination> ColourCombinations { get; }
        IModelEquipment Equipment { get; }
        IModelTechnicalSpecifications TechnicalSpecifications { get; }
    }
}
