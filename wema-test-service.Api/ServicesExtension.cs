namespace wema_test_service.Api;

public static class ServicesExtension
{
    public static void ConfigureDatabaseConnection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), options =>
            {
                options.EnableRetryOnFailure();
            }));
    }

    public static void ConfigureAppServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IHttpClientService, HttpClientService>();
        services.AddScoped<IOtpService, OtpService>();
        services.AddScoped<IUtilityService, UtilityService>();
        services.AddScoped<ISmsService, SmsService>();
        services.AddHostedService<NotificationService>();
    }

    public static void ConfigureOtherServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AppSettings>().Bind(configuration.GetSection("AppSettings"));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
                    .AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
        services.AddMemoryCache();
        services.AddHttpClient();
    }

    public static void ConfigureApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(v =>
        {
            v.AssumeDefaultVersionWhenUnspecified = true;
            v.DefaultApiVersion = new ApiVersion(1, 0);
            v.ReportApiVersions = true;
            v.ApiVersionReader = ApiVersionReader.Combine(
                // new QueryStringApiVersionReader("api-version"),
                new HeaderApiVersionReader("x-version")
            // new MediaTypeApiVersionReader("api-version")
            );
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        }).AddMvc();
    }
}
