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
        private readonly Repository.Objects.Core.Label _label;

        public Label(Repository.Objects.Core.Label label)
        {
            if (label == null) throw new ArgumentNullException("label");
            _label = label;
        }

        public string Code { get { return _label.Code; } }

        public string Value { get { return _label.Value; } }
    }
}
