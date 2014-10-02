using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher;
using FakeItEasy;
using System.Collections.ObjectModel;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.Carconfigurator.Tests.Base
{
    public class PublicationTest : TestBase
    {
        IS3Service _service;
        S3Publisher _publisher;
        IContext _context;
        String _brand = "Toyota";
        String _country = "BE";
        String[] _languages = new[] { "nl", "fr", "de", "en" };

        IReadOnlyDictionary<String, IReadOnlyList<TimeFrame>> _timeFrames;

        protected override void Arrange()
        {
            _service = new TestImplementations.TestS3Service();
            var serialiser = A.Fake<IS3Serialiser>();
            _publisher = new S3Publisher(_service, serialiser);
            _context = CreateFakeContext();

            

            _timeFrames = new ReadOnlyDictionary<String, IReadOnlyList<TimeFrame>>(
                            _languages.ToDictionary(
                                language => language,
                                language => (IReadOnlyList<TimeFrame>)new ReadOnlyCollection<TimeFrame>(
                                    new List<TimeFrame> { }
                                )));
            
            A.CallTo(() => _context.Brand).Returns(_brand);
            A.CallTo(() => _context.Country).Returns(_country);
            A.CallTo(() => _context.TimeFrames).Returns(_timeFrames);
        }

        IContext CreateFakeContext()
        {
            var context = A.Fake<IContext>();

            var dataSets = _languages.ToDictionary(
                language => language,
                CreateFakeContextData
            );
            
            _timeFrames = new ReadOnlyDictionary<String, IReadOnlyList<TimeFrame>>(
                            _languages.ToDictionary(
                                language => language,
                                language => (IReadOnlyList<TimeFrame>)new ReadOnlyCollection<TimeFrame>(
                                    new List<TimeFrame> { }
                                )));

            A.CallTo(() => _context.Brand).Returns(_brand);
            A.CallTo(() => _context.Country).Returns(_country);
            A.CallTo(() => _context.TimeFrames).Returns(_timeFrames);

            var foo = dataSets["foo"];

            return context;
        }

        IContextData CreateFakeContextData(String language)
        {
            var data = A.Fake<IContextData>();

            var generations = new Repository<Generation>();
            generations.Add(CreateFakeGeneration(language));

            var cars = new Repository<Car>();
            cars.Add(new Car());

            A.CallTo(() => data.Generations).Returns(generations);
            A.CallTo(() => data.Cars).Returns(cars);

            return data;
        }

        Generation CreateFakeGeneration(String language)
        {
            return FillFakeBaseObject(new Generation
            {
                CarConfiguratorVersion = new CarConfiguratorVersion
                {
                    Name = "carConfigVersion-" + language
                },
                SSNs = new List<String> { "SSN1-" + language, "SSN2-" + language }
            }, language);
        }

        T FillFakeBaseObject<T>(T baseObject, String language) where T : BaseObject
        {
            var objectName = baseObject.GetType().Name;

            baseObject.Description = objectName + "Description-" + language;
            baseObject.FootNote = objectName + "generationFootNote-" + language;
            baseObject.InternalCode = objectName + "generationInternalCode-" + language;
            baseObject.LocalCode = objectName + "generationLocalCode-" + language;
            baseObject.Name = objectName + "generationName-" + language;
            baseObject.ToolTip = objectName + "generationToolTip-" + language;
            baseObject.Labels = new List<Label>
            {
                new Label {
                    Code = objectName + "Label1Code-" + language,
                    Value = objectName + "Label1Value-" + language
                },
                new Label {
                    Code = objectName + "Label2Code-" + language,
                    Value = objectName + "Label2Value-" + language
                }
            };

            return baseObject;
        }


        protected override void Act()
        {
            _publisher.Publish(_context);
        }
    }
}
