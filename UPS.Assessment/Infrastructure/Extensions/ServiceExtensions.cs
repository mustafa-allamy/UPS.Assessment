using Microsoft.Extensions.DependencyInjection;
using System;
using UPS.Assessment.Infrastructure.Helpers;

namespace UPS.Assessment.Infrastructure.Extensions;

public static class ServiceExtensions
{

    public static void AddFormFactory<TForm>(this IServiceCollection services) where TForm : class
    {
        services.AddTransient<TForm>();
        services.AddTransient<Func<TForm>>(x => () => x.GetService<TForm>()!);
        services.AddTransient<IAbstractFactory<TForm>, AbstractFactory<TForm>>();
    }
}