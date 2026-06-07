using Microsoft.Extensions.DependencyInjection;

namespace Registry.Application;

public static class DependencyInjection
{
    // Adicionar para fazer validators dps
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        //services.Scan(scan => scan
        //    .FromAssemblyOf<Program>()
        //    .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
        //        .AsImplementedInterfaces()
        //        .WithScopedLifetime()
        //    .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)))
        //        .AsImplementedInterfaces()
        //        .WithScopedLifetime()
        //    .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<,>)))
        //        .AsImplementedInterfaces()
        //        .WithScopedLifetime());

        return services;
    }
}
