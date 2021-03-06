﻿using System;
using System.Collections.Generic;
using FakeItEasy;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Common.Interfaces;

using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Core;
using TME.CarConfigurator.Tests.Shared;
using System.Threading.Tasks;
using Context = TME.CarConfigurator.Publisher.Common.Context;
using TME.Carconfigurator.Tests.Builders;
using TME.CarConfigurator.Tests.Shared.TestBuilders;

namespace TME.Carconfigurator.Tests.GivenAS3Publisher
{
    public abstract class ActivatePublicationTestBase : TestBase
    {
        protected IPublicationPublisher PublicationPublisher;
        protected IModelPublisher PutModelPublisher;
        protected CarConfigurator.QueryServices.IModelService GetModelService;
        protected IBodyTypePublisher BodyTypePublisher;
        protected IEnginePublisher EnginePublisher;
        protected ICarPublisher CarPublisher;
        protected IAssetPublisher AssetPublisher;
        protected IPublisher Publisher;
        protected const string PublishedBy = "test";
        protected const string Brand = "Toyota";
        protected const string Country = "BE";
        protected const string Language1 = "nl";
        protected const string Language2 = "fr";
        protected IContext Context;
        protected readonly Guid GenerationID = Guid.NewGuid();
        protected readonly Guid ModelID = Guid.NewGuid();
        protected const string ModelNameForLanguage1 = "ModelNameForLanguage1";
        protected const string ModelNameForLanguage2 = "ModelNameForLanguage2";
        protected const string InternalCodeForLanguage1 = "InternalCode1";
        protected const string LocalCodeForLanguage1 = "LocalCode1";
        protected const string DescriptionForLanguage1 = "Description";
        protected const string FootNoteForLanguage1 = "FootNote";
        protected const string TooltipForLanguage1 = "ToolTip";
        protected const int SortIndexForLanguage1 = 4;
        protected List<Label> LabelsForLanguage1 = new List<Label>
        {
            new Label{Code = "New Code 1",Value = "new value 1"},
            new Label{Code = "New Code 2",Value = "new value 2"},
            new Label{Code = "New Code 3",Value = "new value 3"}
        };

        protected override void Arrange()
        {
            PublicationPublisher = A.Fake<IPublicationPublisher>(x => x.Strict());
            PutModelPublisher = A.Fake<IModelPublisher>(x => x.Strict());
            GetModelService = A.Fake<CarConfigurator.QueryServices.IModelService>(x => x.Strict());
            BodyTypePublisher = A.Fake<IBodyTypePublisher>(x => x.Strict());
            EnginePublisher = A.Fake<IEnginePublisher>(x => x.Strict());
            CarPublisher = A.Fake<ICarPublisher>(x => x.Strict());
            AssetPublisher = A.Fake<IAssetPublisher>(x => x.Strict());

            Context = new Context(Brand, Country, GenerationID, PublicationDataSubset.Live, String.Empty, string.Empty);

            var contextDataForLanguage1 = new ContextData();
            contextDataForLanguage1.Models.Add(new Model
            {
                Name = ModelNameForLanguage1,
                ID = ModelID,
                InternalCode = InternalCodeForLanguage1,
                LocalCode = LocalCodeForLanguage1,
                Description = DescriptionForLanguage1,
                FootNote = FootNoteForLanguage1,
                ToolTip = TooltipForLanguage1,
                SortIndex = SortIndexForLanguage1,
                Labels = LabelsForLanguage1
            });
            contextDataForLanguage1.Generations.Add(new Generation());

            var contextDataForLanguage2 = new ContextData();
            contextDataForLanguage2.Models.Add(new Model { Name = ModelNameForLanguage2, ID = ModelID });
            contextDataForLanguage2.Generations.Add(new Generation());

            Context.ContextData.Add(Language1, contextDataForLanguage1);
            Context.ContextData.Add(Language2, contextDataForLanguage2);

            var timeFrames = new List<TimeFrame> { new TimeFrameBuilder().WithDateRange(DateTime.MinValue, DateTime.MaxValue).Build() };

            Context.TimeFrames.Add(Language1, timeFrames);
            Context.TimeFrames.Add(Language2, timeFrames);

            var task = Task.Run(() => { });

            A.CallTo(() => PutModelPublisher.PublishModelsByLanguage(null, null)).WithAnyArguments().Returns(task);
            A.CallTo(() => PublicationPublisher.PublishPublicationsAsync(null)).WithAnyArguments().Returns(task);
            A.CallTo(() => BodyTypePublisher.PublishGenerationBodyTypesAsync(null)).WithAnyArguments().Returns(task);
            A.CallTo(() => EnginePublisher.PublishGenerationEnginesAsync(null)).WithAnyArguments().Returns(task);
            A.CallTo(() => CarPublisher.PublishGenerationCarsAsync(null)).WithAnyArguments().Returns(task);
            A.CallTo(() => AssetPublisher.PublishAsync(null)).WithAnyArguments().Returns(task);

            Publisher = new PublisherBuilder()
                .WithPublicationPublisher(PublicationPublisher)
                .WithModelPublisher(PutModelPublisher)
                .WithModelService(GetModelService)
                .WithBodyTypePublisher(BodyTypePublisher)
                .WithEnginePublisher(EnginePublisher)
                .WithCarPublisher(CarPublisher)
                .WithAssetPublisher(AssetPublisher)
                .Build();
        }

        protected override void Act()
        {
            Publisher.PublishAsync(Context).Wait();
        }

        protected Model GetModel(string modelName, string internalCode, string localCode, string oldDescriptionForLanguage1, string footNote, string tooltip, int sortIndex, List<Label> labels)
        {
            return new Model
            {
                Name = modelName,
                ID = ModelID,
                InternalCode = internalCode,
                LocalCode = localCode,
                Labels = labels,
                Publications =
                {
                    new PublicationInfo(new Publication{ID = Guid.NewGuid(),Generation = new Generation()})
                }
            };
        }
    }
}