using Hangfire;

namespace EduTrailblaze.API.Extensions
{
    public static class ApplicationExtensions
    {
        public static void UseInfrastructure(this IApplicationBuilder app)
        {
            app.UseHangfireDashboard();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
            });
            app.UseHttpsRedirection();

            app.UseHsts();
            app.Use(async (context, next) =>
            {
                context.Response.Headers["X-Content-Type-Options"] = "nosniff";
                context.Response.Headers["Referrer-Policy"] = "no-referrer";
                context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
                context.Response.Headers["X-Frame-Options"] = "DENY";
                await next();
            });

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseMiddleware<RequestResponseMiddleware>();

        }
    }
}
