using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Sentry;
using SignalR.Hubs;

public class SignalRHealthCheck : IHealthCheck
{
    private readonly HubLifetimeManager<HestiaHub> _lifetime;
    public SignalRHealthCheck(HubLifetimeManager<HestiaHub> lifetime) => _lifetime = lifetime;

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
    {
        var count = HestiaHub.ConnectionCount;
        return Task.FromResult(HealthCheckResult.Healthy($"Connections={count}"));
    }
}
