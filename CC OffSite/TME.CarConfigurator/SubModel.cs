using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator
{
    public class SubModel : BaseObject, ISubModel
    {
        private readonly Repository.Objects.SubModel _repositorySubModel;
        private readonly Publication _repositoryPublication;
        private readonly Context _repositoryContext;
        private readonly IAssetFactory _assetFactory;

        private IEnumerable<IAsset> _assets;
        private IEnumerable<ILink> _links;
        private IPrice _startingPrice;

        public SubModel(Repository.Objects.SubModel repositorySubModel,Publication repositoryPublication,Context repositoryContext,IAssetFactory assetFactory) 
            : base(repositorySubModel)
        {
            if (repositorySubModel == null) throw new ArgumentNullException("repositorySubModel");
            if (repositoryPublication == null) throw new ArgumentNullException("repositoryPublication");
            if (repositoryContext == null) throw new ArgumentNullException("repositoryContext");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");

            _repositorySubModel = repositorySubModel;
            _repositoryPublication = repositoryPublication;
            _repositoryContext = repositoryContext;
            _assetFactory = assetFactory;
        }

        public IPrice StartingPrice { get { return _startingPrice = _startingPrice ?? new Price(_repositorySubModel.StartingPrice); } }

        public IEnumerable<IEquipmentItem> Equipment
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IGrade> Grades
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IAsset> Assets { get { return _assets = _assets ?? _assetFactory.GetAssets(_repositoryPublication, ID, _repositoryContext); } }

        public IEnumerable<ILink> Links
        {
            get { return _links = _links ?? _repositorySubModel.Links.Select(l => new Link(l)).ToArray(); }
        }

 
    }
}