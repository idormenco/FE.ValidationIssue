using FastEndpoints;
using FastEndpoints.Swagger;
using FluentValidationIssue.Feature; //add this

var bld = WebApplication.CreateBuilder();
bld.Services
    .AddFastEndpoints(o => o.Assemblies = new[]
    {
        typeof(Request).Assembly
    })
    .SwaggerDocument(); //define a swagger document

var app = bld.Build();
app.UseFastEndpoints()
    .UseSwaggerGen(); //add this
app.Run();