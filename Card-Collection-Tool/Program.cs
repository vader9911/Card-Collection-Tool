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

            // Add logging to the services
            builder.Services.AddLogging();

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
                    builder.WithOrigins("http://localhost:4200")
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
                 client.DefaultRequestHeaders.Add("Accept", "application/json");
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

            // Ensure CORS is applied before routing, authentication, or authorization.
            app.UseCors("AllowSpecificOrigins");

            // Redirect HTTP requests to HTTPS.
            app.UseHttpsRedirection();

            // Serve static files from wwwroot.
            app.UseStaticFiles();

            // Configure routing; it should come before authentication.
            app.UseRouting();

            // Authentication and authorization should be after routing and before endpoint mapping.
            app.UseAuthentication();
            app.UseAuthorization();

            // Map controllers and endpoints.
            app.UseEndpoints(endpoints =>
            {
                _ = endpoints.MapControllers();
            });

            // Fallback route for Angular app.
            app.MapFallbackToFile("index.html");

            // Run the application.
            app.Run();
        }
    }
}