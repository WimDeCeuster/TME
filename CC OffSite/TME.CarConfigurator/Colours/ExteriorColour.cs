using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Assets;
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
        private IReadOnlyList<IVisibleInModeAndView> _fetchedVisibleInModeAndViews;

        public ExteriorColour(Repository.Objects.Colours.ExteriorColour repositoryColour, Publication publication, Context context, IAssetFactory assetFactory)
            : base(repositoryColour)
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
            get { return RepositoryObject.Transformation == null ? null : _transformation = _transformation ?? new ColourTransformation(RepositoryObject.Transformation); }
        }

        public IExteriorColourType Type
        {
            get { return _type = _type ?? new ExteriorColourType(RepositoryObject.Type); }
        }

        public IReadOnlyList<IVisibleInModeAndView> VisibleIn
        {
            get
            {
                return _fetchedVisibleInModeAndViews = _fetchedVisibleInModeAndViews ?? RepositoryObject.VisibleIn.Select(visibleInModeAndView => new VisibleInModeAndView(RepositoryObject.ID, visibleInModeAndView, _publication, _context, _assetFactory)).ToList();
            }
        }

        public IReadOnlyList<IAsset> Assets
        {
            get { return _assets = _assets ?? _assetFactory.GetAssets(_publication, RepositoryObject.ID, _context); }
        }
    }
}
