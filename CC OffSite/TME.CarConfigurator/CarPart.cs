using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Assets;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator
{
    public class CarPart : ICarPart
    {
        private readonly Repository.Objects.CarPart _repositoryCarPart;
        private readonly IAssetFactory _assetFactory;
        private readonly Publication _repositoryPublication;
        private readonly Guid _carID;
        private readonly Context _repositoryContext;
        private IReadOnlyList<VisibleInModeAndView> _fetchedVisibleIn;

        public CarPart(Repository.Objects.CarPart repositoryCarPart, Publication repositoryPublication, Guid carID, Context repositoryContext, IAssetFactory assetFactory)
        {
            if (repositoryCarPart == null) throw new ArgumentNullException("repositoryCarPart");
            if (repositoryPublication == null) throw new ArgumentNullException("repositoryPublication");
            if (repositoryContext == null) throw new ArgumentNullException("repositoryContext");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");

            _repositoryCarPart = repositoryCarPart;
            _assetFactory = assetFactory;
            _repositoryContext = repositoryContext;
            _repositoryPublication = repositoryPublication;
            _carID = carID;
        }

        public string Code { get { return _repositoryCarPart.Code; } }
        public string Name { get { return _repositoryCarPart.Name; } }
        public Guid ID { get { return _repositoryCarPart.ID; } }

        public IReadOnlyList<IVisibleInModeAndView> VisibleIn
        {
            get
            {
                return
                    _fetchedVisibleIn =
                    _fetchedVisibleIn ??
                    _repositoryCarPart.VisibleIn.Select(
                    visibleIn =>
                    new CarPartVisibleInModeAndView(_carID, ID, visibleIn, _repositoryPublication,
                    _repositoryContext, _assetFactory)).ToList();
            }
        }
    }
}