using UserService.Data;
using Microsoft.EntityFrameworkCore;
using UserService.Services;
using UserService.Models;

namespace UserService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add database service to the container.
            builder.Services.AddDbContext<UserDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("AzureSqlConnection")));

            // Add EmailService to the DI container
            builder.Services.Configure<SendGridSettings>(builder.Configuration.GetSection("SendGrid"));
            builder.Services.AddTransient<IEmailService, EmailService>();

            // Add JwtTokenService to the DI container
            builder.Services.AddScoped<JwtTokenService>();

            // add PasswordHashingService to the DI container
            builder.Services.AddScoped<IPasswordHashingService, PasswordHashingService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
