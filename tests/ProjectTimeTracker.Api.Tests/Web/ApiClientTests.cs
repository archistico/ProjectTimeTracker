using System.Net;
using FluentAssertions;
using ProjectTimeTracker.Api.Tests.Helpers;
using ProjectTimeTracker.Web.Api;

namespace ProjectTimeTracker.Api.Tests.Web;

public class ApiClientTests
{
    [Fact]
    public async Task GetListAsync_Should_Return_Deserialized_List_When_Response_Is_Success()
    {
        var httpClient = new HttpClient(new FakeHttpMessageHandler(_ =>
            FakeHttpMessageHandler.Json(HttpStatusCode.OK, "[{\"id\":1,\"nome\":\"Test\"}]")))
        {
            BaseAddress = new Uri("http://localhost")
        };

        var client = new ApiClient(httpClient);
        var result = await client.GetListAsync<TestItem>("/api/test");

        result.Should().HaveCount(1);
        result[0].Nome.Should().Be("Test");
    }

    [Fact]
    public async Task GetAsync_Should_Return_Null_When_Response_Is_Not_Success()
    {
        var httpClient = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.BadRequest)))
        {
            BaseAddress = new Uri("http://localhost")
        };

        var client = new ApiClient(httpClient);
        var result = await client.GetAsync<TestItem>("/api/test");

        result.Should().BeNull();
    }

    [Fact]
    public async Task PostAsync_Should_Return_Default_When_Response_Is_Not_Success()
    {
        var httpClient = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.InternalServerError)))
        {
            BaseAddress = new Uri("http://localhost")
        };

        var client = new ApiClient(httpClient);
        var result = await client.PostAsync<object, TestItem>("/api/test", new { Nome = "X" });

        result.Should().BeNull();
    }

    [Fact]
    public async Task PutAsync_And_DeleteAsync_Should_Return_True_On_Success_Status()
    {
        var httpClient = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.NoContent)))
        {
            BaseAddress = new Uri("http://localhost")
        };

        var client = new ApiClient(httpClient);

        (await client.PutAsync("/api/test", new { Nome = "X" })).Should().BeTrue();
        (await client.DeleteAsync("/api/test/1")).Should().BeTrue();
    }

    private sealed class TestItem
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }
}
