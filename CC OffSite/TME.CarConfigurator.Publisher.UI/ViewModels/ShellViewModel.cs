using System;
using System.Collections.Generic;
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
        private static ICarConfiguratorPublisher _carConfiguratorPublisher;
        private static String[] _environments = new [] { "Development", "Production" };
        private String _selectedEnvironment = _environments[0];

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

        public IEnumerable<String> Environments
        {
            get { return _environments; }
        }

        public ModelGeneration SelectedGeneration
        {
            get { return _selectedGeneration; }
            set
            {
                if (Equals(value, _selectedGeneration)) return;
                _selectedGeneration = value;

                NotifyOfPropertyChange(() => SelectedGeneration);
                NotifyOfPropertyChange(() => CanPublishLive);
                NotifyOfPropertyChange(() => CanPublishPreview);
            }
        }

        public String SelectedEnvironment
        {
            get { return _selectedEnvironment; }
            set
            {
                if (Equals(value, _selectedEnvironment)) return;
                _selectedEnvironment = value;

                NotifyOfPropertyChange(() => SelectedGeneration);
                NotifyOfPropertyChange(() => CanPublishLive);
                NotifyOfPropertyChange(() => CanPublishPreview);
            }
        }

        public bool CanPublishLive { get { return SelectedGeneration != null; } }
        public bool CanPublishPreview { get { return SelectedGeneration != null; } }

        public void PublishLive()
        {
            Publish(PublicationDataSubset.Live);
        }

        public void PublishPreview()
        {
            Publish(PublicationDataSubset.Preview);
        }

        private async void Publish(PublicationDataSubset publicationDataSubset)
        {
            var result = await PublicationService.Publish(SelectedGeneration.ID, SelectedEnvironment, Target, Brand, Country, publicationDataSubset);
            MessageBox.Show(result is Successfull ? "Success!" : "Failure!");
        }
    }
}