using System;
using System.Collections.Generic;
using Caliburn.Micro;
using TME.CarConfigurator.Administration;

namespace TME.CarConfigurator.Publisher.UI.ViewModels
{
    public class ShellViewModel : PropertyChangedBase
    {
        private string _country;
        private Model _selectedModel;
        private ModelGeneration _selectedGeneration;

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
                    MyContext.SetSystemContext("Toyota", _country, "en");

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
            }
        }

        public ShellViewModel()
        {
            Country = "DE";
        }

        public void PublishLive()
        {
            
        }

        public void PublishPreview()
        {
            
        }
    }
}