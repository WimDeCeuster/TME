using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Administration.BaseObjects;
using TME.CarConfigurator.Administration.Translations;
using TME.CarConfigurator.Publisher.Extensions;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects.Core;
using Label = TME.CarConfigurator.Repository.Objects.Core.Label;
using AdministrationLabel = TME.CarConfigurator.Administration.Translations.Label;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class BaseMapper : IBaseMapper
    {
        readonly ILabelMapper _labelMapper;

        public BaseMapper(ILabelMapper labelMapper)
        {
            if (labelMapper == null) throw new ArgumentNullException("labelMapper");

            _labelMapper = labelMapper;
        }

        public T MapLocalizableDefaults<T>(T baseObject, LocalizeableBusinessBase localizableObject)
            where T : BaseObject
        {
            baseObject.InternalCode = localizableObject.BaseCode;
            baseObject.LocalCode = localizableObject.LocalCode.DefaultIfEmpty(localizableObject.BaseCode);
            return baseObject;
        }

        public T MapTranslateableDefaults<T>(T baseObject, TranslateableBusinessBase translateableObject, params IEnumerable<AdministrationLabel>[] additionalLabelSources)
            where T : BaseObject
        {
            baseObject.ID = translateableObject.ID;

            return MapTranslation(baseObject, translateableObject.Translation, translateableObject.BaseName, additionalLabelSources);
        }

        public T MapSortDefaults<T>(T baseObject, ISortedIndex sortableObject)
            where T : BaseObject
        {
            baseObject.SortIndex = sortableObject.Index;
            return baseObject;
        }

        public T MapDefaults<T>(T baseObject, LocalizeableBusinessBase localizableObject, TranslateableBusinessBase translateableObject, params IEnumerable<AdministrationLabel>[] additionalLabelSources)
            where T : BaseObject
        {
            return MapLocalizableDefaults(MapTranslateableDefaults(baseObject, translateableObject, additionalLabelSources), localizableObject);
        }

        public T MapDefaultsWithSort<T, TTranslateableAndSortable>(T baseObject, LocalizeableBusinessBase localizableObject, TTranslateableAndSortable translateableAndSortableObject, params IEnumerable<AdministrationLabel>[] additionalLabelSources)
            where T : BaseObject
            where TTranslateableAndSortable : TranslateableBusinessBase, ISortedIndex
        {
            return MapSortDefaults(MapDefaults(baseObject, localizableObject, translateableAndSortableObject, additionalLabelSources), translateableAndSortableObject);
        }

        public T MapTranslateableDefaultsWithSort<T, TTranslateableAndSortable>(T baseObject, TTranslateableAndSortable translateableAndSortableObject, params IEnumerable<AdministrationLabel>[] additionalLabelSources)
            where T : BaseObject
            where TTranslateableAndSortable : TranslateableBusinessBase, ISortedIndex
        {
            return MapSortDefaults(MapTranslateableDefaults(baseObject, translateableAndSortableObject, additionalLabelSources), translateableAndSortableObject);
        }

        public T MapDefaults<T>(T baseObject, LocalizeableBusinessBase localizableObject, params IEnumerable<AdministrationLabel>[] additionalLabelSources) where T : BaseObject
        {
            return MapDefaults(baseObject, localizableObject, localizableObject, additionalLabelSources);
        }

        public T MapDefaultsWithSort<T, TLocalizeableAndSortable>(T baseObject, TLocalizeableAndSortable localizeableAndSortableObject, params IEnumerable<AdministrationLabel>[] additionalLabelSources)
            where T : BaseObject
            where TLocalizeableAndSortable : LocalizeableBusinessBase, ISortedIndex
        {
            return MapDefaultsWithSort(baseObject, localizeableAndSortableObject, localizeableAndSortableObject, additionalLabelSources);
        }

        public T MapTranslation<T>(T baseObject, Translation translation, string defaultName, params IEnumerable<AdministrationLabel>[] additionalLabelSources) where T : BaseObject
        {
            baseObject.Description = translation.Description;
            baseObject.FootNote = translation.FootNote;
            
            var labelSets = new[] { translation.Labels }.Concat(additionalLabelSources).ToArray();

            baseObject.Labels = _labelMapper.MapLabels(labelSets);
            baseObject.Name = translation.Name.DefaultIfEmpty(defaultName);
            baseObject.ToolTip = translation.ToolTip;

            return baseObject;
        }
    }
}
