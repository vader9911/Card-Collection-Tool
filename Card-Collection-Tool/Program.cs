using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Card_Collection_Tool.Data;
using Card_Collection_Tool.Services;



namespace Card_Collection_Tool
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add authorization services
            builder.Services.AddAuthorization();

            // Register the database context with Identity support
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            //// Add Identity services
            //builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            // Configure Identity
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Configure JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });

            // Configure CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins", builder =>
                {
                    builder.WithOrigins("http://localhost:4200") // Replace with your Angular front end URL
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
            });

            // Register the ScryfallService with HttpClient
            builder.Services.AddHttpClient<ScryfallService>(client =>
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Card-Collection-App/1.0");
            });

            // Register the ScryfallSyncService with HttpClient for dependency injection
            builder.Services.AddHttpClient<ScryfallSyncService>(client =>
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Card-Collection-App/1.0");
            });

            // Register the hosted service for periodic data synchronization
            builder.Services.AddHostedService<ScryfallSyncHostedService>();


            // Add services to the container.
            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }


            app.UseHttpsRedirection();
            app.UseStaticFiles(); // Serve static files from wwwroot

            app.UseRouting();

            app.UseCors("AllowSpecificOrigins");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapFallbackToFile("index.html"); // Fallback route to serve Angular app

            app.Run();
        }
    }
}

           

            //var builder = WebApplication.CreateBuilder(args);

            //// Add services to the container.
            //builder.Services.AddControllersWithViews();


            //// Register the database context with Identity support
            //builder.Services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            //// Add Identity services
            //builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            //// Register the ScryfallService with HttpClient
            //builder.Services.AddHttpClient<ScryfallService>(client =>
            //{
            //    client.DefaultRequestHeaders.Add("User-Agent", "Card-Collection-App/1.0");
            //});

            //// Register the ScryfallSyncService with HttpClient for dependency injection
            //builder.Services.AddHttpClient<ScryfallSyncService>(client =>
            //{
            //    client.DefaultRequestHeaders.Add("User-Agent", "Card-Collection-App/1.0");
            //});

            //// Register the hosted service for periodic data synchronization
            //builder.Services.AddHostedService<ScryfallSyncHostedService>();

            //var app = builder.Build();

            //// Configure the HTTP request pipeline.
            //if (!app.Environment.IsDevelopment())
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}

            //app.UseHttpsRedirection();
            //app.UseStaticFiles();

            //app.UseRouting();
            //app.UseAuthentication();
            //app.UseAuthorization();

            //app.MapControllerRoute(
            //    name: "default",
            //    pattern: "{controller=Home}/{action=Index}/{id?}");

            //app.MapRazorPages();

            //app.Run();
        