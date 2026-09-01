using Innovation.Services.Errors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Innovation.Api.Middleware;

// Maps typed service exceptions to real HTTP status codes + RFC 7807
// ProblemDetails, replacing the original ErrorsController which always
// returned 500 regardless of ErrorType (Backend ROADMAP §5.5/§13) and the
// `catch(ex){throw ex;}` pattern that destroyed stack traces.
public static class ProblemDetailsExceptionMapper
{
    public static void Register(WebApplication app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                var (status, title, type) = Classify(exception);

                context.Response.StatusCode = status;
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = status,
                    Title = title,
                    Type = type,
                    Detail = exception?.Message,
                    Instance = context.Request.Path,
                });
            });
        });
    }

    private static (int Status, string Title, string Type) Classify(Exception? exception) => exception switch
    {
        InvalidCredentialsException => (StatusCodes.Status401Unauthorized, "Invalid credentials", "https://errors.innovation-mes.local/invalid-credentials"),
        BarcodeNotFoundException => (StatusCodes.Status404NotFound, "Barcode not found", "https://errors.innovation-mes.local/barcode-not-found"),
        RmBalNotFoundException => (StatusCodes.Status404NotFound, "RM balance not found", "https://errors.innovation-mes.local/rm-bal-not-found"),
        SettingNotFoundException => (StatusCodes.Status404NotFound, "Required setting not found", "https://errors.innovation-mes.local/setting-not-found"),
        TotalWeightAlreadyExistsException => (StatusCodes.Status409Conflict, "Total weight already saved", "https://errors.innovation-mes.local/total-weight-exists"),
        StepNotAcceptedException => (StatusCodes.Status409Conflict, "Step weight not submitted", "https://errors.innovation-mes.local/step-not-accepted"),
        _ => (StatusCodes.Status500InternalServerError, "Unexpected error", "https://errors.innovation-mes.local/unexpected"),
    };
}
