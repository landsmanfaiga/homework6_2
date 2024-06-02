using Microsoft.AspNetCore.Authentication.Cookies;

namespace homework6_1.Web;

public class Program
{
    private static string CookieScheme = "ReactAuthDemo";
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();
        builder.Services.AddSignalR();

        builder.Services.AddAuthentication(CookieScheme)
          .AddCookie(CookieScheme, options =>
          {
              options.Events = new CookieAuthenticationEvents
              {
                  OnRedirectToLogin = context =>
                  {
                      context.Response.StatusCode = 403;
                      context.Response.ContentType = "application/json";
                      var result = System.Text.Json.JsonSerializer.Serialize(new { error = "You are not authenticated" });
                      return context.Response.WriteAsync(result);
                  }
              };
          });

        builder.Services.AddSession();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
        }

        //app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.MapHub<TaskHub>("/api/task");

        app.UseSession();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller}/{action=Index}/{id?}");

        app.MapFallbackToFile("index.html");

        app.Run();
    }
}