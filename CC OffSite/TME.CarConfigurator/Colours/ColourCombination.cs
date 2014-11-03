using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Colours
{
    public class ColourCombination : IColourCombination
    {
        readonly Repository.Objects.Colours.ColourCombination _repositoryColourCombination;
        readonly IColourFactory _colourFactory;
        readonly Context _context;
        readonly Publication _publication;
        
        private IExteriorColour _exteriorColour;
        private IUpholstery _upholstery;

        public ColourCombination(Repository.Objects.Colours.ColourCombination repositoryColourCombination, Publication publication, Context context, IColourFactory colourFactory)
        {
            if (repositoryColourCombination == null) throw new ArgumentNullException("repositoryColourCombination");
            if (publication == null) throw new ArgumentNullException("publication");
            if (context == null) throw new ArgumentNullException("context");
            if (colourFactory == null) throw new ArgumentNullException("colourFactory");

            _repositoryColourCombination = repositoryColourCombination;
            _publication = publication;
            _context = context;
            _colourFactory = colourFactory;
        }
    
        public Guid ID
        {
	        get { return _repositoryColourCombination.ID; }
        }

        public IExteriorColour ExteriorColour
        {
            get { return _exteriorColour = _exteriorColour ?? _colourFactory.GetExteriorColour(_repositoryColourCombination.ExteriorColour, _publication, _context); }
        }

        public IUpholstery Upholstery
        {
            get { return _upholstery = _upholstery ?? _colourFactory.GetUpholstery(_repositoryColourCombination.Upholstery, _publication, _context); }
        }

        public int SortIndex
        {
            get { return _repositoryColourCombination.SortIndex; }
        }

        public IReadOnlyList<IAsset> Assets
        {
	        get { throw new NotImplementedException(); }
        }
    }
}
