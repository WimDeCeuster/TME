namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IBaseMapper
    {
        T MapLocalizableDefaults<T>(T baseObject, Administration.BaseObjects.LocalizeableBusinessBase localizableObject)
            where T : Repository.Objects.Core.BaseObject;
        T MapTranslateableDefaults<T>(T baseObject, Administration.BaseObjects.TranslateableBusinessBase translateableObject)
            where T : Repository.Objects.Core.BaseObject;
        T MapSortDefaults<T>(T baseObject, Administration.BaseObjects.ISortedIndex sortableObject)
            where T : Repository.Objects.Core.BaseObject;
        T MapDefaults<T>(T baseObject, Administration.BaseObjects.LocalizeableBusinessBase localizableObject, Administration.BaseObjects.TranslateableBusinessBase translateableObject)
            where T : Repository.Objects.Core.BaseObject;
        T MapDefaultsWithSort<T, TTranslateableAndSortable>(T baseObject, Administration.BaseObjects.LocalizeableBusinessBase localizableObject, TTranslateableAndSortable translateableAndSortableObject)
            where T : Repository.Objects.Core.BaseObject
            where TTranslateableAndSortable : Administration.BaseObjects.TranslateableBusinessBase, Administration.BaseObjects.ISortedIndex;
    }
}
