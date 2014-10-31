using System;
using System.Collections.Generic;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Colours
{
    public class Upholstery : BaseObject<Repository.Objects.Colours.Upholstery>, IUpholstery
    {
        private readonly Publication _publication;
        private readonly Context _context;
        private readonly IAssetFactory _assetFactory;

        private UpholsteryType _type;
        private IReadOnlyList<IAsset> _assets;

        public Upholstery(Repository.Objects.Colours.Upholstery repositoryUpholstery, Publication publication, Context context, IAssetFactory assetFactory)
            : base(repositoryUpholstery)
        {
            if (publication == null) throw new ArgumentNullException("publication");
            if (context == null) throw new ArgumentNullException("context");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");

            _publication = publication;
            _context = context;
            _assetFactory = assetFactory;
        }

        public string InteriorColourCode
        {
            get { return RepositoryObject.InteriorColourCode; }
        }

        public string TrimCode
        {
            get { return RepositoryObject.TrimCode; }
        }

        public IUpholsteryType Type
        {
            get { return _type = _type ?? new UpholsteryType(RepositoryObject.Type); }
        }

        public IReadOnlyList<IAsset> Assets
        {
            get { return _assets = _assets ?? _assetFactory.GetAssets(_publication, RepositoryObject.ID, _context); }
        }
    }
}
