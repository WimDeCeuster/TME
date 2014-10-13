using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class LabelBuilder
    {
        private Label _label;

        public LabelBuilder()
        {
            _label = new Label();
        }

        public LabelBuilder WithCode(String code)
        {
            _label.Code = code;
            return this;
        }

        public LabelBuilder WithValue(String value)
        {
            _label.Value = value;
            return this;
        }

        public Label Build()
        {
            return _label;
        }
    }
}
