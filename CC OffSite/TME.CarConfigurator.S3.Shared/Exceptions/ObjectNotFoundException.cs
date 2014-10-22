using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.S3.Shared.Exceptions
{
    public class ObjectNotFoundException : Exception
    {
        public String Path { get; private set; }

        public ObjectNotFoundException(Amazon.S3.AmazonS3Exception ex, String path)
            : base("Amazon S3 object not found", ex)
        {
            Path = path;        
        }

    }
}
