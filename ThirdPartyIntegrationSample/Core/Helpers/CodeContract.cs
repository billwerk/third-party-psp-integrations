using System;
using System.Collections.Generic;
using System.Linq;
using Core.Exceptions;

namespace Core.Helpers
{
    public static class CodeContract
    {
        public static void Requires<TException>(bool condition, string userMessage) where TException : Exception
        {
            if (condition) 
                return;
            
            var errMessage = userMessage==null ? "":$" '{userMessage}'";
            if (Activator.CreateInstance(typeof(TException), "Contract Requires condition"+errMessage) is TException exception)
            {
                throw exception;
            }
        }

        public static void Requires(bool condition, string userMessage)
        {
            Requires<CodeContractException>(condition, userMessage);
        }

        public static void Assert(bool condition, string userMessage)
        {
            Requires(condition, userMessage);
        }

        /// <summary>Determines whether all the elements in a collection exist within a function.</summary>
        /// <param name="collection">The collection from which elements of type T will be drawn to pass to <paramref name="predicate" />.</param>
        /// <param name="predicate">The function to evaluate for the existence of all the elements in <paramref name="collection" />.</param>
        public static bool ForAll<T>(IEnumerable<T> collection, Predicate<T> predicate)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            
            return collection.All(obj => predicate(obj));
        }
    }
}