using System;
using System.Collections.Generic;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Colours
{
    public class ExteriorColour : BaseObject<Repository.Objects.Colours.ExteriorColour>, IExteriorColour
    {
        private readonly Publication _publication;
        private readonly Context _context;
        private readonly IAssetFactory _assetFactory;

        private ColourTransformation _transformation;
        private ExteriorColourType _type;
        private IReadOnlyList<IAsset> _assets;

        public ExteriorColour(Repository.Objects.Colours.ExteriorColour repoColor, Publication publication, Context context, IAssetFactory assetFactory)
            : base(repoColor)
        {
            if (publication == null) throw new ArgumentNullException("publication");
            if (context == null) throw new ArgumentNullException("context");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");

            _publication = publication;
            _context = context;
            _assetFactory = assetFactory;
        }

        public bool Promoted
        {
            get { return RepositoryObject.Promoted; }
        }

        public IColourTransformation Transformation
        {
            get { return _transformation = _transformation ?? new ColourTransformation(RepositoryObject.Transformation); }
        }

        public IExteriorColourType Type
        {
            get { return _type = _type ?? new ExteriorColourType(RepositoryObject.Type); }
        }

        public IReadOnlyList<IAsset> Assets
        {
            get { return _assets = _assets ?? _assetFactory.GetAssets(_publication, RepositoryObject.ID, _context); }
        }
    }
}
