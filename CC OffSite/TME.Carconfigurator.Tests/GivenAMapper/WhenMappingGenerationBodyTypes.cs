using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher;
using TME.CarConfigurator.Tests.Shared;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAMapper
{
    public class WhenMappingGenerationBodyTypes : TestBase
    {
        const string _brand = "Toyota";
        const string _country = "DE";
        Mapper _mapper;

        protected override void Arrange()
        {
            throw new NotImplementedException();
        }

        protected override void Act()
        {
            _mapper.Map(_brand, _country, _generationId, _generationFinder, _context);
        }

        [Fact]
        public void ThenModelGenerationsShouldExist()
        {

        }
    }
}
