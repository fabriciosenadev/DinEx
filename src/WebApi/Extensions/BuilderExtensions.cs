namespace Dinex.Api
{
    public static class BuilderExtensions
    {
        public static void AddArchitecture(this WebApplicationBuilder builder, string corsRoleName)
        {
            builder.Services.AddSwaggerConfiguration();

            builder.Services.AddAppServices(builder.Configuration, corsRoleName);

            builder.Services.AddBusinessServices();
        }

        public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration, string corsRoleName)
        {
            // Add services to the container.
            //services.AddControllers();

            //connection string
            //builder.Services.AddDbContext<DinexBackendContext>(
            //    options => options.UseNpgsql(builder.Configuration.GetConnectionString("DinExDB"))
            //    );

            var appSettings = new AppSettings();
            new ConfigureFromConfigurationOptions<AppSettings>(configuration.GetSection("AppSettings")).Configure(appSettings);
            services.AddSingleton(appSettings);

            // fix to work with this section on classes and methods
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

            //configuration to allow authentication -- see doc https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/security?view=aspnetcore-8.0
            services.AddCors(options =>
            {
                options.AddPolicy(name: corsRoleName,
                    builder =>
                    {
                        builder.WithOrigins(appSettings.AllowedHost)
                            .WithMethods("POST", "PUT", "GET", "DELETE", "OPTIONS")
                            .WithHeaders("accept", "content-type", "origin", "authorization");
                    });
            });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Secret))
                        };
                    });
            services.AddAuthorization();

            services.AddAntiforgery();

            // configuration to work with ExcelDataReader --- see readme https://github.com/ExcelDataReader/ExcelDataReader
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            return services;
        }

        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                #region security to swagger
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Insira o token JWT desta maneira: Bearer {seu token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
                #endregion
            });

            return services;
        }
    }
}
