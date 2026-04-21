using System.Net;
using System.Text;
using System.Text.Json;
using Moq;
using Moq.Protected;
using PokemonApi.Services;

namespace PokemonApi.Tests;

public class PokemonServiceTests
{
    private static HttpClient CreateMockHttpClient(HttpStatusCode statusCode, string content)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            });

        return new HttpClient(handlerMock.Object);
    }

    private static string BuildValidPokeApiJson(int id = 25, string name = "pikachu") => JsonSerializer.Serialize(new
    {
        id,
        name,
        height = 4,
        weight = 60,
        base_experience = 112,
        types = new[] { new { type = new { name = "electric" } } },
        stats = new[]
        {
            new { base_stat = 35, stat = new { name = "hp" } },
            new { base_stat = 55, stat = new { name = "attack" } }
        },
        sprites = new
        {
            front_default = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/25.png",
            front_shiny = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/shiny/25.png"
        }
    });

    [Fact]
    public async Task GetByNameAsync_ValidName_ReturnsPokemon()
    {
        var client = CreateMockHttpClient(HttpStatusCode.OK, BuildValidPokeApiJson());
        var service = new PokemonService(client);

        var result = await service.GetByNameAsync("pikachu");

        Assert.NotNull(result);
        Assert.Equal("pikachu", result!.Name);
        Assert.Equal(25, result.Id);
        Assert.Equal(4, result.Height);
        Assert.Equal(60, result.Weight);
        Assert.Equal(112, result.BaseExperience);
    }

    [Fact]
    public async Task GetByNameAsync_ValidName_ReturnsCorrectTypes()
    {
        var client = CreateMockHttpClient(HttpStatusCode.OK, BuildValidPokeApiJson());
        var service = new PokemonService(client);

        var result = await service.GetByNameAsync("pikachu");

        Assert.NotNull(result);
        Assert.Single(result!.Types);
        Assert.Equal("electric", result.Types[0].Name);
    }

    [Fact]
    public async Task GetByNameAsync_ValidName_ReturnsCorrectStats()
    {
        var client = CreateMockHttpClient(HttpStatusCode.OK, BuildValidPokeApiJson());
        var service = new PokemonService(client);

        var result = await service.GetByNameAsync("pikachu");

        Assert.NotNull(result);
        Assert.Equal(2, result!.Stats.Count);
        Assert.Equal("hp", result.Stats[0].Name);
        Assert.Equal(35, result.Stats[0].BaseStat);
        Assert.Equal("attack", result.Stats[1].Name);
        Assert.Equal(55, result.Stats[1].BaseStat);
    }

    [Fact]
    public async Task GetByNameAsync_ValidName_ReturnsSprites()
    {
        var client = CreateMockHttpClient(HttpStatusCode.OK, BuildValidPokeApiJson());
        var service = new PokemonService(client);

        var result = await service.GetByNameAsync("pikachu");

        Assert.NotNull(result);
        Assert.NotNull(result!.Sprites.FrontDefault);
        Assert.Contains("25.png", result.Sprites.FrontDefault!);
    }

    [Fact]
    public async Task GetByNameAsync_NotFound_ReturnsNull()
    {
        var client = CreateMockHttpClient(HttpStatusCode.NotFound, "");
        var service = new PokemonService(client);

        var result = await service.GetByNameAsync("notapokemon");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameAsync_EmptyName_ReturnsNull()
    {
        var client = CreateMockHttpClient(HttpStatusCode.OK, "");
        var service = new PokemonService(client);

        var result = await service.GetByNameAsync("   ");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameAsync_NormalizesNameToLowercase()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        HttpRequestMessage? capturedRequest = null;

        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(BuildValidPokeApiJson(), Encoding.UTF8, "application/json")
            });

        var service = new PokemonService(new HttpClient(handlerMock.Object));
        await service.GetByNameAsync("PIKACHU");

        Assert.NotNull(capturedRequest);
        Assert.Contains("pikachu", capturedRequest!.RequestUri!.ToString());
    }

    [Fact]
    public async Task GetByNameAsync_ServerError_ReturnsNull()
    {
        var client = CreateMockHttpClient(HttpStatusCode.InternalServerError, "");
        var service = new PokemonService(client);

        var result = await service.GetByNameAsync("pikachu");

        Assert.Null(result);
    }
}
