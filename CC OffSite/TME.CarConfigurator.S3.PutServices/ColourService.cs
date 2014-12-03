using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;

using TME.CarConfigurator.Repository.Objects.Colours;
using TME.CarConfigurator.Repository.Objects.Packs;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.CommandServices
{
    public class ColourService : IColourService
    {
        private readonly IService _s3Service;
        private readonly ISerialiser _serialiser;
        private readonly IKeyManager _keymanager;

        public ColourService(IService s3Service, ISerialiser serialiser, IKeyManager keymanager)
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

        public async Task PutCarColourCombinations(string brand, string country, Guid publicationID, Guid carID, IList<CarColourCombination> carColourCombinations)
        {
            if (brand == null) throw new ArgumentNullException("brand");
            if (country == null) throw new ArgumentNullException("country");
            if (carColourCombinations == null) throw new ArgumentNullException("carColourCombinations");

            var path = _keymanager.GetCarColourCombinationsKey(publicationID, carID);
            var value = _serialiser.Serialise(carColourCombinations);

            await _s3Service.PutObjectAsync(brand, country, path, value);
        }

        public async Task PutCarPackAccentColourCombinations(string brand, string country, Guid publicationID, Guid carID, IDictionary<Guid, IList<AccentColourCombination>> carPackAccentColourCombinations)
        {
            if (brand == null) throw new ArgumentNullException("brand");
            if (country == null) throw new ArgumentNullException("country");
            if (carPackAccentColourCombinations == null) throw new ArgumentNullException("carPackAccentColourCombinations");
            
            var path = _keymanager.GetCarPackAccentColourCombinationsKey(publicationID, carID);
            var value = _serialiser.Serialise(carPackAccentColourCombinations);

            await _s3Service.PutObjectAsync(brand, country, path, value);
        }
    }
}