using BankingSystem.Data;
using BankingSystem.Data.models;
using BankingSystem.Middlewares;
using BankingSystem.Repository.BankerRepository;
using BankingSystem.Repository.CreditCardRepository;
using BankingSystem.Repository.GenericRepository;
using BankingSystem.Repository.LoanRepository;
using BankingSystem.Repository.SpecificRepository.AccountRepository;
using BankingSystem.Repository.SpecificRepository.CustomerRepository;
using BankingSystem.Repository.SpecificRepository.TransactionRepository;
using BankingSystem.Repository.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddIdentity<User, Role>(options => {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 7;
                options.Password.RequiredUniqueChars = 1;
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 10;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IAccountRepository , AccountRepository>();
            builder.Services.AddScoped<IBankerRepository, BankerRepository>();
            builder.Services.AddScoped<ICreditCardRepository, CreditCardRepository>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<ILoanRepository, LoanRepository>();
            builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
            builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var app = builder.Build();
            app.UseMiddleware<ExceptionMiddleware>();

            app.Use(async (context, next) =>
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                context.Response.OnStarting(() =>
                {
                    watch.Stop();
                    context.Response.Headers.Append("X-Response-Time", $"{watch.ElapsedMilliseconds} ms");
                    return Task.CompletedTask;
                });

                await next();
            });

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
