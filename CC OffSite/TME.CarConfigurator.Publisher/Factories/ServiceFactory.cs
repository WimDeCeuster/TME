using Amazon.S3;
using Spring.Context.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Interfaces;

namespace TME.CarConfigurator.Publisher.Factories
{
    public class ServiceFactory : IServiceFactory
    {
        public IS3Service Get(String target, String brand, String country)
        {
            var springContext = ContextRegistry.GetContext();
            switch (target)
            {
                case "S3":
                    var client = (IAmazonS3)springContext.CreateObject("AmazonS3", typeof(IAmazonS3), new object[] { });
                    return (IS3Service)springContext.CreateObject("S3Service", typeof(IS3Service), new object[] { brand, country, client });
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
