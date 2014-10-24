using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Common.Result;
using TME.CarConfigurator.Publisher.Interfaces;
using System.Windows;

namespace TME.CarConfigurator.Publisher.UI.ViewModels
{
    public class ShellViewModel : PropertyChangedBase
    {
        private const string Brand = "Toyota";
        private const string Target = "S3";

        private string _country = "DE";
        private Model _selectedModel;
        private ModelGeneration _selectedGeneration;
        private bool _isPublishing;
        private static ICarConfiguratorPublisher _carConfiguratorPublisher;

        public ShellViewModel()
        {
            Environments = new[] { "Development", "Production" };
            SelectedEnvironment = Environments.First();
        }

        public ICarConfiguratorPublisher PublicationService
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
                SelectedGeneration = null;

                NotifyOfPropertyChange(() => Country);
                NotifyOfPropertyChange(() => Models);
                NotifyOfPropertyChange(() => Generations);
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
                NotifyOfPropertyChange(() => Generations);
            }
        }

        public IEnumerable<ModelGeneration> Generations
        {
            get { return SelectedModel == null ? new List<ModelGeneration>() : (IList<ModelGeneration>)SelectedModel.Generations; }
        }

        public IEnumerable<String> Environments { get; set; }

        public ModelGeneration SelectedGeneration
        {
            get { return _selectedGeneration; }
            set
            {
                if (Equals(value, _selectedGeneration)) return;
                _selectedGeneration = value;

                NotifyOfPropertyChange(() => SelectedGeneration);
                NotifyOfPropertyChange(() => CanPublishLiveAsync);
                NotifyOfPropertyChange(() => CanPublishPreviewAsync);
            }
        }

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
            }
        }

        public bool CanPublishLiveAsync { get { return SelectedGeneration != null && !IsPublishing; } }
        public bool CanPublishPreviewAsync { get { return SelectedGeneration != null && !IsPublishing; } }

        public async void PublishLiveAsync()
        {
            var result = await PublishAsync(PublicationDataSubset.Live);

            DisplayResult(result);
        }

        public async void PublishPreviewAsync()
        {
            var result = await PublishAsync(PublicationDataSubset.Preview);

            DisplayResult(result);
        }

        private async Task<Result> PublishAsync(PublicationDataSubset publicationDataSubset)
        {
            if (IsPublishing) return new Failed { Reason = "Already finished" };

            IsPublishing = true;

            var result = await PublicationService.PublishAsync(SelectedGeneration.ID, SelectedEnvironment, Target, Brand, Country, publicationDataSubset);

            IsPublishing = false;

            return result;
        }

        private static void DisplayResult(Result result)
        {
            MessageBox.Show(result is Successfull ? "Success!" : string.Format("Failure! {0}", ((Failed)result).Reason));
        }
    }
}