using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using MVCBlog.Business;
using MVCBlog.Business.Email;
using MVCBlog.Business.IO;
using MVCBlog.Data;
using MVCBlog.Web.Infrastructure;
using MVCBlog.Web.Infrastructure.Mvc;
using MVCBlog.Web.Infrastructure.Mvc.Health;
using MVCBlog.Web.Infrastructure.Mvc.SecurityHeaders;

namespace MVCBlog.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IConfigurationRoot ConfigurationRoot { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo("DataEncrpytionKeys"));

            services.Configure<BlogSettings>(this.Configuration.GetSection("BlogSettings"));
            services.Configure<MailSettings>(this.Configuration.GetSection("MailSettings"));

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
            });

            // Configure Compression level
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);

            // Add Response compression services
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.EnableForHttps = true;
            });

            services.AddDbContext<EFUnitOfWork>(options =>
                options.UseSqlServer(
                    this.Configuration.GetConnectionString("EFUnitOfWork")));

            services.AddDefaultIdentity<User>(config =>
                {
                    // Lockout settings
                    config.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                    config.Lockout.MaxFailedAccessAttempts = 10;

                    // User settings
                    // TOOD: Enable if desired: config.SignIn.RequireConfirmedEmail = true;
                    config.User.RequireUniqueEmail = true;
                })
                .AddDefaultUI()
                .AddEntityFrameworkStores<EFUnitOfWork>()
                .AddErrorDescriber<LocalizedIdentityErrorDescriber>();

            services.AddLocalization();

            services.AddControllersWithViews()
                .AddViewLocalization(options => options.ResourcesPath = "Resources");

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient<INotificationService, EmailNotificationService>();
            services.AddTransient<IEmailSender, EmailSenderAdapter>();
            services.AddTransient<IImageFileProvider, ImageFileProvider>();
            services.AddTransient<IBlogEntryFileFileProvider, BlogEntryFileFileProvider>();

            // Add CommandHandlers with decoractor
            // See https://github.com/khellang/Scrutor
            foreach (var serviceType in typeof(ICommandHandler<>).Assembly.GetTypes())
            {
                if (serviceType.IsAbstract || serviceType.IsInterface || serviceType.BaseType == null)
                {
                    continue;
                }

                foreach (var interfaceType in serviceType.GetInterfaces())
                {
                    if (interfaceType.IsGenericType && typeof(ICommandHandler<>).IsAssignableFrom(interfaceType.GetGenericTypeDefinition()))
                    {
                        // Register service
                        services.AddScoped(interfaceType, serviceType);

                        break;
                    }
                }
            }

            services.Decorate(typeof(ICommandHandler<>), typeof(CommandLoggingDecorator<>));

            services.AddHealthChecks()
                .AddSqlServer(this.Configuration["ConnectionStrings:EFUnitOfWork"])
                .AddCheck<LogfileHealthCheck>("Log files");

            // Register the Google Analytics configuration
            services.Configure<GoogleAnalyticsOptions>(options => Configuration.GetSection("GoogleAnalytics").Bind(options));

            // Register the TagHelperComponent
            services.AddTransient<ITagHelperComponent, GoogleAnalyticsTagHelperComponent>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            var supportedCultures = new[]
            {
                new CultureInfo("en"),
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(supportedCultures[0]),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseHttpsRedirection();
            app.UseResponseCompression();

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    const int durationInSeconds = 24 * 60 * 60; // 24 hours
                    ctx.Context.Response.Headers[HeaderNames.CacheControl] = "public,max-age=" + durationInSeconds;
                }
            });
            app.UseCookiePolicy();

            app.UseStatusCodePagesWithReExecute("/Error/{0}");
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();

           ConfigurationRoot=  builder.Build();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Blog}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            app.UseHealthChecks("/health", new HealthCheckOptions()
            {
                ResponseWriter = WriteResponse
            });

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<EFUnitOfWork>();
                context.Database.Migrate();
            }
        }
        private static Task WriteResponse(HttpContext httpContext, HealthReport result)
        {
            var writerOptions = new JsonWriterOptions
            {
                Indented = true
            };

            using (var ms = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(ms, options: writerOptions))
                {
                    writer.WriteStartObject();
                    writer.WriteString("status", result.Status.ToString());
                    writer.WriteStartObject("results");

                    foreach (var kv in result.Entries)
                    {
                        writer.WriteStartObject(kv.Key);
                        writer.WriteString("status", kv.Value.Status.ToString());
                        writer.WriteString("description", kv.Value.Description);

                        writer.WriteStartObject("data");
                        foreach (var item in kv.Value.Data)
                        {
                            writer.WriteString(item.Key, item.Value?.ToString());
                        }

                        writer.WriteEndObject();

                        writer.WriteEndObject();
                    }

                    writer.WriteEndObject();
                    writer.WriteEndObject();
                }

                httpContext.Response.ContentType = "application/json";
                return httpContext.Response.WriteAsync(Encoding.UTF8.GetString(ms.ToArray()));
            }
        }
    }
}
