using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Extensions;
using TME.CarConfigurator.Publisher.Interfaces;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class BaseMapper : IBaseMapper
    {
        ILabelMapper _labelMapper;

        public BaseMapper(ILabelMapper labelMapper)
        {
            if (labelMapper == null) throw new ArgumentNullException("labelMapper");

            _labelMapper = labelMapper;
        }

        public TBase MapLocalizableDefaults<TBase>(TBase baseObject, Administration.BaseObjects.LocalizeableBusinessBase localizableObject)
            where TBase : Repository.Objects.Core.BaseObject
        {
            baseObject.InternalCode = localizableObject.BaseCode;
            baseObject.LocalCode = localizableObject.LocalCode.DefaultIfEmpty(localizableObject.BaseCode);
            return baseObject;
        }

        public TBase MapTranslateableDefaults<TBase, TTranslateable>(
            TBase baseObject,
            TTranslateable translateableObject,
            String defaultName)
            where TBase : Repository.Objects.Core.BaseObject
            where TTranslateable : Administration.BaseObjects.TranslateableBusinessBase
        {
            baseObject.Description = translateableObject.Translation.Description;
            baseObject.FootNote = translateableObject.Translation.FootNote;
            baseObject.ID = translateableObject.ID;
            baseObject.Labels = translateableObject.Translation.Labels.Select(_labelMapper.MapLabel)
                                                                      .Where(label => !String.IsNullOrWhiteSpace(label.Value))
                                                                      .ToList();
            baseObject.Name = translateableObject.Translation.Name.DefaultIfEmpty(defaultName);
            baseObject.ToolTip = translateableObject.Translation.ToolTip;

            return baseObject;
        }

        public TBase MapSortDefaults<TBase>(TBase baseObject, Administration.BaseObjects.ISortedIndex sortableObject)
            where TBase : Repository.Objects.Core.BaseObject
        {
            baseObject.SortIndex = sortableObject.Index;

            return baseObject;
        }
        
        public TBase MapDefaults<TBase, TTranslateable>(
            TBase baseObject,
            Administration.BaseObjects.LocalizeableBusinessBase localizableObject,
            TTranslateable translateableObject,
            String defaultName)
            where TBase : Repository.Objects.Core.BaseObject
            where TTranslateable : Administration.BaseObjects.TranslateableBusinessBase
        {
            return MapLocalizableDefaults(MapTranslateableDefaults(baseObject, translateableObject, defaultName), localizableObject);
        }


        public TBase MapDefaultsWithSort<TBase, TTranslateableAndSortable>(
            TBase baseObject,
            Administration.BaseObjects.LocalizeableBusinessBase localizableObject,
            TTranslateableAndSortable translateableAndSortableObject,
            String defaultName)
            where TBase : Repository.Objects.Core.BaseObject
            where TTranslateableAndSortable : Administration.BaseObjects.TranslateableBusinessBase, Administration.BaseObjects.ISortedIndex
        {
            return MapSortDefaults(MapDefaults(baseObject, localizableObject, translateableAndSortableObject, defaultName), translateableAndSortableObject);
        }
    }
}
