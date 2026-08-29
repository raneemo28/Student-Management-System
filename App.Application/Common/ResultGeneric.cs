namespace App.Application.Common;

public class Result<T> : Result
{
    public T? Value { get; }

    protected Result(bool isSuccess, T? value, string error) : base(isSuccess, error)
    {
        Value = value;
    }

    public static Result<T> Success(T value)
    {
        return new Result<T>(true, value, string.Empty);
    }

    public static Result<T> Failure(string error)
    {
        return new Result<T>(false, default, error);
    }
}
