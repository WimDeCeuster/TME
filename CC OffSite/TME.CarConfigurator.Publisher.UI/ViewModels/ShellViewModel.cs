using System;
using System.Collections.Generic;
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
        private const string Environment = "Development";

        private string _country = "DE";
        private Model _selectedModel;
        private ModelGeneration _selectedGeneration;
        private static ICarConfiguratorPublisher _carConfiguratorPublisher;

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

        public async Task PublishLive()
        {
            var result = await Publish(PublicationDataSubset.Live);

            DisplayResult(result);
        }

        public async Task PublishPreview()
        {
            var result = await Publish(PublicationDataSubset.Preview);

            DisplayResult(result);
        }

        private async Task<Result> Publish(PublicationDataSubset publicationDataSubset)
        {
            return await PublicationService.Publish(SelectedGeneration.ID, Environment, Target, Brand, Country, publicationDataSubset);
        }

        private static void DisplayResult(Result result)
        {
            MessageBox.Show(result is Successfull ? "Success!" : string.Format("Failure! {0}", ((Failed)result).Reason));
        }
    }
}