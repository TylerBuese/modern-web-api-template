namespace template.DI;

public static class DependencyInjection
{
	public static IServiceCollection AddServices(this IServiceCollection services)
	{
		//Add services and other builder items here with services.AddScoped<ITempalteService, templateService>();

		services.AddHttpContextAccessor();

		services.AddExceptionHandler<NotFoundExceptionHandler>();
		services.AddExceptionHandler<ValidationExceptionHandler>();
		services.AddExceptionHandler<GlobalExceptionHandler>();

		return services;
	}


}