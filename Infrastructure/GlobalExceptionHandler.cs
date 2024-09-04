using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;

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

public class GenericErrorHandler : IExceptionHandler
{
	public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception,
		CancellationToken cancellationToken)
	{
		if (exception is GenericError validationException)
		{
			var problem = new ProblemDetails
			{
				Status = StatusCodes.Status400BadRequest,
				Title = "Error",
				Type = "",
				Detail = validationException.ErrorMessage,
			};
			context.Response.StatusCode = StatusCodes.Status400BadRequest;
			await context.Response.WriteAsJsonAsync(problem, cancellationToken);

			return true;
		}

		return false;
	}
}