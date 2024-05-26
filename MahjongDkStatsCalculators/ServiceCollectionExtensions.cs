using MahjongDkStatsCalculators.StatsCalculators;
using Microsoft.Extensions.DependencyInjection;

namespace MahjongDkStatsCalculators;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStatsCalculators(this IServiceCollection services) 
    {
        services
            .AddTransient<IStatsCalculator, GlobalCountsCalculator>()
            .AddTransient<IStatsCalculator, BestScoreCalculator>();

        return services;
    }
}