using System.Text.Json;
using FluentAssertions;
using ProjectTimeTracker.Web.Api;
using ProjectTimeTracker.Web.Models.ViewModels;
using ProjectTimeTracker.Web.Services;

namespace ProjectTimeTracker.Api.Tests.Web;

public class DashboardUiServiceTests
{
    [Fact]
    public async Task GetDashboardAsync_Should_Map_And_Order_Recent_Activity()
    {
        var apiClient = new FakeApiClient
        {
            Json = """
            {
              "progettiAperti": 7,
              "progettiUrgenti": 2,
              "progettiInRitardo": 1,
              "minutiLavoratiOggi": 135,
              "minutiLavoratiUltimi7Giorni": 420,
              "ultimiAggiornamenti": [
                { "progettoId": 10, "utente": "Mario Rossi", "azione": "Vecchio", "data": "2026-04-10T08:00:00" },
                { "progettoId": 11, "utente": "Emilie Rollandin", "azione": "Nuovo", "data": "2026-04-12T10:00:00" }
              ]
            }
            """
        };

        var service = new DashboardUiService(apiClient);
        var dashboard = await service.GetDashboardAsync();

        dashboard.ProgettiAperti.Valore.Should().Be("7");
        dashboard.TempoOggi.Valore.Should().Be("2h 15m");
        dashboard.TempoUltimi7Giorni.Valore.Should().Be("7h 0m");
        dashboard.UltimiAggiornamenti.First().Azione.Should().Be("Nuovo");
    }

    [Theory]
    [InlineData(0, "0h 0m")]
    [InlineData(59, "0h 59m")]
    [InlineData(60, "1h 0m")]
    [InlineData(125, "2h 5m")]
    public void FormatMinutes_Should_Format_Expected_Value(int minutes, string expected)
    {
        var service = new DashboardUiService(new FakeApiClient());

        service.FormatMinutes(minutes).Should().Be(expected);
    }

    private sealed class FakeApiClient : IApiClient
    {
        public string? Json { get; set; }

        public Task<bool> DeleteAsync(string url, CancellationToken cancellationToken = default) => Task.FromResult(true);
        public Task<List<T>> GetListAsync<T>(string url, CancellationToken cancellationToken = default) => Task.FromResult(new List<T>());
        public Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(Json))
            {
                return Task.FromResult(default(T));
            }

            var result = JsonSerializer.Deserialize<T>(Json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return Task.FromResult(result);
        }
        public Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest request, CancellationToken cancellationToken = default) => Task.FromResult(default(TResponse));
        public Task<bool> PutAsync<TRequest>(string url, TRequest request, CancellationToken cancellationToken = default) => Task.FromResult(true);
    }
}
