// Copyright (c) billwerk GmbH. All rights reserved

namespace PaymentGateway.Shared;

public static class EnumerableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var element in enumerable)
            action(element);
    }

    public static async Task ForEachAsync<T>(this IEnumerable<T> enumerable, Func<T, Task> action)
    {
        foreach (var element in enumerable)
            await action(element);
    }

    /// <example>[1,2,3,4], [1,2,3] -> [4]
    /// <br/>[9,8,7], [10,9,8] -> [9,8,7]
    /// <br/>[1], [2] -> [1]
    /// <br/>[1,2], [2] -> [1,2]</example>
    public static IEnumerable<T> SkipSubsequentEqualsAndTakeRemaining<T>(this IEnumerable<T> takeRemainingFrom, IEnumerable<T> toCompareWith)
    {
        using var initialEnumerator = takeRemainingFrom.GetEnumerator();
        using var toCompareEnumerator = toCompareWith.GetEnumerator();
        while (initialEnumerator.MoveNext())
        {
            if (toCompareEnumerator.MoveNext() && initialEnumerator.Current?.Equals(toCompareEnumerator.Current) == true) 
                continue;

            yield return initialEnumerator.Current;
            break;
        }
        
        while (initialEnumerator.MoveNext())
            yield return initialEnumerator.Current;
    }

    public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T>? enumerable) => enumerable ?? Enumerable.Empty<T>();

    public static IEnumerable<T> ExceptNulls<T>(this IEnumerable<T?> enumerable) => enumerable.Where(x => x is not null)!;
}
