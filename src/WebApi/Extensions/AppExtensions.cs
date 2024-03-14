namespace Dinex.Api
{
    public static class AppExtensions
    {
        public static void UseArchitectures(this WebApplication app, string corsRoleName)
        {
            app.UseAppConfig(app.Environment, app.Services, corsRoleName);

            app.MapUserEndpoints();
            app.MapFileEndpoints();
            app.MapQueueEndpoints();
        }

        public static IApplicationBuilder UseAppConfig(
            this IApplicationBuilder app, IWebHostEnvironment environment, IServiceProvider serviceScope, string corsRoleName)
        {
            // Execute Migrations on start app
            using (var scope = serviceScope.CreateScope())
            {
                //var dataContext = scope.ServiceProvider.GetRequiredService<DinexBackendContext>();
                //dataContext.Database.Migrate();
            }

            app.UseRouting();

            app.UseCors(corsRoleName);

            if (environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseHsts();
            }

            app.UseMiddleware<AuthInfoMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseAntiforgery();

            app.UseStaticFiles();

            return app;
        }
    }
}
