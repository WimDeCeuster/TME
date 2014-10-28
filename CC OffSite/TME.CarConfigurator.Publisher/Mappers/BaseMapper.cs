using System;
using System.Linq;
using TME.CarConfigurator.Administration.BaseObjects;
using TME.CarConfigurator.Publisher.Extensions;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects.Core;

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

        public T MapTranslateableDefaults<T>(T baseObject, TranslateableBusinessBase translateableObject)
            where T : BaseObject
        {
            baseObject.Description = translateableObject.Translation.Description;
            baseObject.FootNote = translateableObject.Translation.FootNote;
            baseObject.ID = translateableObject.ID;
            baseObject.Labels = translateableObject.Translation.Labels.Select(_labelMapper.MapLabel)
                                                                      .Where(label => !String.IsNullOrWhiteSpace(label.Value))
                                                                      .ToList();
            baseObject.Name = translateableObject.Translation.Name.DefaultIfEmpty(translateableObject.BaseName);
            baseObject.ToolTip = translateableObject.Translation.ToolTip;

            return baseObject;
        }

        public T MapSortDefaults<T>(T baseObject, ISortedIndex sortableObject)
            where T : BaseObject
        {
            baseObject.SortIndex = sortableObject.Index;
            return baseObject;
        }

        public T MapDefaults<T>(T baseObject, LocalizeableBusinessBase localizableObject, TranslateableBusinessBase translateableObject)
            where T : BaseObject
        {
            return MapLocalizableDefaults(MapTranslateableDefaults(baseObject, translateableObject), localizableObject);
        }

        public T MapDefaultsWithSort<T, TTranslateableAndSortable>(T baseObject, LocalizeableBusinessBase localizableObject, TTranslateableAndSortable translateableAndSortableObject)
            where T : BaseObject
            where TTranslateableAndSortable : TranslateableBusinessBase, ISortedIndex
        {
            return MapSortDefaults(MapDefaults(baseObject, localizableObject, translateableAndSortableObject), translateableAndSortableObject);
        }
    }
}
