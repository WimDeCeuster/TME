using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace KellermanSoftware.CompareNetObjects.TypeComparers
{
    /// <summary>
    /// Compare two properties (Note inherits from BaseComparer instead of TypeComparer
    /// </summary>
    public class PropertyComparer : BaseComparer
    {
        private readonly RootComparer _rootComparer;
        private readonly IndexerComparer _indexerComparer;

        /// <summary>
        /// Constructor that takes a root comparer
        /// </summary>
        /// <param name="rootComparer"></param>
        public PropertyComparer(RootComparer rootComparer)
        {
            _rootComparer = rootComparer;
            _indexerComparer = new IndexerComparer(rootComparer);
        }

        /// <summary>
        /// Compare the properties of a class
        /// </summary>
        public void PerformCompareProperties(CompareParms parms)
        {
            IEnumerable<PropertyInfo> currentProperties = new List<PropertyInfo>();
            var foundMembers = false;

            //Interface Member Logic
            if (parms.Config.InterfaceMembers.Count > 0)
            {
                Type[] interfaces = parms.Object1Type.GetInterfaces();

                foreach (var type in interfaces)
                {
                    if (parms.Config.InterfaceMembers.Contains(type))
                    {
                        currentProperties = currentProperties.Concat(Cache.GetPropertyInfo(parms.Result, type)).ToList();
                        foundMembers = true;
                    }
                }
            }

            if (!foundMembers)
                currentProperties = Cache.GetPropertyInfo(parms.Result, parms.Object1Type);

            foreach (PropertyInfo info in currentProperties)
            {
                //If we can't read it, skip it
                if (info.CanRead == false)
                    continue;

                //Skip if this is a shallow compare
                if (!parms.Config.CompareChildren && TypeHelper.CanHaveChildren(info.PropertyType))
                    continue;

                //Skip if it should be excluded based on the configuration
                if (ExcludeLogic.ShouldExcludeMember(parms, info))
                    continue;    

                //If we should ignore read only, skip it
                if (!parms.Config.CompareReadOnly && info.CanWrite == false)
                    continue;

                //If we ignore types then we must get correct PropertyInfo object
                PropertyInfo secondObjectInfo = null;
                if (parms.Config.IgnoreObjectTypes)
                {
                    var secondObjectPropertyInfos = Cache.GetPropertyInfo(parms.Result, parms.Object2Type);

                    foreach (var propertyInfo in secondObjectPropertyInfos)
                    {
                        if (propertyInfo.Name != info.Name) continue;

                        secondObjectInfo = propertyInfo;
                        break;
                    }
                }
                else
                    secondObjectInfo = info;

                object objectValue1;
                object objectValue2;
                var hasException = false;
                if (!IsValidIndexer(parms.Config, info, parms.BreadCrumb))
                {
                    try
                    {
                        objectValue1 = info.GetValue(parms.Object1, null);
                    }
                    catch (Exception ex)
                    {
                        if (!parms.Config.AllowPropertyExceptions)
                            throw;
                        hasException = true;
                        objectValue1 = ex;
                    }

                    try
                    {
                        objectValue2 = secondObjectInfo != null ? secondObjectInfo.GetValue(parms.Object2, null) : null;
                    }
                    catch (Exception ex)
                    {
                        if (!parms.Config.AllowPropertyExceptions)
                            throw;
                        hasException = true;
                        objectValue2 = ex;
                    }
                }
                else
                {
                    _indexerComparer.CompareIndexer(parms, info);
                    continue;
                }

                bool object1IsParent = objectValue1 != null && (objectValue1 == parms.Object1 || parms.Result.Parents.ContainsKey(objectValue1.GetHashCode()));
                bool object2IsParent = objectValue2 != null && (objectValue2 == parms.Object2 || parms.Result.Parents.ContainsKey(objectValue2.GetHashCode()));

                //Skip properties where both point to the corresponding parent
                if ((TypeHelper.IsClass(info.PropertyType) || TypeHelper.IsInterface(info.PropertyType) || TypeHelper.IsStruct(info.PropertyType)) 
                    && (object1IsParent && object2IsParent))
                {
                    continue;
                }

                string currentBreadCrumb = AddBreadCrumb(parms.Config, parms.BreadCrumb, info.Name);

                CompareParms childParms = new CompareParms
                {
                    Result = parms.Result,
                    Config = parms.Config,
                    ParentObject1 = parms.Object1,
                    ParentObject2 = parms.Object2,
                    Object1 = objectValue1,
                    Object2 = objectValue2,
                    MemberPath = parms.MemberPath + "." + info.Name,
                    BreadCrumb = currentBreadCrumb,
                    Property = info,
                    ClassDepth = parms.ClassDepth
                };

                if (hasException)
                {
                    var ex1 = objectValue1 is Exception ? (objectValue1 as Exception).InnerException : null;
                    var ex2 = objectValue2 is Exception ? (objectValue2 as Exception).InnerException : null;


                    if (ex1 is NotImplementedException || ex2 is NotImplementedException)
                        this.AddDifference(childParms, null, DifferenceType.NotImplemented);
                    else
                        this.AddDifference(childParms, null, DifferenceType.Exception);
                }
                else
                {
                    var key = info.DeclaringType.FullName + "." + info.Name;
                    if (parms.Config.CustomPropertyComparers.ContainsKey(key))
                    {
                        var comparison = parms.Config.CustomPropertyComparers[key](objectValue1, objectValue2);

                        if (comparison == null)
                            _rootComparer.Compare(childParms);
                        else if (!comparison.Value)
                            this.AddDifference(childParms);
                    }
                    else
                    {
                        _rootComparer.Compare(childParms);
                    }
                }

                if (parms.Result.ExceededDifferences)
                    return;
            }
        }

        private bool IsValidIndexer(ComparisonConfig config, PropertyInfo info, string breadCrumb)
        {
            ParameterInfo[] indexers = info.GetIndexParameters();

            if (indexers.Length == 0)
            {
                return false;
            }

            if (indexers.Length > 1)
            {
                if (!config.SkipInvalidIndexers)
                    throw new Exception("Cannot compare objects with more than one indexer for object " + breadCrumb);
            }

            if (indexers[0].ParameterType != typeof(Int32))
            {
                if (!config.SkipInvalidIndexers)
                    throw new Exception("Cannot compare objects with a non integer indexer for object " + breadCrumb);
            }

            if (info.ReflectedType.GetProperty("Count") == null)
            {
                if (!config.SkipInvalidIndexers)
                    throw new Exception("Indexer must have a corresponding Count property for object " + breadCrumb);
            }

            if (info.ReflectedType.GetProperty("Count").PropertyType != typeof(Int32))
            {
                if (!config.SkipInvalidIndexers)
                    throw new Exception("Indexer must have a corresponding Count property that is an integer for object " + breadCrumb);
            }

            return true;
        }
    }
}
