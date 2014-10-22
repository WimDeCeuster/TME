using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Common.Result;
using TME.CarConfigurator.Publisher.Interfaces;

namespace TME.CarConfigurator.S3.Publisher
{
    public class GradePublisher : IGradePublisher
    {
        IGradeService _gradeService;

        public GradePublisher(IGradeService gradeService)
        {
            if (gradeService == null) throw new ArgumentNullException("gradeService");

            _gradeService = gradeService;
        }

        public async Task<IEnumerable<Result>> PublishGenerationGrades(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            var tasks = new List<Task<IEnumerable<Result>>>();

            foreach (var entry in context.ContextData)
            {
                var language = entry.Key;
                var data = entry.Value;
                var timeFrames = context.TimeFrames[language];

                tasks.Add(PutTimeFramesGenerationGrades(context.Brand, context.Country, timeFrames, data));
            }

            var result = await Task.WhenAll(tasks);
            return result.SelectMany(xs => xs);
        }

        async Task<IEnumerable<Result>> PutTimeFramesGenerationGrades(String brand, String country, IEnumerable<TimeFrame> timeFrames, ContextData data)
        {
            var publication = data.Publication;
            
            var grades = timeFrames.ToDictionary(
                                timeFrame => data.Publication.TimeFrames.Single(publicationTimeFrame => publicationTimeFrame.ID == timeFrame.ID),
                                timeFrame => data.Grades.Where(grade => timeFrame.Cars.Any(car => car.Grade.ID == grade.ID))
                                                         .OrderBy(grade => grade.SortIndex)
                                                         .ThenBy(grade => grade.Name)
                                                         .ToList());

            var tasks = new List<Task<Result>>();

            foreach (var entry in grades)
                tasks.Add(_gradeService.PutTimeFrameGenerationGrades(brand, country, publication.ID, entry.Key.ID, entry.Value));

            return await Task.WhenAll(tasks);
        }
    }
}
