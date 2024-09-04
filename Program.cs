using System.Text.Json.Serialization;
using template.DI;
using template.Models;
using template.Service;
using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Serilog.Sinks.PostgreSQL;
using NpgsqlTypes;
using Serilog;
using template.Errors;

var CorsName = "CorsAllow";

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
	options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddCors(options =>
{
	options.AddPolicy(name: CorsName,
	  builder => builder
	  .AllowAnyOrigin() //If you want a specific origin (recommended), then use .WithOrigin(<OriginAddress>)
	  .AllowAnyMethod()
	  //   .AllowCredentials()
	  .AllowAnyHeader()
	  .Build()
   );
});

builder.Services.AddApiVersioning(options =>
	{
		options.AssumeDefaultVersionWhenUnspecified = true;
		options.DefaultApiVersion = new ApiVersion(1, 0); //same as ApiVersion.Default
		options.ReportApiVersions = true;
		options.ApiVersionReader = ApiVersionReader.Combine(
			new UrlSegmentApiVersionReader(),
			new QueryStringApiVersionReader("api-version"),
			new HeaderApiVersionReader("X-Version"),
			new MediaTypeApiVersionReader("X-Version"));
	})
	.AddMvc(options => { })
	.AddApiExplorer(options =>
	{
		options.GroupNameFormat = "'v'VVV";
		options.SubstituteApiVersionInUrl = true;
	});

builder.Services.AddDbContext<PostgresContext>(o =>
	{
		o.UseNpgsql(builder.Configuration.GetValue<String>("ConnectionStrings:database"));
	});

builder.Services.AddEndpoints(typeof(Program).Assembly);
builder.Services.AddRateLimiter(options =>
	{
		options.AddFixedWindowLimiter("Fixed", opt =>
		{
			opt.Window = TimeSpan.FromSeconds(.5);
			opt.PermitLimit = 50;
		});
	});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

IDictionary<string, ColumnWriterBase> columnWriters = new Dictionary<string, ColumnWriterBase>
{
	{"message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
	{"level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
	{"raise_date", new TimestampColumnWriter(NpgsqlDbType.Timestamp) },
	{"exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
	{"properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb) },
};

var logger = new LoggerConfiguration()
		.ReadFrom.Configuration(builder.Configuration)
		.Enrich.FromLogContext()
		.Enrich.WithClientIp()
		.WriteTo.PostgreSQL(builder.Configuration.GetValue<String>("ConnectionStrings:database"), "request_log", columnWriters, needAutoCreateTable: true)
		.WriteTo.File("./log", rollingInterval: RollingInterval.Day)
		.CreateLogger();

Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine(msg));

builder.Logging.ClearProviders();

builder.Logging.AddSerilog(logger);

logger.Information($"Starting template in {builder.Environment.EnvironmentName} mode");

builder.Services.AddServices();

var app = builder.Build();

ApiVersionSet apiVersionSet = app.NewApiVersionSet()
	.HasApiVersion(new ApiVersion(1))
	// .HasApiVersion(new ApiVersion(2))
	.ReportApiVersions()
	.Build();

RouteGroupBuilder versionedGroup = app
	.MapGroup("api/v{version:apiVersion}")
	.WithApiVersionSet(apiVersionSet);

app.UseHttpsRedirection();

app.UseCors(CorsName);

app.UseMiddleware<GlobalLogHandler>();        //Logs every request's headers to aggregate logging database

app.UseExceptionHandler(o => { }); //Workaround to allow the exception handling interface to work

app.MapEndpoints(versionedGroup);

app.UseRateLimiter();

app.Run();

