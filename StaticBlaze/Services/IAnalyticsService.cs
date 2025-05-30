﻿namespace StaticBlaze.Services;

public interface IAnalyticsService
{
    Task<(int views, int activeUsers)> GetAnalyticsData();
    Task<double> GetAverageLoadTime();
}