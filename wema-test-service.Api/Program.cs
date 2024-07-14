WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Logger Setup
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

// Add services to the container.

builder.Services.ConfigureDatabaseConnection(builder.Configuration);
builder.Services.ConfigureAppServices();
builder.Services.ConfigureOtherServices(builder.Configuration);
builder.Services.ConfigureApiVersioning();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
})
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    options.SerializerSettings.Converters.Add(new StringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
string[] origins = builder.Configuration["AppSettings:Origins"].Split(",");
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
    .WithOrigins(origins)
    .AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo() { Title = builder.Configuration["AppSettings:ServiceName"], Version = "v1" });

    // Set the comments path for the Swagger JSON and UI.
    string xmlFile = $"{Assembly.GetEntryAssembly()?.GetName().Name}.xml";
    string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Host.UseSerilog();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(s =>
    {
        s.SwaggerEndpoint(builder.Configuration["AppSettings:SwaggerEndpoint"], builder.Configuration["AppSettings:ServiceName"]);
    });
}

app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UsePathBase(builder.Configuration["AppSettings:ContextPath"]);
app.UseAuthorization();
app.UseMiddleware<RequestResponseLoggerMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.MapControllers();

app.Run();
