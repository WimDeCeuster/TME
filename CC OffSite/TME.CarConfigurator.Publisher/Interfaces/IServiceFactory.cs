using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IServiceFactory
    {
        IService Get(String target, String brand, String country);
    }
}
