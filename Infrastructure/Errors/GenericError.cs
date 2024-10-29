public class BadRequestException : Exception
{
	public BadRequestException(string message)
	{
		ErrorMessage = message;
	}
	public string ErrorMessage { get; set; }
}

public class ValidationException : Exception
{
	public ValidationException(string message)
	{
		ErrorMessage = message;
	}
	public string ErrorMessage { get; set; }
}

public class NotFoundException : Exception
{
	public NotFoundException(string message)
	{
		ErrorMessage = message;
	}
	public string ErrorMessage { get; set; }
}