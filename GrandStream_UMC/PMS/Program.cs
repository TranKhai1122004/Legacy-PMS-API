using PMS.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<IGrandstreamService, GrandstreamService>();

builder.Services.AddCors(options =>
{

    options.AddPolicy("AllowReact",
        policy =>
        {
            policy.AllowAnyOrigin() // Hoặc .WithOrigins("http://localhost:5173") để an toàn hơn
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

app.UseCors();
app.MapControllers();

app.Run("http://localhost:5274");