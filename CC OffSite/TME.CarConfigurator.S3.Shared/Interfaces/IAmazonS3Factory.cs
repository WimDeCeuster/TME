using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.S3.Shared.Interfaces
{
    public interface IAmazonS3Factory
    {
        IAmazonS3 CreateInstance(String accessKey, String secretKey);
    }
}
