using Awesomium.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.WebComparer
{
    public class CompareWindow
    {
        private static object _navigationLock = new Object();

        private WebView _window;
        private System.Collections.Concurrent.ConcurrentBag<String> _visited;
        private System.Collections.Concurrent.ConcurrentStack<String> _links;

        public event EventHandler Finished;
        public event EventHandler NewLinks;

        public Result Result { get; private set; }
        public bool IsFinished { get; private set; }
        public bool IsCrashed { get; private set; }

        public CompareWindow(ConcurrentBag<string> visited, ConcurrentStack<string> links, WebView window)
        {
            _visited = visited;
            _links = links;

            IsFinished = true;
            Result = new Result();

            _window = window;

            _window.LoadingFrameComplete += DocumentReadyHandler;
            _window.Crashed += CrashedHandler;
            _window.LoadingFrameFailed += LoadingFrameFailedHandler;
        }

        public void Start() {
            lock (_navigationLock)
            {
                if (IsCrashed || !IsFinished)
                    return;
                IsFinished = false;
            }
            GoToNextPage();
        }

        void LoadingFrameFailedHandler(object sender, LoadingFrameFailedEventArgs e)
        {
            Result.FailedPages.Add(new PageFailure(e.Url.ToString(), "Page load failed"));

            Console.WriteLine("Page load failed... {0}", e.Url);

            GoToNextPage();
        }

        void CrashedHandler(object sender, CrashedEventArgs e)
        {
            Result.FailedPages.Add(new PageFailure(_window.Source.ToString(), "CRASHED"));
            Finish();
        }

        void DocumentReadyHandler(object sender, UrlEventArgs e)
        {
            try
            {
                ProcessPage();
            }
            catch (Exception ex)
            {
                Result.FailedPages.Add(new PageFailure(e.Url.ToString(), String.Format("Failed to process page {0}: {1}", e.Url, ex.Message)));
                Console.WriteLine("Failed to process page {0}: {1}", e.Url, ex.Message);
            }

            GoToNextPage();
        }

        void GoToNextPage()
        {
            while (_links.Count > 0)
            {
                var nextUrl = "";
                if (!_links.TryPop(out nextUrl))
                    break;

                lock (_navigationLock) { 
                    if (_visited.Contains(nextUrl))
                        continue;

                    _window.BeginInvoke(new Action(() => _visited.Add(_window.Source.ToString())), new object[] { });
                }

                Console.WriteLine("{0} links left", _links.Except(_visited).Distinct().Count());
                _window.BeginInvoke(new Action(() => _window.Source = new Uri(nextUrl)), new object[] { });
                return;
            }
            Finish();
        }

        private void ProcessPage()
        {
            if (!CanBeVerified())
            {
                Result.FailedPages.Add(new PageFailure(_window.Source.ToString(), "Unrecognised page structure."));
                Console.WriteLine("Cannot process {0}: unrecognised page structure", _window.Source);
                return;
            }

            while (!PageIsVerified())
                System.Threading.Thread.Sleep(500);

            var pageResult = new PageResult(_window.Source.ToString());
            if (!PageIsValid())
            {                
                pageResult.MissingObjects = GetMissingObjects();
                pageResult.WrongOrderedObjects = GetWrongOrderedObjects();
                pageResult.MismatchedProperties = GetMismatchedProperties();   
            }

            Result.ProcessedPages.Add(pageResult);

            var pageLinks = GetPageLinks();
            lock (_navigationLock)
            {
                var hadLinks = pageLinks.Count > 0;

                foreach (var link in pageLinks)
                    _links.Push(link);

                if (!hadLinks && pageLinks.Any())
                    NotifyNewLinks();
            }
        }

        private IList<String> GetMissingObjects()
        {
            var missingObjects = (JSValue[])_window.ExecuteJavascriptWithResult(
                @"$('.object.missing').toArray().map(getBreadCrumb)");
            return missingObjects.Select(value => (String)value).ToList();
        }

        private IList<String> GetWrongOrderedObjects()
        {
            var wrongOrderedObjects = (JSValue[])_window.ExecuteJavascriptWithResult(
                @"$('#newarea .object.wrong-order').toArray().map(getBreadCrumb)");
            return wrongOrderedObjects.Select(value => (String)value).ToList();
        }

        private IList<String> GetMismatchedProperties()
        {
            var mismatchedProperties = (JSValue[])_window.ExecuteJavascriptWithResult(
                @"$('#newarea .property.mismatch').toArray().map(getBreadCrumb)");
            return mismatchedProperties.Select(value => (String)value).ToList();
        }

        private void Finish()
        {
            var handlers = Finished;

            IsFinished = true;

            if (handlers != null)
                handlers(this, new EventArgs());
        }

        private void NotifyNewLinks()
        {
 	        var handlers = NewLinks;

            if (handlers != null)
                handlers(this, new EventArgs());
        }

        private IList<String> GetPageLinks()
        {
            var result = (JSValue[])_window.ExecuteJavascriptWithResult("$('.object:not(.ignore-compare) a').toArray().filter(function (a) { return !$(a).parents('.object.missing').length }).map(function (a) { return a.href })");
            return result.Select(value => (String)value).ToList();
        }

        private bool CanBeVerified()
        {
            var result = _window.ExecuteJavascriptWithResult("$('#validated').length");

            if (!result.IsInteger)
                return false;

            return (int)result == 1;
        }

        private bool PageIsVerified()
        {
            var result = _window.ExecuteJavascriptWithResult("$('#validated.valid, #validated.invalid').length");

            if (!result.IsInteger)
                return false;

            return (int)result == 1;
        }

        private bool PageIsValid()
        {
            var result = _window.ExecuteJavascriptWithResult("$('#validated.valid').length");

            if (!result.IsInteger)
                return false;

            return (int)result == 1;
        }
    }
}
