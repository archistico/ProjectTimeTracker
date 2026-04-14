using FluentAssertions;
using ProjectTimeTracker.Api.Services;
using ProjectTimeTracker.Api.Tests.Helpers;

namespace ProjectTimeTracker.Api.Tests;

public class AreeServiceTests
{
    [Fact]
    public async Task GetAllAsync_Should_Return_Areas_Ordered_By_Description()
    {
        using var db = TestDbFactory.CreateContext();
        var service = new AreeService(db);

        var result = await service.GetAllAsync();

        result.Select(x => x.Descrizione).Should().Equal("Casa Editrice", "Grafica", "Programmazione");
    }
}
