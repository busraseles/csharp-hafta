using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// LM Studio ayarlarını appsettings.json'dan al
builder.Services.Configure<LmStudioOptions>(
    builder.Configuration.GetSection("LmStudio"));

// LM Studio ile konuşmak için HttpClient
builder.Services.AddHttpClient<LmStudioClient>((sp, http) =>
{
    var opt = sp.GetRequiredService<IOptions<LmStudioOptions>>().Value;

    http.BaseAddress = new Uri(opt.BaseUrl);
    http.Timeout = TimeSpan.FromSeconds(opt.TimeoutSeconds);
});

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<ITodoRepository, InMemoryTodoRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();