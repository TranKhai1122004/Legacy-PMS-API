using PMS_Real.Services;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký hệ thống Controller
builder.Services.AddControllers();

// Khai báo Dependency Injection cho Service của mình
builder.Services.AddSingleton<IGrandstreamService, GrandstreamService>();

// Mở CORS cho Frontend local gọi vào
builder.Services.AddCors(options =>
{
    // Thay vì dùng AddDefaultPolicy, hãy dùng AddPolicy với tên "AllowReact"
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

// Cho chạy ở cổng 5274
app.Run("http://localhost:5274");