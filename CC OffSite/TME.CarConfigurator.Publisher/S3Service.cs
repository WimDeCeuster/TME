using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.Publisher
{
    public interface IS3Service
    {
        void PutObject(String key, String item);
    }

    public class S3Service : IS3Service
    {
        public void PutObject(string key, string item)
        {
            throw new NotImplementedException();
        }
    }
}
