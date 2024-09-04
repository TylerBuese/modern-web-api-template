public class GenericError : Exception
{
	public GenericError(string message)
	{
		ErrorMessage = message;
	}
	public string ErrorMessage { get; set; }
}