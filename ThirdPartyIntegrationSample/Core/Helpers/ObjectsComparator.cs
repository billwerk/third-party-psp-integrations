using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace Core.Helpers
{
    public static class ObjectsComparator
    {
         public static bool AreObjectsEqual(object objectA, object objectB, params string[] ignoreList)
        {
            bool result;
            if (objectA != null && objectB != null)
            {
                var objectType = objectA.GetType();

                result = true; // assume by default they are equal

                foreach (var propertyInfo in objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead && !ignoreList.Contains(p.Name)))
                {
                    var valueA = propertyInfo.GetValue(objectA, null);
                    var valueB = propertyInfo.GetValue(objectB, null);

                    // if it is a primitive type, value type or implements IComparable, just directly try and compare the value
                    if (CanDirectlyCompare(propertyInfo.PropertyType))
                    {
                        if (!AreValuesEqual(valueA, valueB))
                        {
                            result = false;
                        }
                    }
                    // if it implements IEnumerable, then scan any items
                    else if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        // null check
                        if (valueA == null && valueB != null || valueA != null && valueB == null)
                        {
                            result = false;
                        }
                        else if (valueA != null)
                        {
                            var collectionItems1 = ((IEnumerable)valueA).Cast<object>();
                            var collectionItems2 = ((IEnumerable)valueB).Cast<object>();
                            var collectionItemsCount1 = collectionItems1.Count();
                            var collectionItemsCount2 = collectionItems2.Count();

                            // check the counts to ensure they match
                            if (collectionItemsCount1 != collectionItemsCount2)
                            {
                                result = false;
                            }
                            // and if they do, compare each item... this assumes both collections have the same order
                            else
                            {
                                for (var i = 0; i < collectionItemsCount1; i++)
                                {
                                    var collectionItem1 = collectionItems1.ElementAt(i);
                                    var collectionItem2 = collectionItems2.ElementAt(i);
                                    var collectionItemType = collectionItem1.GetType();

                                    if (CanDirectlyCompare(collectionItemType))
                                    {
                                        if (!AreValuesEqual(collectionItem1, collectionItem2))
                                        {
                                            result = false;
                                        }
                                    }
                                    else if (!AreObjectsEqual(collectionItem1, collectionItem2, ignoreList))
                                    {
                                        result = false;
                                    }
                                }
                            }
                        }
                    }
                    else if (propertyInfo.PropertyType.IsClass)
                    {
                        if (!AreObjectsEqual(propertyInfo.GetValue(objectA, null), propertyInfo.GetValue(objectB, null), ignoreList))
                        {
                            result = false;
                        }
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            else
            {
                result = Equals(objectA, objectB);
            }

            return result;
        }

        private static bool CanDirectlyCompare(Type type)
        {
            return typeof(IComparable).IsAssignableFrom((Type?) type) || type.IsPrimitive || type.IsValueType;
        }

        private static bool AreValuesEqual(object valueA, object valueB)
        {
            bool result;

            if (valueA == null && valueB != null || valueA != null && valueB == null)
            {
                result = false; // one of the values is null
            }
            else if (valueA is IComparable selfValueComparer && selfValueComparer.CompareTo(valueB) != 0)
            {
                result = false; // the comparison using IComparable failed
            }
            else if (!Equals(valueA, valueB))
            {
                result = false; // the comparison using Equals failed
            }
            else
            {
                result = true; // match
            }

            return result;
        }
    }
}