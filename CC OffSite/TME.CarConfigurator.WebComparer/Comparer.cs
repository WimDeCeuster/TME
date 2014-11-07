using Awesomium.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.WebComparer
{
    public class Comparer
    {
        private static ConcurrentStack<String> _links;
        private static ConcurrentBag<String> _visited;
        private static List<CompareWindow> _windows;
        private string _startUrl;

        public Result Result { get; private set; }
        public event EventHandler Finished;
        public Stopwatch Watch { get; private set; }
        public bool IsFinished { get; private set; }
        public string Country { get; private set; }
        public string Language { get; private set; }

        public Comparer(String country, String language, String startUrl)
        {
            _links = new ConcurrentStack<String>();
            _visited = new ConcurrentBag<String>();
            _startUrl = startUrl;
            _links.Push(_startUrl);

            Country = country;
            Language = language;

            Watch = new Stopwatch();
        }

        public void Start()
        {
            if (IsFinished)
                return;

            _windows = Enumerable.Range(0, 1).Select(i => new CompareWindow(_visited, _links, WebCore.CreateWebView(200, 200, WebViewType.Offscreen))).ToList();

            Watch.Start();

            _windows.AsParallel().ForAll(window =>
            {
                window.Finished += FinishedHandler;
                window.NewLinks += NewLinksHandler;
                window.Start();
            });
        }


        void NewLinksHandler(object sender, EventArgs e)
        {
            foreach (var window in _windows.Where(w => w.IsFinished))
                window.Start();
        }

        void FinishedHandler(object sender, EventArgs e)
        {
            if (_windows.All(window => window.IsFinished))
            {
                Watch.Stop();

                IsFinished = true;

                Result = new Result
                {
                    FailedPages = _windows.SelectMany(window => window.Result.FailedPages).ToList(),
                    ProcessedPages = _windows.SelectMany(window => window.Result.ProcessedPages).ToList()
                };

                var handler = Finished;
                if (handler != null)
                    handler(sender, new EventArgs());
            }
        }
    }
}
