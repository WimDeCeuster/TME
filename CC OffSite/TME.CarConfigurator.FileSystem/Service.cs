using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.S3.Shared.Exceptions;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.FileSystem
{
    public class Service : IService
    {
        readonly String _pathTemplate;

        public Service(String pathTemplate)
        {
            if (String.IsNullOrWhiteSpace(pathTemplate)) throw new ArgumentNullException("pathTemplate");

            _pathTemplate = pathTemplate;
        }

        public async Task PutObjectAsync(string brand, string country, string key, string item)
        {
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentException("brand cannot be empty", "brand");
            if (String.IsNullOrWhiteSpace(country)) throw new ArgumentException("country cannot be empty", "country");
            if (String.IsNullOrWhiteSpace(key)) throw new ArgumentException("key cannot be empty", "key");
            if (String.IsNullOrWhiteSpace(item)) throw new ArgumentException("item cannot be empty", "item");

            var path = GetPath(brand, country, key);

            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));

            System.IO.File.WriteAllText(path, item);

            await Task.FromResult(String.Empty);
        }

        public string GetObject(string brand, string country, string key)
        {
            if (brand == null) throw new ArgumentNullException("brand");
            if (country == null) throw new ArgumentNullException("country");
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentException("brand cannot be empty", "brand");
            if (String.IsNullOrWhiteSpace(country)) throw new ArgumentException("country cannot be empty", "country");

            var path = GetPath(brand, country, key);

            if (!System.IO.File.Exists(path))
                throw new ObjectNotFoundException(null, key);

            return System.IO.File.ReadAllText(path);
        }

        private string GetPath(string brand, string country, string key)
        {
            return System.IO.Path.Combine(FillPath(brand, country), key);
        }

        private string FillPath(String brand, String country)
        {
            return _pathTemplate.ToLowerInvariant()
                                .Replace("{brand}", brand.ToLowerInvariant())
                                .Replace("{country}", country.ToLowerInvariant());
        }

        public void Dispose()
        {
            return; //nothing to dispose
        }
    }
}
