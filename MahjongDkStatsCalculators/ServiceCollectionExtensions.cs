using MahjongDkStatsCalculators.StatsCalculators;
using Microsoft.Extensions.DependencyInjection;

namespace MahjongDkStatsCalculators;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStatsCalculators(this IServiceCollection services) 
    {
        services
            .AddTransient<IStatsCalculator, GlobalCountsCalculator>()
			.AddTransient<IStatsCalculator, GlobalMcrCountsCalculator>()
			.AddTransient<IStatsCalculator, GlobalRiichiCountsCalculator>()
			.AddTransient<IStatsCalculator, HighestScoreCalculator>()
            .AddTransient<IStatsCalculator, PlayerStatisticsCalculator>();

        return services;
    }
}