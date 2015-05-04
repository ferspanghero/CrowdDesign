using System;
using System.Collections.Generic;
using System.Linq;

namespace CrowdDesign.Utils.Extensions
{
    /// <summary>
    /// Represents a collection of extension methods for System.Object.
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Throws a System.ArgumentNullException if the object is null.
        /// </summary>
        /// <typeparam name="T">The type of the object to be checked.</typeparam>
        /// <param name="obj">The object to be checked.</param>
        /// <param name="objName">The name of the object.</param>
        public static void TryThrowArgumentNullException<T>(this T obj, string objName)
        {
            if (obj == null)
                throw new ArgumentNullException(objName);
        }

        /// <summary>
        /// Checks if a collection of System.Collections.Generic.IEnumerable is null or has no elements.
        /// </summary>
        /// <typeparam name="T">The type of the collection to be checked.</typeparam>
        /// <param name="enumerable">The collection to be checked.</param>
        /// <returns>Returns true if the collection is null or has no elements.</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return
                enumerable == null || !enumerable.Any();
        }
    }
}
