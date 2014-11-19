using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Interfaces;

namespace TME.CarConfigurator.Publisher.Services
{

    public class AssetFileService : IAssetFileService
    {
        readonly Dictionary<String, String> _cache = new Dictionary<String, String>();

        public String GetFileContent(String filePath, String assetUrl)
        {
            if (String.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException("filePath");

            var key = filePath.ToLowerInvariant();
            if (!_cache.ContainsKey(key))
                _cache.Add(key, FetchFileContent(filePath, assetUrl));

            return _cache[key];
        }

        static String FetchFileContent(String path, String assetUrl)
        {
            using (var webClient = new WebClient()) { 
                var content = webClient.DownloadString(new Uri(new Uri(assetUrl), path));
            
                return content;
            }
        }
    }
}
