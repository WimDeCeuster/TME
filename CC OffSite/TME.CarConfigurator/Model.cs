using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator
{
    public class Model : Core.BaseObject, IModel
    {
        private readonly Repository.Objects.Model _repositoryModel;
        private readonly IPublicationFactory _publicationFactory;
        private readonly IAssetFactory _assetFactory;
        private Publication _publication;

        private Publication Publication
        {
            get
            {
                _publication = _publication ?? _publicationFactory.GetPublication(_repositoryModel, Context);
                return _publication;
            }
        }

        public string Brand { get { return _repositoryModel.Brand; } }

        public bool Promoted { get { return _repositoryModel.Promoted; } }

        public string SSN { get { return Publication.Generation.SSN; } }

        public ICarConfiguratorVersion CarConfiguratorVersion { get { throw new NotImplementedException(); } }

        public IEnumerable<ILink> Links { get { throw new NotImplementedException(); } }

        public IEnumerable<IAsset> Assets { get { return _assetFactory.GetAssets(ID); } }

        public IEnumerable<IBodyType> BodyTypes { get { throw new NotImplementedException(); } }

        public IEnumerable<IEngine> Engines { get { throw new NotImplementedException(); } }

        public IEnumerable<IFuelType> FuelTypes { get { throw new NotImplementedException(); } }

        public IEnumerable<ICar> Cars { get { throw new NotImplementedException(); } }

        public Model(Repository.Objects.Model repositoryModel, Context context, IPublicationFactory publicationFactory, IAssetFactory assetFactory)
            : base(repositoryModel, context)
        {
            if (repositoryModel == null) throw new ArgumentNullException("repositoryModel");
            if (publicationFactory == null) throw new ArgumentNullException("publicationFactory");
            if (context == null) throw new ArgumentNullException("context");

            _repositoryModel = repositoryModel;
            _publicationFactory = publicationFactory;
            _assetFactory = assetFactory;
        }
    }
}
