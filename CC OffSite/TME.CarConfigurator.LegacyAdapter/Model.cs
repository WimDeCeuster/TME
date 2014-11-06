using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.TechnicalSpecifications;
using TME.CarConfigurator.LegacyAdapter.Equipment;
using TME.CarConfigurator.LegacyAdapter.TechnicalSpecifications;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class Model : BaseObject, IModel
    {

        #region Dependencies (Adaptee)
        private Legacy.Model Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public Model(Legacy.Model adaptee) : base(adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion
        

        public string Brand
        {
            get { return Adaptee.Brand;}
        }

        public string SSN
        {
            get { return Adaptee.Generation.SSN; }
        }

        public bool Promoted
        {
            get
            {
                return Object.Equals(Legacy.Model.GetPromotedModel(Legacy.MyContext.CurrentContext), Adaptee); 
            }
        }

        public ICarConfiguratorVersion CarConfiguratorVersion
        {
            get { return new CarConfiguratorVersion(Adaptee.CarConfiguratorVersion); }
        }

        public IReadOnlyList<ILink> Links
        {
            get { return Adaptee.Links.Cast<Legacy.Link>().Select(x => new Link(x)).ToList(); }
        }

        public IReadOnlyList<IAsset> Assets
        {
            get { return Adaptee.Assets.Cast<Legacy.Asset>().Select(x => new Asset(x)).ToList(); }
        }

        public IReadOnlyList<IBodyType> BodyTypes
        {
            get { return Adaptee.BodyTypes.Cast<Legacy.BodyType>().Select(x => new BodyType(x)).ToList(); }
        }

        public IReadOnlyList<IEngine> Engines
        {
            get { return Adaptee.Engines.Cast<Legacy.Engine>().Select(x => new Engine(x)).ToList(); }
        }

        public IReadOnlyList<ITransmission> Transmissions
        {
            get { return Adaptee.Transmissions.Cast<Legacy.Transmission>().Select(x => new Transmission(x)).ToList(); }
        }

        public IReadOnlyList<IWheelDrive> WheelDrives
        {
            get { return Adaptee.WheelDrives.Cast<Legacy.WheelDrive>().Select(x => new WheelDrive(x)).ToList(); }
        }

        public IReadOnlyList<ISteering> Steerings
        {
            get { return Adaptee.Cars.Cast<Legacy.Car>().Select(car => car.Steering).Distinct().Select(x => new Steering(x)).ToList(); }
        }

        public IReadOnlyList<IGrade> Grades
        {
            get { return Adaptee.Grades.Cast<Legacy.Grade>().Select(x => new Grade(x)).ToList(); }
        }

        public IReadOnlyList<IFuelType> FuelTypes
        {
            get { return Adaptee.FuelTypes.Cast<Legacy.FuelType>().Select(x => new FuelType(x)).ToList(); }
        }

        public IReadOnlyList<ICar> Cars
        {
            get { return Adaptee.Cars.Cast<Legacy.Car>().Select(x => new Car(x)).ToList(); }
        }

        public IReadOnlyList<ISubModel> SubModels
        {
            get { return Adaptee.SubModels.Cast<Legacy.SubModel>().Select(x => new SubModel(x)).ToList(); }
        }

        public IReadOnlyList<IColourCombination> ColourCombinations
        {
            get
            {
                return Adaptee.Cars
                              .Cast<Legacy.Car>()
                              .SelectMany(car => car.Colours.Cast<Legacy.CarColourCombination>())
                              .GroupBy(colourCombination => Tuple.Create(colourCombination.ExteriorColour.ID, colourCombination.Upholstery.ID))
                              .Select(group => new Colours.ColourCombination(group.First()))
                              .OrderBy(combination => combination.ExteriorColour.Type.SortIndex)
                              .ThenBy(combination => combination.ExteriorColour.SortIndex)
                              .ThenBy(combination => combination.Upholstery.Type.SortIndex)
                              .ThenBy(combination => combination.Upholstery.SortIndex)
                              .ToList();
            }
        }

        public IModelEquipment Equipment
        {
            get { return new ModelEquipment();}
        }

        public IModelTechnicalSpecifications TechnicalSpecifications
        {
            get { return new ModelTechnicalSpecifications(); }
        }
    }
}