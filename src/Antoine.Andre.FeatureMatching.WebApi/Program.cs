using Antoine.Andre.FeatureMatching;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.MapPost("/featurematching", async ([FromForm] IFormFileCollection files) =>
{
    if (files.Count != 2)
        return Results.BadRequest();

    using var objectSourceStream = files[0].OpenReadStream();
    using var objectMemoryStream = new MemoryStream();
    await objectSourceStream.CopyToAsync(objectMemoryStream);
    var imageObjectData = objectMemoryStream.ToArray();

    using var sceneSourceStream = files[1].OpenReadStream();
    using var sceneMemoryStream = new MemoryStream();
    await sceneSourceStream.CopyToAsync(sceneMemoryStream);
    var imageSceneData = sceneMemoryStream.ToArray();

    var result = await new ObjectDetection().DetectObjectInScenesAsyncMock(imageObjectData, new List<byte[]> { imageSceneData });
    
    var imageData = result[0].ImageData;
    
    return Results.File(imageData, "image/jpg");
    
}).DisableAntiforgery();

app.UseAntiforgery();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}