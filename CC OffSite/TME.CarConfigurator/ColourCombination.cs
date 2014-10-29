using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Factories;

namespace TME.CarConfigurator
{
    public class ColourCombination : IColourCombination
    {
        readonly Repository.Objects.Colours.ColourCombination _repositoryColourCombination;
        readonly IColourFactory _colourFactory;
        
        private IExteriorColour _exteriorColour;
        private IUpholstery _upholstery;

        public ColourCombination(Repository.Objects.Colours.ColourCombination repositoryColourCombination, IColourFactory colourFactory)
        {
            if (repositoryColourCombination == null) throw new ArgumentNullException("repositoryColourCombination");
            if (colourFactory == null) throw new ArgumentNullException("colourFactory");

            _repositoryColourCombination = repositoryColourCombination;
            _colourFactory = colourFactory;
        }
    
        public Guid ID
        {
	        get { return _repositoryColourCombination.ID; }
        }

        public IExteriorColour ExteriorColour
        {
            get { return _exteriorColour = _exteriorColour ?? new ExteriorColour(_repositoryColourCombination.ExteriorColour); }
        }

        public IUpholstery Upholstery
        {
            get { return _upholstery = _upholstery ?? _colourFactory.GetUpholstery(_repositoryColourCombination.Upholstery); }
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
