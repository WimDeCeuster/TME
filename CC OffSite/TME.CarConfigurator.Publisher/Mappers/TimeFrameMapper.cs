using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Exceptions;
using TME.CarConfigurator.Publisher.Extensions;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Colours;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.Repository.Objects.Packs;
using BodyType = TME.CarConfigurator.Repository.Objects.BodyType;
using Car = TME.CarConfigurator.Repository.Objects.Car;
using Engine = TME.CarConfigurator.Repository.Objects.Engine;
using EquipmentItem = TME.CarConfigurator.Repository.Objects.Equipment.EquipmentItem;
using Steering = TME.CarConfigurator.Repository.Objects.Steering;
using Transmission = TME.CarConfigurator.Repository.Objects.Transmission;
using WheelDrive = TME.CarConfigurator.Repository.Objects.WheelDrive;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class TimeFrameMapper : ITimeFrameMapper
    {
        public IReadOnlyList<TimeFrame> GetTimeFrames(String language, IContext context)
        {
            var contextData = context.ContextData[language];

            //For preview, return only 1 Min/Max TimeFrame with all data
            if (context.DataSubset == PublicationDataSubset.Preview)
                return new List<TimeFrame> { GetPreviewTimeFrame(contextData, context.ModelGenerations[language].Cars) };

            var timeFrames = new List<TimeFrame>();

            var timeProjection = context.ModelGenerations[language]
                                        .Cars
                                        .Where(car => car.Approved)
                                        .SelectMany(car => new[] {
                                            new { Date = car.LineOffFromDate, Open = true, Car = car },
                                            new { Date = car.LineOffToDate, Open = false, Car = car }
                                        })
                                        .OrderBy(point => point.Date);

            var openCars = new List<Administration.Car>();

            DateTime? openDate = null;
            foreach (var point in timeProjection)
            {
                DateTime closeDate;
                if (point.Open)
                {
                    if (openDate != null)
                    {
                        closeDate = point.Date;
                        AddTimeFrameIfRelevant(openDate, closeDate, timeFrames, openCars, context.ContextData[language]);
                    }

                    openCars.Add(point.Car);
                    openDate = point.Date;

                    continue;
                }

                closeDate = point.Date;

                AddTimeFrameIfRelevant(openDate, closeDate, timeFrames, openCars, context.ContextData[language]);

                openCars.Remove(point.Car);
                openDate = openCars.Any() ? (DateTime?)point.Date : null;
            }

            return timeFrames;
        }

        static void AddTimeFrameIfRelevant(DateTime? openDate, DateTime closeDate, ICollection<TimeFrame> timeFrames, IReadOnlyList<Administration.Car> timeFrameCars, ContextData contextData)
        {
            // time lines with identical from/until can occur when multiple line off dates fall on the same point
            // these "empty" time lines can simply be ignored (though the openCars logic is still relevant)
            if (openDate == closeDate) return;

            if (openDate == null)
                throw new CorruptDataException("The open date could not be retrieved, could not create timeframe");

            timeFrames.Add(new TimeFrame(openDate.Value, closeDate, timeFrameCars.Select(car => car.ID)));
        }

        static TimeFrame GetPreviewTimeFrame(ContextData contextData, IEnumerable<Administration.Car> timeFrameCars)
        {
            return new TimeFrame(DateTime.MinValue, DateTime.MaxValue, timeFrameCars.Select(car => car.ID));
        }
    }
}
