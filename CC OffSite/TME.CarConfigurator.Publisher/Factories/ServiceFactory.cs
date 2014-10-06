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
        public IService Get(String target, String brand, String country)
        {
            var springContext = ContextRegistry.GetContext();
            switch (target)
            {
                case "S3":
                    var serialiser = (IS3Serialiser)springContext.CreateObject("S3Serialiser", typeof(IS3Serialiser), new object[] {});
                    return (IService)springContext.CreateObject("S3Service", typeof(IService), new object[] { brand, country, serialiser });
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
