var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Thêm dịch vụ cần thiết cho việc khám phá các API endpoint
builder.Services.AddEndpointsApiExplorer();

// Cấu hình Swagger đầy đủ để hỗ trợ JWT
builder.Services.AddSwaggerGen(options =>
{
    // Thêm tiêu đề và phiên bản cho API
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "ClassManager API",
        Version = "v1",
        Description = "API for ClassManager Application"
    });

    // Định nghĩa Security Scheme để Swagger biết cách sử dụng JWT Bearer token
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    // Yêu cầu Swagger phải gửi token cho tất cả các request
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
// Chỉ bật Swagger trong môi trường phát triển (Development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Chúng ta sẽ thêm các endpoint của chúng ta ở đây sau này

app.Run();