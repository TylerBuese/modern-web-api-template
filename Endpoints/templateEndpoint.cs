using System.Text.Json;
using System.Text.Json.Serialization;
using template.Interfaces;
using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Mvc;


public class templateEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (/*,[FromServices] IAuthorService authorService*/) =>
        {
            return "Hello, world! 👋";
        });

        // app.MapGet("/", ([FromQuery] string query) => $"Hello there!").MapToApiVersion(2);
    }
}