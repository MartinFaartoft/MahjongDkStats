using MahjongDkStatsCalculators.Calculators;
using Microsoft.Extensions.DependencyInjection;

namespace MahjongDkStatsCalculators;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStatsCalculators(this IServiceCollection services) 
    {
        services
            .AddTransient<IStatsCalculator, StatisticsCalculator>();

        return services;
    }
}