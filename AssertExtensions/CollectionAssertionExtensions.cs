﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Xunit.Extensions.AssertExtensions
{
    /// <summary>
    /// Extensions which provide assertions to classes derived from <see cref="IEnumerable"/> and <see cref="IEnumerable&lt;T&gt;"/>.
    /// </summary>
    public static class CollectionAssertExtensions
    {
        /// <summary>
        /// Verifies that a collection is empty.
        /// </summary>
        /// <param name="collection">The collection to be inspected</param>
        /// <exception cref="ArgumentNullException">Thrown when the collection is null</exception>
        /// <exception cref="EmptyException">Thrown when the collection is not empty</exception>
        public static void ShouldBeEmpty(this IEnumerable collection) =>
            Assert.Empty(collection);

        /// <summary>
        /// Verifies that a collection contains a given object.
        /// </summary>
        /// <typeparam name="T">The type of the object to be verified</typeparam>
        /// <param name="collection">The collection to be inspected</param>
        /// <param name="expected">The object expected to be in the collection</param>
        /// <exception cref="ContainsException">Thrown when the object is not present in the collection</exception>
        public static void ShouldContain<T>(this IEnumerable<T> collection, T expected) =>
            Assert.Contains(expected, collection);

        /// <summary>
        /// Verifies that a collection is not empty.
        /// </summary>
        /// <param name="collection">The collection to be inspected</param>
        /// <exception cref="ArgumentNullException">Thrown when a null collection is passed</exception>
        /// <exception cref="NotEmptyException">Thrown when the collection is empty</exception>
        public static void ShouldNotBeEmpty(this IEnumerable collection) =>
            Assert.NotEmpty(collection);

        /// <summary>
        /// Verifies that a collection does not contain a given object.
        /// </summary>
        /// <typeparam name="T">The type of the object to be compared</typeparam>
        /// <param name="expected">The object that is expected not to be in the collection</param>
        /// <param name="collection">The collection to be inspected</param>
        /// <exception cref="DoesNotContainException">Thrown when the object is present inside the container</exception>
        public static void ShouldNotContain<T>(this IEnumerable<T> collection, T expected) =>
            Assert.DoesNotContain(expected, collection);
    }
}
