// Copyright (c) billwerk GmbH. All rights reserved

namespace PaymentGateway.Shared;

public static class TaskExtensions
{
    /// <summary>
    /// Monadic Map over Task - projects wrapped value to another form in async context
    /// </summary>
    /// <param name="source">source task</param>
    /// <param name="map">projection</param>
    /// <typeparam name="T">initial type of wrapped value</typeparam>
    /// <typeparam name="R">final type of wrapped value</typeparam>
    /// <returns>new task wrapping projected value</returns>
    public static async Task<R> ToAsync<T, R>(this Task<T> source, Func<T, R> map)
        => map(await source);

    /// <summary>
    /// Map over Result in Task context - simplified form of converting success type without leaving async context
    /// </summary>
    /// <param name="source">task to perform operation on</param>
    /// <param name="map">projection for success state of result</param>
    /// <typeparam name="T">initial success type</typeparam>
    /// <typeparam name="E">error type</typeparam>
    /// <typeparam name="R">final success type</typeparam>
    /// <returns>new Task wrapping new Result with projected success value (or same error value)</returns>
    public static async Task<Result<R, E>> MapResultAsync<T, E, R>(this Task<Result<T, E>> source, Func<T, R> map) =>
        (await source).Map<R>(map);
}
