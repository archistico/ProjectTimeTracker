using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using ProjectTimeTracker.Api.Tests.Helpers;
using ProjectTimeTracker.Web.Api;

namespace ProjectTimeTracker.Api.Tests.Web;

public class ApiClientTests
{
    [Fact]
    public async Task GetAsync_Should_Return_Object_When_Response_Is_Success()
    {
        var payload = new TestResponse { Id = 1, Name = "Mario" };
        var json = JsonSerializer.Serialize(payload);

        var httpClient = new HttpClient(new FakeHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            }))
        {
            BaseAddress = new Uri("http://localhost")
        };

        var apiClient = new ApiClient(httpClient);

        var result = await apiClient.GetAsync<TestResponse>("/test");

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Mario");
    }

    [Fact]
    public async Task GetAsync_Should_Throw_ApiClientException_When_Response_Is_Not_Success()
    {
        var json = JsonSerializer.Serialize(new ApiErrorResponse
        {
            Error = "ValidationError",
            Message = "La richiesta inviata al server non è valida."
        });

        var httpClient = new HttpClient(new FakeHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            }))
        {
            BaseAddress = new Uri("http://localhost")
        };

        var apiClient = new ApiClient(httpClient);

        var act = async () => await apiClient.GetAsync<TestResponse>("/test");

        var ex = await act.Should().ThrowAsync<ApiClientException>();
        ex.Which.Message.Should().Be("La richiesta inviata al server non è valida.");
        ex.Which.IsBadRequest.Should().BeTrue();
    }

    [Fact]
    public async Task PostAsync_Should_Return_Object_When_Response_Is_Success()
    {
        var payload = new TestResponse { Id = 10, Name = "Creato" };
        var json = JsonSerializer.Serialize(payload);

        var httpClient = new HttpClient(new FakeHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            }))
        {
            BaseAddress = new Uri("http://localhost")
        };

        var apiClient = new ApiClient(httpClient);

        var result = await apiClient.PostAsync<object, TestResponse>("/test", new { });

        result.Should().NotBeNull();
        result!.Id.Should().Be(10);
        result.Name.Should().Be("Creato");
    }

    [Fact]
    public async Task PostAsync_Should_Throw_ApiClientException_When_Response_Is_Not_Success()
    {
        var json = JsonSerializer.Serialize(new ApiErrorResponse
        {
            Error = "ServerError",
            Message = "Il server ha restituito un errore interno."
        });

        var httpClient = new HttpClient(new FakeHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            }))
        {
            BaseAddress = new Uri("http://localhost")
        };

        var apiClient = new ApiClient(httpClient);

        var act = async () => await apiClient.PostAsync<object, TestResponse>("/test", new { });

        var ex = await act.Should().ThrowAsync<ApiClientException>();
        ex.Which.Message.Should().Be("Il server ha restituito un errore interno.");
        ex.Which.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    private sealed class TestResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}