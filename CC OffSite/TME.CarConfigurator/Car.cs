using System;
using System.Collections.Generic;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Factories;

namespace TME.CarConfigurator
{
    public class Car : BaseObject, ICar
    {
        readonly Repository.Objects.Car _repositoryCar;
        readonly Repository.Objects.Publication _repositoryPublication;
        readonly Repository.Objects.Context _repositoryContext;
        readonly IBodyTypeFactory _bodyTypeFactory;

        IPrice _basePrice;
        IPrice _startingPrice;
        IBodyType _bodyType;

        public Car(Repository.Objects.Car repositoryCar, Repository.Objects.Publication repositoryPublication, Repository.Objects.Context repositoryContext, IBodyTypeFactory bodyTypeFactory)
            : base(repositoryCar)
        {
            if (repositoryCar == null) throw new ArgumentNullException("repositoryCar");
            if (repositoryPublication == null) throw new ArgumentNullException("repositoryPublication");
            if (repositoryContext == null) throw new ArgumentNullException("repositoryContext");
            if (bodyTypeFactory == null) throw new ArgumentNullException("bodyTypeFactory");

            _repositoryCar = repositoryCar;
            _repositoryPublication = repositoryPublication;
            _repositoryContext = repositoryContext;
            _bodyTypeFactory = bodyTypeFactory;
        }

        public int ShortID { get { return _repositoryCar.ShortID; } }
        public bool Promoted { get { return _repositoryCar.Promoted; } }
        public bool WebVisible { get { return _repositoryCar.WebVisible; } }
        public bool ConfigVisible { get { return _repositoryCar.ConfigVisible; } }
        public bool FinanceVisible { get { return _repositoryCar.FinanceVisible; } }
        public IPrice BasePrice { get { return _basePrice = _basePrice  ?? new Price(_repositoryCar.BasePrice); } }
        public IPrice StartingPrice { get { return _startingPrice = _startingPrice ?? new Price(_repositoryCar.StartingPrice); } }
        public IBodyType BodyType { get { return _bodyType = _bodyType ?? _bodyTypeFactory.GetCarBodyType(_repositoryCar.BodyType, _repositoryCar.ID, _repositoryPublication, _repositoryContext); } }
        public IEngine Engine { get { throw new NotImplementedException(); } }
        public ITransmission Transmission { get { throw new NotImplementedException(); } }
        public IWheelDrive WheelDrive { get { throw new NotImplementedException(); } }
        public ISteering Steering { get { throw new NotImplementedException(); } }
    }
}