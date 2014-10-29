using System;
using System.Collections.Generic;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;

namespace TME.CarConfigurator
{
    public class ExteriorColour : BaseObject<Repository.Objects.Colours.ExteriorColour>, IExteriorColour
    {
        ColourTransformation _transformation;
        ExteriorColourType _type;

        public ExteriorColour(Repository.Objects.Colours.ExteriorColour repoColor)
            : base(repoColor)
        {

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

        public IEnumerable<IAsset> Assets
        {
            get { throw new NotImplementedException(); }
        }
    }
}
