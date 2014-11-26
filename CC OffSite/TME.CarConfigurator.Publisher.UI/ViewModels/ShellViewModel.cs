using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher.Common.Enums;

using TME.CarConfigurator.Publisher.Interfaces;
using System.Windows;
using TME.CarConfigurator.Publisher.Progress;

namespace TME.CarConfigurator.Publisher.UI.ViewModels
{
    public class ShellViewModel : PropertyChangedBase
    {
        private const string Brand = "Toyota";
        private const string PublishedBy = "UIPublisher";
        private const string AssetUrl = @"http://t1-carassets.toyota-europe.com";

        private string _country = "DE";
        private Model _selectedModel;
        private bool _isPublishing;
        private IObservableCollection<string> _messages = new BindableCollection<string>();
        private static ICarConfiguratorPublisher _carConfiguratorPublisher;

        public ShellViewModel()
        {
            Environments = new[] { "Development", "Acceptance", "Production" };
            SelectedEnvironment = Environments.First();

            Targets = new[] { "FileSystem", "S3" };
            SelectedTarget = Targets.First();
        }

        public ICarConfiguratorPublisher CarConfiguratorPublisher
        {
            get { return _carConfiguratorPublisher; }
            set { _carConfiguratorPublisher = value; }
        }

        public string Country
        {
            get { return _country; }
            set
            {
                if (value == _country) return;
                _country = value;

                SelectedModel = null;

                NotifyOfPropertyChange(() => Country);
                NotifyOfPropertyChange(() => Models);
            }
        }

        public IEnumerable<Model> Models
        {
            get
            {
                try
                {
                    MyContext.SetSystemContext(Brand, _country, "en");

                    return Administration.Models.GetModels();
                }
                catch (Exception)
                {
                    return new List<Model>();
                }
            }
        }

        public Model SelectedModel
        {
            get { return _selectedModel; }
            set
            {
                if (Equals(value, _selectedModel)) return;
                _selectedModel = value;

                NotifyOfPropertyChange(() => SelectedModel);
                NotifyOfPropertyChange(() => CanPublishLiveAsync);
                NotifyOfPropertyChange(() => CanPublishPreviewAsync);
            }
        }

        public IEnumerable<String> Environments { get; set; }

        public string SelectedEnvironment { get; set; }

        public IEnumerable<String> Targets { get; set; }

        public string SelectedTarget { get; set; }

        public bool IsPublishing
        {
            get { return _isPublishing; }
            set
            {
                if (value.Equals(_isPublishing)) return;
                _isPublishing = value;
                NotifyOfPropertyChange(() => IsPublishing);
                NotifyOfPropertyChange(() => CanPublishLiveAsync);
                NotifyOfPropertyChange(() => CanPublishPreviewAsync);
                NotifyOfPropertyChange(() => CanPublishForReviewAsync);
            }
        }

        public bool CanPublishLiveAsync { get { return SelectedModel != null && !IsPublishing; } }
        public bool CanPublishPreviewAsync { get { return SelectedModel != null && !IsPublishing; } }
        public bool CanPublishForReviewAsync { get { return !IsPublishing; } }


        public IObservableCollection<string> Messages
        {
            get { return _messages; }
            set
            {
                if (Equals(value, _messages)) return;
                _messages = value;
                NotifyOfPropertyChange(() => Messages);
            }
        }
        
        public Progress<PublishProgress> Progress { get; private set; }

        public async Task PublishLiveAsync()
        {
            if(!SelectedModel.Approved)
            {
                PublishingDone("Model is not approved for live");
                return;
            }

            var generation = SelectedModel.Generations.SingleOrDefault(g => g.Approved);

            if (generation == null)
            {
                PublishingDone("No generation found");
                return;
            }

            await PublishAsync(generation.ID, PublicationDataSubset.Live);
        }

        public async Task PublishPreviewAsync()
        {
            if (!SelectedModel.Preview)
            {
                PublishingDone("Model is not approved for preview");
                return;
            }

            var generation = SelectedModel.Generations.SingleOrDefault(g => g.Preview);

            if (generation == null)
            {
                PublishingDone("No generation found");
                return;
            }

            await PublishAsync(generation.ID, PublicationDataSubset.Preview);
        }

        private async Task PublishAsync(Guid generationId, PublicationDataSubset publicationDataSubset)
        {
            try
            {
                if (IsPublishing)
                {
                    PublishingDone("Already publishing");
                    return;
                }

                StartPublishing();

                await CarConfiguratorPublisher.PublishAsync(generationId, SelectedEnvironment, SelectedTarget, Brand, Country, publicationDataSubset, PublishedBy, AssetUrl, Progress);

                PublishingDone("Success!");
            }
            catch (Exception e)
            {
                PublishingDone(e.ToString());
            }
        }

        public async Task PublishForReviewAsync()
        {
            try
            {
                if (IsPublishing)
                {
                    PublishingDone("Already publishing");
                    return;
                }

                StartPublishing();

                var modelsThatHaveAreApprovedForPreviewAndThatHaveGenerationsThatAreApprovedForPreview = Models.Where(m => m.Preview && m.Generations.Any(g => g.Preview)).ToList(); // naamgeving expres overdreven :P
                var modelsInRandomOrder = modelsThatHaveAreApprovedForPreviewAndThatHaveGenerationsThatAreApprovedForPreview.OrderBy(m => Guid.NewGuid()).ToList();
                var first5Models = modelsInRandomOrder.Take(5).ToList();
                var generations = first5Models.Select(m => m.Generations.Single(g => g.Preview)).ToList();

                Messages.Add(String.Format("Starting publish for {0}", string.Join(", ", generations)));

                foreach (var generation in generations)
                {
                    Messages.Add(string.Format("Publishing {0} for {1}", generation, Country));

                    await CarConfiguratorPublisher.PublishAsync(generation.ID, SelectedEnvironment, SelectedTarget, Brand, Country, PublicationDataSubset.Preview, PublishedBy, AssetUrl, Progress);
                }

                PublishingDone("Success!");
            }
            catch (Exception e)
            {
                PublishingDone(e.ToString());
            }
        }

        public void GetRandomCountry()
        {
            Country = MyContext.GetContext().Countries.Select(c => c.Code).OrderBy(c => Guid.NewGuid()).First();
        }

        private void StartPublishing()
        {
            IsPublishing = true;

            Messages.Clear();

            Progress = new Progress<PublishProgress>();
            Progress.ProgressChanged += ProgressChanged;
        }

        private void ProgressChanged(object sender, PublishProgress progress)
        {
            Messages.Add(progress.Message);
        }

        private void PublishingDone(string result)
        {
            if (Progress != null)
            {
                Progress.ProgressChanged -= ProgressChanged;
                Progress = null;
            }

            IsPublishing = false;

            MessageBox.Show(result);
        }
    }
}