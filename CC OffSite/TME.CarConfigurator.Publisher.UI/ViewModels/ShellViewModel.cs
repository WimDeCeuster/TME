using System;
using System.Collections.Generic;
using Caliburn.Micro;
using TME.CarConfigurator.Administration;

namespace TME.CarConfigurator.Publisher.UI.ViewModels
{
    public class ShellViewModel : PropertyChangedBase
    {
        private const string Brand = "Toyota";
        private const string Target = "S3";

        private string _country = "DE";
        private Model _selectedModel;
        private ModelGeneration _selectedGeneration;
        private static IPublicationService _publicationService;

        public IPublicationService PublicationService
        {
            get { return _publicationService; }
            set { _publicationService = value; }
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
            get { return SelectedModel == null ? new List<ModelGeneration>() : (IEnumerable<ModelGeneration>)SelectedModel.Generations; }
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

        private void Publish(PublicationDataSubset publicationDataSubset)
        {
            PublicationService.Publish(SelectedGeneration.ID, Target, Brand, Country, publicationDataSubset);
        }
    }
}