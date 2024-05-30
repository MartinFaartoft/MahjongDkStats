﻿namespace MahjongDkStatsCalculators;

public interface IStatsCalculator
{
    void AppendGame(Game game, GameType gameType);

    IEnumerable<Statistic> GetGlobalStatistics();

    IEnumerable<Statistic> GetGlobalMcrStatistics();
    IEnumerable<Statistic> GetGlobalRiichiStatistics();

    IEnumerable<PlayerStatistics> GetPlayerStatistics();
}