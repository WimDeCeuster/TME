using System;
using System.Collections.Generic;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Rules;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.QueryServices
{
    public class RuleService : IRuleService
    {
        private readonly IService _service;
        private readonly ISerialiser _serialiser;
        private readonly IKeyManager _keyManager;

        public RuleService(IService service, ISerialiser serialiser, IKeyManager keyManager)
        {
            if (service == null) throw new ArgumentNullException("service");
            if (serialiser == null) throw new ArgumentNullException("serialiser");
            if (keyManager == null) throw new ArgumentNullException("keyManager");

            _service = service;
            _serialiser = serialiser;
            _keyManager = keyManager;
        }

        public RuleSets GetCarItemRuleSets(Guid itemID, Guid carID, Guid publicationID, Context context)
        {
            if (context == null) throw new ArgumentNullException("context");

            var key = _keyManager.GetCarRulesKey(publicationID,carID);
            var serialisedObject = _service.GetObject(context.Brand, context.Country, key);

            return _serialiser.Deserialise<IDictionary<Guid, RuleSets>>(serialisedObject)[itemID];
        }
    }
}