using StackExchange.Redis;
using Microsoft.Extensions.DependencyInjection;

namespace Valuator;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Добавляем сервисы в контейнер
        builder.Services.AddRazorPages();

        // Регистрируем Redis (подключение)
        builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var configuration = builder.Configuration.GetConnectionString("Redis");
            return ConnectionMultiplexer.Connect(configuration);
        });

        //Создает экзмепляр приложения
        var app = builder.Build();

        // Настройка middleware
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();

        app.Run();

        app.Use(async (context, next) =>
        {
            Console.WriteLine($"Запрос получен на порту: {context.Connection.LocalPort}");
            await next();
        });
        
    }
}