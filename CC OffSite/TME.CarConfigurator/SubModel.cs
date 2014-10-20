using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Extensions;
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




        public IPrice StartingPrice
        {
            get { throw new NotImplementedException(); }
        }

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
            get { throw new NotImplementedException(); }
        }

 
    }
}