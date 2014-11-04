using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;

using TME.CarConfigurator.Repository.Objects.Colours;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.CommandServices
{
    public class ColourCombinationService : IColourService
    {
        private readonly IService _s3Service;
        private readonly ISerialiser _serialiser;
        private readonly IKeyManager _keymanager;

        public ColourCombinationService(IService s3Service, ISerialiser serialiser, IKeyManager keymanager)
        {
            if (s3Service == null) throw new ArgumentNullException("s3Service");
            if (serialiser == null) throw new ArgumentNullException("serialiser");
            if (keymanager == null) throw new ArgumentNullException("keymanager");

            _s3Service = s3Service;
            _serialiser = serialiser;
            _keymanager = keymanager;
        }

        public async Task PutTimeFrameGenerationColourCombinations(string brand, string country, Guid publicationID, Guid timeFrameID,
            IEnumerable<ColourCombination> colourCombinations)
        {
            if (brand == null) throw new ArgumentNullException("brand");
            if (country == null) throw new ArgumentNullException("country");
            if (colourCombinations == null) throw new ArgumentNullException("colourCombinations");

            var path = _keymanager.GetColourCombinationsKey(publicationID, timeFrameID);
            var value = _serialiser.Serialise(colourCombinations);

            await _s3Service.PutObjectAsync(brand, country, path, value);
        }
    }
}