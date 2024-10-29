using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;

// Use for execptaitons
public class GlobalExceptionHandler : IExceptionHandler
{
	public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
	{
		var problem = new ProblemDetails
		{
			Status = StatusCodes.Status500InternalServerError,
			Title = "Server Error",
			Type = "",
			Detail = exception.Message,
		};

		httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

		await httpContext.Response.WriteAsJsonAsync(problem);

		return true;
	}
}

public class NotFoundExceptionHandler : IExceptionHandler
{
	public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception,
		CancellationToken cancellationToken)
	{
		if (exception is NotFoundException e)
		{
			var problem = new ProblemDetails
			{
				Status = StatusCodes.Status404NotFound,
				Title = "Not Found",
				Type = "",
				Detail = e.ErrorMessage,
			};
			context.Response.StatusCode = StatusCodes.Status400BadRequest;
			await context.Response.WriteAsJsonAsync(problem, cancellationToken);

			return true;
		}

		return false;
	}
}

public class ValidationExceptionHandler : IExceptionHandler
{
	public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception,
		CancellationToken cancellationToken)
	{
		if (exception is ValidationException e)
		{
			var problem = new ProblemDetails
			{
				Status = StatusCodes.Status400BadRequest,
				Title = "Bad Request",
				Type = "",
				Detail = e.ErrorMessage,
			};
			context.Response.StatusCode = StatusCodes.Status400BadRequest;
			await context.Response.WriteAsJsonAsync(problem, cancellationToken);

			return true;
		}

		return false;
	}
}