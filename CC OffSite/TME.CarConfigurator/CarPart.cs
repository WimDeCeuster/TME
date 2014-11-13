using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator
{
    public class CarPart : ICarPart
    {
        private readonly Repository.Objects.CarPart _carPart;
        private readonly Publication _repositoryPublication;
        private readonly Context _repositoryContext;
        private readonly IAssetFactory _assetFactory;

        public CarPart(Repository.Objects.CarPart carPart, Publication repositoryPublication, Context repositoryContext, IAssetFactory assetFactory)
        {
            if (carPart == null) throw new ArgumentNullException("carPart");
            if (repositoryPublication == null) throw new ArgumentNullException("repositoryPublication");
            if (repositoryContext == null) throw new ArgumentNullException("repositoryContext");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");

            _carPart = carPart;
            _repositoryPublication = repositoryPublication;
            _repositoryContext = repositoryContext;
            _assetFactory = assetFactory;
        }

        public string Code { get { return _carPart.Code; } }
        public string Name { get { return _carPart.Name; } }

        public IReadOnlyList<IVisibleInModeAndView> VisibleIn { get {return _fetchedVisibleIn = _fetchedVisibleIn ?? _assetFactory.GetAssets(_repositoryPublication,)} }
    }
}