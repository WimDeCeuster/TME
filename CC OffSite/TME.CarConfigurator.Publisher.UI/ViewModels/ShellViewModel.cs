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
        private const string Target = "S3";

        private string _country = "DE";
        private Model _selectedModel;
        private bool _isPublishing;
        private IObservableCollection<string> _messages = new BindableCollection<string>();
        private static ICarConfiguratorPublisher _carConfiguratorPublisher;

        public ShellViewModel()
        {
            Environments = new[] { "Development", "Acceptance", "Production" };
            SelectedEnvironment = Environments.First();
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
            var generationId = SelectedModel.Generations.Single(g => g.Approved).ID;

            await PublishAsync(generationId, PublicationDataSubset.Live);
        }

        public async Task PublishPreviewAsync()
        {
            var generationId = SelectedModel.Generations.Single(g => g.Preview).ID;

            await PublishAsync(generationId, PublicationDataSubset.Preview);
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

                await CarConfiguratorPublisher.PublishAsync(generationId, SelectedEnvironment, Target, Brand, Country, publicationDataSubset, Progress);

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

                var modelsThatHaveApprovedGenerations = Models.Where(m => m.Approved && m.Generations.Any(g => g.Preview)).ToList();
                var modelsInRandomOrder = modelsThatHaveApprovedGenerations.OrderBy(m => Guid.NewGuid()).ToList();
                var first5Models = modelsInRandomOrder.Take(5).ToList();
                var generations = first5Models.Select(m => m.Generations.Single(g => g.Preview)).ToList();

                Messages.Add(String.Format("Starting publish for {0}", string.Join(", ", generations)));

                foreach (var generation in generations)
                {
                    Messages.Add(string.Format("Publishing {0} for {1}", generation, Country));

                    await CarConfiguratorPublisher.PublishAsync(generation.ID, SelectedEnvironment, Target, Brand, Country, PublicationDataSubset.Preview, Progress);
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
            Progress.ProgressChanged -= ProgressChanged;
            Progress = null;

            IsPublishing = false;

            MessageBox.Show(result);
        }
    }
}