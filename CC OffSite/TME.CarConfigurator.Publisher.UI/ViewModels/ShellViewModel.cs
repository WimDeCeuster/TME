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
                NotifyOfPropertyChange(() => CanPublishForReviewAsync);
            }
        }

        public bool CanPublishLiveAsync { get { return SelectedGeneration != null && !IsPublishing; } }
        public bool CanPublishPreviewAsync { get { return SelectedGeneration != null && !IsPublishing; } }
        public bool CanPublishForReviewAsync { get { return !IsPublishing; } }

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

            var result = await CarConfiguratorPublisher.PublishAsync(SelectedGeneration.ID, SelectedEnvironment, Target, Brand, Country, publicationDataSubset);

            IsPublishing = false;

            return result;
        }

        private static void DisplayResult(Result result)
        {
            MessageBox.Show(result is Successfull ? "Success!" : string.Format("Failure! {0}", ((Failed)result).Reason));
        }

        public async Task<Result> PublishForReviewAsync()
        {
            if (IsPublishing) return new Failed { Reason = "Already finished" };

            IsPublishing = true;

            Result result;

            if ((result = await CarConfiguratorPublisher.PublishAsync(new Guid("D45FD002-E547-4D37-BA78-855BAD1CA998"), SelectedEnvironment, Target, Brand, Country, PublicationDataSubset.Preview)) is Failed) { DisplayResult(result); return result; }
            if ((result = await CarConfiguratorPublisher.PublishAsync(new Guid("0B6B6F08-CA5F-4EF9-8720-9A1E033F1276"), SelectedEnvironment, Target, Brand, Country, PublicationDataSubset.Preview)) is Failed) { DisplayResult(result); return result; }
            if ((result = await CarConfiguratorPublisher.PublishAsync(new Guid("66ED2534-32FB-4CDE-8910-F3B8DA35966F"), SelectedEnvironment, Target, Brand, Country, PublicationDataSubset.Preview)) is Failed) { DisplayResult(result); return result; }
            if ((result = await CarConfiguratorPublisher.PublishAsync(new Guid("C7F1DC17-D700-4D62-BCC5-F6B4A88F94E0"), SelectedEnvironment, Target, Brand, Country, PublicationDataSubset.Preview)) is Failed) { DisplayResult(result); return result; }
            if ((result = await CarConfiguratorPublisher.PublishAsync(new Guid("D066CC26-A7A2-4DA2-9A01-FA33F9179698"), SelectedEnvironment, Target, Brand, Country, PublicationDataSubset.Preview)) is Failed) { DisplayResult(result); return result; }

            IsPublishing = false;

            DisplayResult(result);

            return result;
        }
    }
}