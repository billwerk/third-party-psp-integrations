namespace PaymentGateway.Shared;

public class VerificationResult<TError>
{
    protected VerificationResult()
    {
    }

    private VerificationResult(TError error)
    {
        //CodeContract.Requires<ArgumentException>(!IsError(error), $"Invalid state! IsFailure == false.");

        InternalError = error;
    }

    public static VerificationResult<TError> Ok() => new();

    public static VerificationResult<TError> Failure(TError error) => new(error);

    protected TError InternalError { get; set; }

    public TError Error => InternalError;

    public bool IsSuccess => IsError(Error);

    public bool IsFailure => !IsSuccess;

    private static bool IsError(TError data) => EqualityComparer<TError>.Default.Equals(data, default);
}

public class Result<TData, TError> : VerificationResult<TError>
{
    public Result(TData data, TError error)
    {
        // CodeContract.Requires<ArgumentException>(
        //     !EqualityComparer<TData>.Default.Equals(data, default) ||
        //     !EqualityComparer<TError>.Default.Equals(error, default),
        //     "!EqualityComparer<TError>.Default.Equals(data, default) || !EqualityComparer<TError>.Default.Equals(error, default)");

        Data = data;
        InternalError = error;
    }

    public static Result<TData, TError> Ok(TData data)
    {
        // CodeContract.Requires<ArgumentException>(!EqualityComparer<TData>.Default.Equals(data, default),
        //     "!EqualityComparer<TData>.Default.Equals(data, default)");

        return new Result<TData, TError>(data, default);
    }

    public static Result<TData, TError> Failure(TError error)
    {
        // CodeContract.Requires<ArgumentException>(!EqualityComparer<TError>.Default.Equals(error, default),
        //     "!EqualityComparer<TData>.Default.Equals(error, default)");

        return new Result<TData, TError>(default, error);
    }

    public TData Data { get; }

    /// <summary>
    /// Produce Result object with new type of Data - T, if current typeof(Data) can be converted to T.
    /// </summary>
    /// <typeparam name="T">new Data type</typeparam>
    public Result<T, TError> Wrap<T>()
    {
        if (IsSuccess && Data is T resultData)
            return Result<T, TError>.Ok(resultData);

        if (IsFailure)
            return Result<T, TError>.Failure(Error);

        throw new NotSupportedException($"Not supported state of result for making wrap! Result: {this}");
    }

    /// <summary>
    /// project values into new types within Result context
    /// </summary>
    /// <param name="data">data projection</param>
    /// <param name="error">error projection</param>
    /// <typeparam name="TR">new type of data</typeparam>
    /// <typeparam name="TL">new type of error</typeparam>
    /// <returns>updated Result</returns>
    public Result<TR, TL> BiMap<TR, TL>(Func<TData, TR> data, Func<TError, TL> error) => IsSuccess
        ? Result<TR, TL>.Ok(data(Data))
        : Result<TR, TL>.Failure(error(Error));

    /// <summary>
    /// project values into new types within Result context
    /// </summary>
    /// <param name="data">data projection</param>
    /// <typeparam name="TR">new type of data</typeparam>
    /// <returns>updated Result</returns>
    public Result<TR, TError> Map<TR>(Func<TData, TR> map) => BiMap(map, e => e);

    /// <summary>
    /// perform action with side-effects in Result context
    /// </summary>
    /// <param name="data">action to perform for Result in Data state</param>
    /// <param name="error">action to perform for Result in Error state</param>
    public void Do(Action<TData> data, Action<TError> error)
    {
        if (IsSuccess) data(Data);
        else error(Error);
    }

    /// <summary>
    /// simplify instantiation of context in Data state
    /// </summary>
    /// <param name="data">value to wrap</param>
    /// <returns></returns>
    public static implicit operator Result<TData, TError>(TData data) => Ok(data);

    /// <summary>
    /// simplify instantiation of context in Error state
    /// </summary>
    /// <param name="error">error to wrap</param>
    /// <returns></returns>
    public static implicit operator Result<TData, TError>(TError error) => Failure(error);

    /// <summary>
    /// project Data state to new type within Result context
    /// </summary>
    /// <param name="binder">projection returning context</param>
    /// <typeparam name="TR">new Data type</typeparam>
    /// <returns>Result context with updated Data type</returns>
    public Result<TR, TError> Bind<TR>(Func<TData, Result<TR, TError>> binder) =>
        IsSuccess ? binder(Data) : Result<TR, TError>.Failure(Error);
        
    public async Task<Result<TR, TError>> BindAsync<TR>(Func<TData, Task<Result<TR, TError>>> binder) =>
        await (IsSuccess ? binder(Data) : Task.FromResult(Result<TR, TError>.Failure(Error)));
}

public static class Result
{
    public static Result<TData, TError> OnSuccess<TData, TError>(this Result<TData, TError> source, Action<TData> action)
    {
        if (source.IsSuccess) action(source.Data);
        return source;
    }
    
    public static Result<TData, TError> OnError<TData, TError>(this Result<TData, TError> source, Action<TError> action)
    {
        if (source.IsFailure) action(source.Error);
        return source;
    }

    public static async Task<Result<TData, TError>> OnSuccessAsync<TData, TError>(this Result<TData, TError> source, Func<TData, Task> action)
    {
        if (source.IsSuccess) await action(source.Data);
        return source;
    }
    
    public static async Task<Result<TData, TError>> FinallyAsync<TData, TError>(this Result<TData, TError> source, Task task)
    {
        await task;
        return source;
    }

    public static async Task<Result<TData, TError>> OnErrorAsync<TData, TError>(this Result<TData, TError> source, Func<Task> action)
    {
        if (source.IsFailure) await action();
        return source;
    }
}

public class Result<TData>
{
    public TData Data { get; set; }

    public bool IsSuccess { get; set; }

    //Near - syntax sugar for Result<TData, TError>
    //It useful, because in 90% cases TError == string
    //But C# generics can not apply it.

    public static Result<TData, string> Ok(TData data) => Result<TData, string>.Ok(data);

    public static Result<TData, string> Failure(string error) => Result<TData, string>.Failure(error);
}
