using System;
using System.Collections.Generic;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator
{
    public class Car : BaseObject, ICar
    {
        readonly Repository.Objects.Car _repositoryCar;
        Price _basePrice;
        Price _startingPrice;

        public Car(Repository.Objects.Car car)
            : base(car)
        {
            _repositoryCar = car;
        }

        public int ShortID { get { return _repositoryCar.ShortID; } }
        public bool Promoted { get { return _repositoryCar.Promoted; } }
        public bool WebVisible { get { return _repositoryCar.WebVisible; } }
        public bool ConfigVisible { get { return _repositoryCar.ConfigVisible; } }
        public bool FinanceVisible { get { return _repositoryCar.FinanceVisible; } }
        public IPrice BasePrice { get { return _basePrice = _basePrice  ?? new Price(_repositoryCar.BasePrice); } }
        public IPrice StartingPrice { get { return _startingPrice = _startingPrice ?? new Price(_repositoryCar.StartingPrice); } }
        public IBodyType BodyType { get { throw new NotImplementedException(); } }
        public IEngine Engine { get { throw new NotImplementedException(); } }
        public ITransmission Transmission { get { throw new NotImplementedException(); } }
    }
}