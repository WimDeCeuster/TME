using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.S3.Shared.Exceptions
{
    public class ObjectNotFoundException : Exception
    {
        
        public ObjectNotFoundException(Amazon.S3.AmazonS3Exception ex)
            : base("Amazon S3 object not found", ex)
        { }

    }
}
