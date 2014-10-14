using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IBodyTypeFactory _bodyTypeFactory;
        private readonly IEngineFactory _engineFactory;

        private Publication _publication;
        private IEnumerable<IAsset> _assets;
        private IEnumerable<ILink> _links;
        private IEnumerable<IBodyType> _bodyTypes;
        private IEnumerable<IEngine> _engines;

        private CarConfiguratorVersion _carConfiguratorVersion;

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

        public ICarConfiguratorVersion CarConfiguratorVersion { get { return _carConfiguratorVersion = _carConfiguratorVersion ?? new CarConfiguratorVersion(Publication.Generation.CarConfiguratorVersion); } }

        public IEnumerable<ILink> Links { get { return _links = _links ?? Publication.Generation.Links.Select(l => new Link(l)); } }

        public IEnumerable<IAsset> Assets { get { return _assets = _assets ?? Publication.Generation.Assets.Select(a => new Asset(a)); } }

        public IEnumerable<IBodyType> BodyTypes { get { return _bodyTypes = _bodyTypes ?? _bodyTypeFactory.GetBodyTypes(Publication, Context); } }

        public IEnumerable<IEngine> Engines { get { return _engines = _engines ?? _engineFactory.GetEngines(Publication, Context); } }

        public IEnumerable<ITransmission> Transmissions { get { throw new NotImplementedException(); } }

        public IEnumerable<IFuelType> FuelTypes { get { throw new NotImplementedException(); } }

        public IEnumerable<ICar> Cars { get { throw new NotImplementedException(); } }

        public Model(
            Repository.Objects.Model repositoryModel,
            Context context,
            IPublicationFactory publicationFactory,
            IBodyTypeFactory bodyTypeFactory,
            IEngineFactory engineFactory)
            : base(repositoryModel, context)
        {
            if (repositoryModel == null) throw new ArgumentNullException("repositoryModel");
            if (context == null) throw new ArgumentNullException("context");
            if (publicationFactory == null) throw new ArgumentNullException("publicationFactory");
            if (bodyTypeFactory == null) throw new ArgumentNullException("bodyTypeFactory");
            if (engineFactory == null) throw new ArgumentNullException("engineFactory");

            _repositoryModel = repositoryModel;
            _publicationFactory = publicationFactory;
            _bodyTypeFactory = bodyTypeFactory;
            _engineFactory = engineFactory;
        }
    }
}
