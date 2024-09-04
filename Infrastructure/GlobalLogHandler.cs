using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.OpenApi.Expressions;

namespace template.Errors;

public class GlobalLogHandler
{

    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalLogHandler> _logger;

    public GlobalLogHandler(RequestDelegate next, ILogger<GlobalLogHandler> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var bodyString = new string("");
        var req = context.Request;
        req.EnableBuffering();

        using (StreamReader r = new StreamReader(req.Body, Encoding.UTF8, true, 1024, true))
        {
            bodyString = await r.ReadToEndAsync();
        }

        req.Body.Position = 0;

        RequestData data = new()
        {
            Headers = req.Headers,
            Body = bodyString
        };

        _logger.LogInformation(JsonSerializer.Serialize(data));
        await _next(context);
    }

    private class RequestData
    {
        public IHeaderDictionary Headers { get; set; }
        public string? Body { get; set; }
    }
}