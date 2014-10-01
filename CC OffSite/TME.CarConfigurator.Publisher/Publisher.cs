using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.Publisher
{
    public interface IPublisher
    {
        void Publish(IContext context);
    }

    public class Publisher : IPublisher
    {
        public Publisher()
        {

        }

        public void Publish(IContext context)
        {
            throw new NotImplementedException();
        }
    }
}
