using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Repository.Objects.Rules;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.CommandServices
{
    public class RuleService : IRuleService
    {
        readonly IService _service;
        readonly ISerialiser _serialiser;
        readonly IKeyManager _keymanager;

        public RuleService(IService service, ISerialiser serialiser, IKeyManager keymanager)
        {
            if (service == null) throw new ArgumentNullException("service");
            if (serialiser == null) throw new ArgumentNullException("serialiser");
            if (keymanager == null) throw new ArgumentNullException("keymanager");

            _service = service;
            _serialiser = serialiser;
            _keymanager = keymanager;
        }

        public async Task PutCarRules(string brand, string country, Guid publicationID, Guid carID, IDictionary<Guid, RuleSets> carRules)
        {
            if (brand == null) throw new ArgumentNullException("brand");
            if (country == null) throw new ArgumentNullException("country");
            if (carRules == null) throw new ArgumentNullException("carRules");

            var path = _keymanager.GetCarRulesKey(publicationID, carID);
            var value = _serialiser.Serialise(carRules);

            await _service.PutObjectAsync(brand, country, path, value);
        }
    }
}