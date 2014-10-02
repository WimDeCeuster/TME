using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects;
using DBCar = TME.CarConfigurator.Administration.Car;
using ModelGeneration = TME.CarConfigurator.Administration.ModelGeneration;

namespace TME.CarConfigurator.Publisher
{
    public interface IContext
    {
        String Brand { get; }
        String Country { get; }
        IReadOnlyDictionary<String, IReadOnlyList<TimeFrame>> TimeFrames { get; }
        IReadOnlyDictionary<String, ModelGeneration> ModelGenerations { get; }
        IReadOnlyDictionary<String, IContextData> ContextData { get; }
    }

    public class Context : IContext
    {
        public IReadOnlyDictionary<String, ModelGeneration> ModelGenerations { get; private set; }
        public IReadOnlyDictionary<String, IContextData> ContextData { get; private set; }
        public IReadOnlyDictionary<String, IReadOnlyList<TimeFrame>> TimeFrames { get; private set; }

        public readonly PublicationDataSubset DataSubset;

        public String Brand { get; private set; }
        public String Country { get; private set; }

        public Context(String brand, String country, Guid generationID, ICarDbModelGenerationFinder generationFinder, IMapper mapper, PublicationDataSubset dataSubset)
        {
            DataSubset = dataSubset;
            Brand = brand;
            Country = country;

            ModelGenerations = generationFinder.GetModelGeneration(brand, country, generationID);

            ContextData = new ReadOnlyDictionary<String, IContextData>(
                            ModelGenerations.ToDictionary(
                                entry => entry.Key,
                                entry => mapper.Map(entry.Value, new ContextData())));

            TimeFrames = new ReadOnlyDictionary<String, IReadOnlyList<TimeFrame>>(
                            ModelGenerations.ToDictionary(
                                entry => entry.Key,
                                entry => GetTimeFrames(entry.Value, ContextData[entry.Key].Cars)));
        }

        IReadOnlyList<TimeFrame> GetTimeFrames(ModelGeneration generation, IRepository<Car> cars)
        {
            var TimeFrames = new List<TimeFrame>();

            var timeProjection = generation.Cars.SelectMany(car => new [] {
                                                    new { Date = car.LineOffFromDate, Open = true, Car = car },
                                                    new { Date = car.LineOffToDate, Open = false, Car = car }
                                                })
                                                .OrderBy(point => point.Date);

            Func<DBCar, Car> MapCar = dbCar => cars.Single(car => car.ID == dbCar.ID);

            var openCars = new List<DBCar>();
            DateTime? openDate = null;
            DateTime closeDate;
            foreach (var point in timeProjection)
            {
                if (point.Open)
                {
                    if (openDate != null)
                        closeDate = point.Date;

                    openCars.Add(point.Car);
                    openDate = point.Date;
                }
                else
                {
                    closeDate = point.Date;

                    // time lines with identical from/until can occur when multiple line off dates fall on the same point
                    // these "empty" time lines can simply be ignored (though the openCars logic is still relevant)
                    if (openDate != closeDate)
                        TimeFrames.Add(new TimeFrame(openDate.Value, closeDate, new ReadOnlyCollection<Car>(openCars.Select(MapCar).ToList())));

                    openCars.Remove(point.Car);
                    openDate = openCars.Any() ? (DateTime?)point.Date : null;
                }
            }

            return new ReadOnlyCollection<TimeFrame>(TimeFrames);
        }

    }
}
