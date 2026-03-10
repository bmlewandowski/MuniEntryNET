using FluentValidation;
using IValidationContext = FluentValidation.IValidationContext;

namespace Munientry.Api.Validation;

/// <summary>
/// Endpoint filter applied to the /api/v1 route group.
/// Resolves any registered <see cref="IValidator{T}"/> for each POST body argument and returns a
/// 422 ValidationProblem if validation fails — mirroring the Python required_check hard-stops.
/// </summary>
public sealed class FluentValidationFilter : IEndpointFilter
{
    private readonly IServiceProvider _services;

    public FluentValidationFilter(IServiceProvider services) => _services = services;

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext ctx,
        EndpointFilterDelegate next)
    {
        foreach (var arg in ctx.Arguments)
        {
            if (arg is null) continue;

            var validatorInterface = typeof(IValidator<>).MakeGenericType(arg.GetType());
            if (_services.GetService(validatorInterface) is not IValidator validator) continue;

            // Build a non-generic ValidationContext<T> at runtime so the correct rule sets run.
            var contextType = typeof(ValidationContext<>).MakeGenericType(arg.GetType());
            var validationContext = (IValidationContext)Activator.CreateInstance(contextType, arg)!;

            var result = await validator.ValidateAsync(
                validationContext, ctx.HttpContext.RequestAborted);

            if (!result.IsValid)
                return Results.ValidationProblem(
                    result.ToDictionary(),
                    statusCode: StatusCodes.Status422UnprocessableEntity);
        }

        return await next(ctx);
    }
}
