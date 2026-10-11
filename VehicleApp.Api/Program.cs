
using Microsoft.AspNetCore.Diagnostics;
using Polly.CircuitBreaker;
using Polly.Timeout;
using VehicleApp.Api.Services;

namespace VehicleApp.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AngularApp", policy =>
                {
                    policy
                        .WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            builder.Services.AddControllers();

            builder.Services.AddMemoryCache();

            builder.Services.AddHttpClient<NhtsaService>(client =>
            {
                client.BaseAddress = new Uri("https://vpic.nhtsa.dot.gov/api/");
            })
            .AddStandardResilienceHandler();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            var app = builder.Build();

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

                    context.Response.ContentType = "application/json";

                    context.Response.StatusCode = exception switch
                    {
                        ArgumentException => StatusCodes.Status400BadRequest,
                        TaskCanceledException => StatusCodes.Status504GatewayTimeout,
                        TimeoutRejectedException => StatusCodes.Status504GatewayTimeout,
                        BrokenCircuitException => StatusCodes.Status503ServiceUnavailable,
                        HttpRequestException => StatusCodes.Status502BadGateway,
                        _ => StatusCodes.Status500InternalServerError
                    };

                    var message = context.Response.StatusCode switch
                    {
                        StatusCodes.Status400BadRequest => "Invalid request parameters.",
                        StatusCodes.Status504GatewayTimeout => "The external service request timed out.",
                        StatusCodes.Status502BadGateway => "The external service request failed.",
                        StatusCodes.Status503ServiceUnavailable => "The external service is temporarily unavailable.",

                        _ => "An unexpected error occurred."
                    };

                    await context.Response.WriteAsJsonAsync(new
                    {
                        context.Response.StatusCode,
                        message
                    });
                });
            });


          
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AngularApp");

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
