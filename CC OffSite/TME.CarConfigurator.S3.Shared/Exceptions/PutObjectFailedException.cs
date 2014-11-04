using System;
using Amazon.S3.Model;

namespace TME.CarConfigurator.S3.Shared.Exceptions
{
    public class PutObjectFailedException : Exception
    {
        public PutObjectRequest Request { get; private set; }

        public PutObjectResponse Response { get; private set; }

        public PutObjectFailedException(PutObjectRequest request, PutObjectResponse response)
            : base(string.Format("Could not put object for request {0}\\{1}, response status code was {2}", request.BucketName, request.Key, response.HttpStatusCode))
        {
            Request = request;
            Response = response;
        }
    }
}