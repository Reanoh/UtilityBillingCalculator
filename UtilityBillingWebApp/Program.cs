using UtilityBillingWebApp.Models;
using UtilityBillingWebApp.Services;
using QuestPDF.Infrastructure; // ADD THIS NAMESPACE

namespace UtilityBillingWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllersWithViews();

            // Register billing service as scoped (one instance per request)
            builder.Services.AddScoped<BillingService>();

            // ADD THIS: Register invoice service for PDF generation
            builder.Services.AddScoped<InvoiceService>();

            // Optional: Validate tariffs on startup
            builder.Services.AddOptions<List<TariffTier>>()
                .Bind(builder.Configuration.GetSection("Tariffs"))
                .Validate(tariffs => 
                {
                    if (tariffs == null || tariffs.Count == 0)
                        return false;
                    
                    var ordered = tariffs.OrderBy(t => t.Min).ToList();
                    
                    // Validate first tier starts at 0
                    if (ordered[0].Min != 0)
                        return false;
                    
                    // Validate no gaps
                    for (int i = 0; i < ordered.Count - 1; i++)
                    {
                        if (ordered[i].Max + 1 != ordered[i + 1].Min)
                            return false;
                    }
                    
                    return true;
                }, "Tariff configuration is invalid");

            var app = builder.Build();

            // ADD THIS: Set QuestPDF license (required for QuestPDF to work)
            QuestPDF.Settings.License = LicenseType.Community;

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Billing}/{action=Index}/{id?}");

            app.Run();
        }
    }
}