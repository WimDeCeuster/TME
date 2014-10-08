using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.Carconfigurator.Tests.Base;
using TME.CarConfigurator.Publisher;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Publisher.S3;
using TME.CarConfigurator.Tests.Shared;

namespace TME.Carconfigurator.Tests.GivenAS3Publisher
{
    public class WhenPublishingBodyTypesPerTimeFrame : PublicationTestBase
    {
        
        protected override void Arrange()
        {
            base.Arrange();

        }

    }
}
