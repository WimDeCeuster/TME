using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator
{
    public class Label : ILabel
    {
        private readonly Repository.Objects.Core.Label _repositoryLabel;

        public Label(Repository.Objects.Core.Label repositoryLabel)
        {
            if (repositoryLabel == null) throw new ArgumentNullException("repositoryLabel");
            _repositoryLabel = repositoryLabel;
        }

        public string Code { get { return _repositoryLabel.Code; } }

        public string Value { get { return _repositoryLabel.Value; } }
    }
}
