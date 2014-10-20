using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class PriceBuilder
    {
        readonly Price _price;

        public PriceBuilder()
        {
            _price = new Price();
        }

        public PriceBuilder WithPriceExVat(Decimal price)
        {
            _price.ExcludingVat = price;

            return this;
        }

        public PriceBuilder WithPriceInVat(Decimal price)
        {
            _price.IncludingVat = price;

            return this;
        }

        public Price Build()
        {
            return _price;
        }
    }
}
