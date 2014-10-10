using System;

namespace TME.CarConfigurator.S3.QueryServices.Exceptions
{
    public class CountryLanguageCombinationDoesNotExistException : Exception
    {
        public CountryLanguageCombinationDoesNotExistException(string country, string language): base(string.Format("The country/language combination {0}/{1} could not be retrieved.", country, language))
        {

        }
    }
}