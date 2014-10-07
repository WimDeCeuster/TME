using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Publisher.S3;
using TME.CarConfigurator.Tests.Shared;

namespace TME.Carconfigurator.Tests.GivenAS3Publisher
{
    public class WhenPublishingBodyTypesPerTimeFrame : TestBase
    {
        Context _context;
        IService _service;
        S3Publisher _publisher;
        
        protected override void Arrange()
        {
            var brand = "Toyota";
            var country = "DE";
            var generationID = Guid.Empty;
            
            _context = new Context(brand, country, generationID, CarConfigurator.Publisher.Enums.PublicationDataSubset.Live);
            
            _service = A.Fake<IService>();

            _publisher = new S3Publisher(_service);
        }

        protected override void Act()
        {
            
        }
    }
}
