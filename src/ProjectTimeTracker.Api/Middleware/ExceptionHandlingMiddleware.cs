using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectTimeTracker.Api.Exceptions;
using ProjectTimeTracker.Api.Models;

namespace ProjectTimeTracker.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (EntityNotFoundException ex)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new ApiErrorResponse
            {
                Error = "NotFound",
                Message = ex.Message
            });
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new ApiErrorResponse
            {
                Error = "ValidationError",
                Message = ex.Message
            });
        }
        catch (DbUpdateException ex)
        {
            _logger.LogWarning(ex, "Errore di aggiornamento database.");

            context.Response.StatusCode = StatusCodes.Status409Conflict;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new ApiErrorResponse
            {
                Error = "DatabaseConflict",
                Message = "Operazione non completata per un conflitto di integrità dati."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore non gestito.");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Errore interno del server",
                Detail = "Si è verificato un errore non previsto."
            };

            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}