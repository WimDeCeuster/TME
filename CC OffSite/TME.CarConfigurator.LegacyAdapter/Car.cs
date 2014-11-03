using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.LegacyAdapter.Equipment;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class Car : BaseObject, ICar
    {
        #region Dependencies (Adaptee)
        private Legacy.Car Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public Car(Legacy.Car adaptee)
            : base(adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

        public int ShortID
        {
            get { return Adaptee.ShortID; }
        }

        public bool Promoted
        {
            get { return Adaptee.Promoted; }
        }

        public bool WebVisible
        {
            get { return Adaptee.WebVisible; }
        }

        public bool ConfigVisible
        {
            get { return Adaptee.ConfigVisible; }
        }

        public bool FinanceVisible
        {
            get { return Adaptee.FinanceVisible; }
        }

        public IPrice BasePrice
        {
            get { return new Price(Adaptee); }
        }

        public IPrice StartingPrice
        {
            get { return new StartingPrice(Adaptee); }
        }

        public IBodyType BodyType
        {
            get { return new BodyType(Adaptee.BodyType); }
        }

        public IEngine Engine
        {
            get { return new Engine(Adaptee.Engine); }
        }

        public ITransmission Transmission
        {
            get { return new Transmission(Adaptee.Transmission); }
        }

        public IWheelDrive WheelDrive
        {
            get { return new WheelDrive(Adaptee.WheelDrive); }
        }

        public ISteering Steering
        {
            get { return new Steering(Adaptee.Steering); }
        }

        public IGrade Grade
        {
            get { return new Grade(Adaptee.Grade); }
        }
        public ISubModel SubModel
        {
            get
            {
                return Adaptee.SubModel != null
                    ? new SubModel(Adaptee.SubModel)
                    : null;
            }
        }

        public IReadOnlyList<ICarPart> Parts
        {
            get { return Adaptee.Parts.Cast<Legacy.CarPart>().Select(x => new CarPart(x)).ToList(); }
        }

        public ICarEquipment Equipment
        {
            get { return new CarEquipment(Adaptee); }
        }
    }
}
