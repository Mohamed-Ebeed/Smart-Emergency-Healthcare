using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SmartEmergency.API.Extensions;

// بيحوّل استثناءات الـ Application Services لـ HTTP status مفهوم (ProblemDetails).
// أي استثناء تاني بيعدّي للـ GlobalExceptionMiddleware ويطلع 500.
// بنستخدمه على الـ Controller بتاعنا بـ [TypeFilter(typeof(ApiExceptionFilter))]
// علشان منعدّلش Program.cs ولا الـ Middleware المشترك.
public sealed class ApiExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var (status, title) = context.Exception switch
        {
            KeyNotFoundException         => (StatusCodes.Status404NotFound, "Resource not found."),
            ValidationException          => (StatusCodes.Status400BadRequest, "Validation failed."),
            ArgumentException            => (StatusCodes.Status400BadRequest, "Invalid request."),
            InvalidOperationException    => (StatusCodes.Status409Conflict, "Business rule violated."),
            UnauthorizedAccessException  => (StatusCodes.Status403Forbidden, "Access denied."),
            _                            => (0, string.Empty)
        };

        if (status == 0) return; // مش مسؤوليتنا، يكمّل للـ Middleware

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = context.Exception.Message,
            Instance = context.HttpContext.Request.Path
        };
        problem.Extensions["traceId"] = context.HttpContext.TraceIdentifier;

        context.Result = new ObjectResult(problem) { StatusCode = status };
        context.ExceptionHandled = true;
    }
}
