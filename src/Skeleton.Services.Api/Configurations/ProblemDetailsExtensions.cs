using Microsoft.AspNetCore.Mvc;

namespace Skeleton.Services.Api.Configurations
{
    public static class ProblemDetailsExtensions
    {
        public static void AddProblemDetailsModelStateConfiguration(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Type = null,
                        Title = "Ocorreu um ou mais erros de validação.",
                        Status = StatusCodes.Status400BadRequest,
                        Detail = null,
                        Instance = $"{context.HttpContext.Request.Method.ToLower()} {context.HttpContext.Request.Path}"
                    };

                    return new BadRequestObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json", "application/problem+xml" }
                    };
                };
            });
        }
    }
}