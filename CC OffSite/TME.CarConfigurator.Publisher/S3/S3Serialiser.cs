using TME.CarConfigurator.Publisher.Interfaces;
using Newtonsoft.Json;

namespace TME.CarConfigurator.Publisher.S3
{
    public class S3Serialiser : IS3Serialiser
    {
        public string Serialise(Repository.Objects.Publication publication)
        {
            return JsonConvert.SerializeObject(publication);
        }
    }
}
