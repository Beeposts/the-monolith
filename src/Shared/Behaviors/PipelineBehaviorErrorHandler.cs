using Ardalis.Result;

namespace Shared.Behaviors;

public abstract class PipelineBehaviorErrorHandler
{
    protected static TResult CreateFailureResult<TResult>(ValidationError[] errors)
        where TResult : IResult
    {

        if (typeof(TResult) == typeof(Result))
        {
            return (TResult) typeof(Result)
                .GetMethod(nameof(Result.Invalid))!
                .Invoke(null, [errors])!;
        }

        var validationResult = typeof(Result<>)
            .GetGenericTypeDefinition()
            .MakeGenericType(typeof(TResult).GenericTypeArguments[0])
            .GetMethods()
            .FirstOrDefault(m =>
                m.GetParameters()
                    .Any(p => p.ParameterType == typeof(ValidationError[])))!
            .Invoke(null, [errors])!;

        return (TResult) validationResult;
    }
}