
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace SmartEmergency.API.Extensions;

public sealed class ApiExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;

        var (status, title, detail) = exception switch
        {
            DbUpdateException dbException
                when IsUniqueConstraintViolation(dbException)
                => (
                    StatusCodes.Status409Conflict,
                    "Data conflict.",
                    "A conflicting record already exists. Please refresh and try again."
                ),

            KeyNotFoundException
                => (
                    StatusCodes.Status404NotFound,
                    "Resource not found.",
                    exception.Message
                ),

            ValidationException
                => (
                    StatusCodes.Status400BadRequest,
                    "Validation failed.",
                    exception.Message
                ),

            ArgumentException
                => (
                    StatusCodes.Status400BadRequest,
                    "Invalid request.",
                    exception.Message
                ),

            InvalidOperationException
                => (
                    StatusCodes.Status409Conflict,
                    "Business rule violated.",
                    exception.Message
                ),

            UnauthorizedAccessException
                => (
                    StatusCodes.Status403Forbidden,
                    "Access denied.",
                    exception.Message
                ),

            _ => (0, string.Empty, string.Empty)
        };

        if (status == 0)
            return;

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Instance = context.HttpContext.Request.Path
        };

        problem.Extensions["traceId"] =
            context.HttpContext.TraceIdentifier;

        context.Result = new ObjectResult(problem)
        {
            StatusCode = status
        };

        context.ExceptionHandled = true;
    }

    private static bool IsUniqueConstraintViolation(
        DbUpdateException exception)
    {
        var current = exception.InnerException;

        while (current is not null)
        {
            if (current is SqlException sqlException &&
                (sqlException.Number == 2601 ||
                 sqlException.Number == 2627))
            {
                return true;
            }

            current = current.InnerException;
        }

        return false;
    }
}
