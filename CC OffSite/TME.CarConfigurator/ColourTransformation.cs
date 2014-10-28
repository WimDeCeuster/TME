using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces.Colours;

namespace TME.CarConfigurator
{
    public class ColourTransformation : IColourTransformation
    {
        readonly Repository.Objects.Colours.ColourTransformation _repoTransformation;

        public ColourTransformation(Repository.Objects.Colours.ColourTransformation repositoryTransformation)
        {
            if (repositoryTransformation == null) throw new ArgumentNullException("repositoryTransformation");

            _repoTransformation = repositoryTransformation;
        }

        public string RGB
        {
            get { return _repoTransformation.RGB; }
        }

        public decimal Hue
        {
            get { return _repoTransformation.Hue; }
        }

        public decimal Saturation
        {
            get { return _repoTransformation.Saturation; }
        }

        public decimal Brightness
        {
            get { return _repoTransformation.Brightness; }
        }

        public decimal Contrast
        {
            get { return _repoTransformation.Contrast; }
        }

        public decimal Alpha
        {
            get { return _repoTransformation.Alpha; }
        }
    }
}
