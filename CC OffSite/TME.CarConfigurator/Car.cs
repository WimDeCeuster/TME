using System;
using System.Collections.Generic;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Interfaces.Packs;

namespace TME.CarConfigurator
{
    public class Car : BaseObject<Repository.Objects.Car>, ICar
    {
        readonly Repository.Objects.Publication _repositoryPublication;
        readonly Repository.Objects.Context _repositoryContext;
        readonly IBodyTypeFactory _bodyTypeFactory;
        readonly IEngineFactory _engineFactory;

        IPrice _basePrice;
        IPrice _startingPrice;
        IBodyType _bodyType;
        IEngine _engine;

        public Car(
            Repository.Objects.Car repositoryCar,
            Repository.Objects.Publication repositoryPublication,
            Repository.Objects.Context repositoryContext,
            IBodyTypeFactory bodyTypeFactory,
            IEngineFactory engineFactory)
            : base(repositoryCar)
        {
            if (repositoryPublication == null) throw new ArgumentNullException("repositoryPublication");
            if (repositoryContext == null) throw new ArgumentNullException("repositoryContext");
            if (bodyTypeFactory == null) throw new ArgumentNullException("bodyTypeFactory");
            if (engineFactory == null) throw new ArgumentNullException("engineFactory");

            _repositoryPublication = repositoryPublication;
            _repositoryContext = repositoryContext;
            _bodyTypeFactory = bodyTypeFactory;
            _engineFactory = engineFactory;
        }

        public int ShortID { get { return RepositoryObject.ShortID; } }
        public bool Promoted { get { return RepositoryObject.Promoted; } }
        public bool WebVisible { get { return RepositoryObject.WebVisible; } }
        public bool ConfigVisible { get { return RepositoryObject.ConfigVisible; } }
        public bool FinanceVisible { get { return RepositoryObject.FinanceVisible; } }
        public IPrice BasePrice { get { return _basePrice = _basePrice  ?? new Price(RepositoryObject.BasePrice); } }
        public IPrice StartingPrice { get { return _startingPrice = _startingPrice ?? new Price(RepositoryObject.StartingPrice); } }
        public IBodyType BodyType { get { return _bodyType = _bodyType ?? _bodyTypeFactory.GetCarBodyType(RepositoryObject.BodyType, RepositoryObject.ID, _repositoryPublication, _repositoryContext); } }
        public IEngine Engine { get { return _engine = _engine ?? _engineFactory.GetCarEngine(RepositoryObject.Engine, RepositoryObject.ID, _repositoryPublication, _repositoryContext); } }
        public ITransmission Transmission { get { throw new NotImplementedException(); } }
        public IWheelDrive WheelDrive { get { throw new NotImplementedException(); } }
        public ISteering Steering { get { throw new NotImplementedException(); } }
        public IGrade Grade { get { throw new NotImplementedException(); }}
        public ISubModel SubModel { get { throw new NotImplementedException(); }}

        public IReadOnlyList<ICarPart> Parts
        {
            get { throw new NotImplementedException(); }
        }

        public ICarEquipment Equipment
        {
            get { throw new NotImplementedException(); }
        }

        public IReadOnlyList<ICarPack> Packs
        {
            get { throw new NotImplementedException(); }
        }
    }
}