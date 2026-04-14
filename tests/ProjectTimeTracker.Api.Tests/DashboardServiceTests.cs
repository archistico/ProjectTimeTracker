using FluentAssertions;
using ProjectTimeTracker.Api.Services;
using ProjectTimeTracker.Api.Tests.Helpers;

namespace ProjectTimeTracker.Api.Tests;

public class DashboardServiceTests
{
    [Fact]
    public async Task GetDashboardAsync_Should_Calculate_All_Key_Metrics()
    {
        using var db = TestDbFactory.CreateContext();
        var service = new DashboardService(db);

        var result = await service.GetDashboardAsync();

        result.ProgettiAperti.Should().Be(2);
        result.ProgettiUrgenti.Should().Be(1);
        result.ProgettiInRitardo.Should().Be(1);
        result.MinutiLavoratiOggi.Should().Be(90);
        result.MinutiLavoratiUltimi7Giorni.Should().Be(255);
        result.UltimiAggiornamenti.Should().HaveCount(3);
        result.UltimiAggiornamenti.First().Azione.Should().Be("Aggiornato stato");
    }
}
