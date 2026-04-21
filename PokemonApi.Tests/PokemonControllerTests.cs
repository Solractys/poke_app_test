using Microsoft.AspNetCore.Mvc;
using Moq;
using PokemonApi.Controllers;
using PokemonApi.Models;
using PokemonApi.Services;

namespace PokemonApi.Tests;

public class PokemonControllerTests
{
    private static PokemonResponse MakePokemon(string name = "bulbasaur") => new()
    {
        Id = 1,
        Name = name,
        Height = 7,
        Weight = 69,
        BaseExperience = 64,
        Types = new List<PokemonType> { new() { Name = "grass" }, new() { Name = "poison" } },
        Stats = new List<PokemonStat> { new() { Name = "hp", BaseStat = 45 } },
        Sprites = new PokemonSprites { FrontDefault = "https://example.com/1.png" }
    };

    [Fact]
    public async Task GetByName_ExistingPokemon_ReturnsOk()
    {
        var serviceMock = new Mock<IPokemonService>();
        serviceMock.Setup(s => s.GetByNameAsync("bulbasaur")).ReturnsAsync(MakePokemon());
        var controller = new PokemonController(serviceMock.Object);

        var result = await controller.GetByName("bulbasaur");

        var okResult = Assert.IsType<OkObjectResult>(result);
        var pokemon = Assert.IsType<PokemonResponse>(okResult.Value);
        Assert.Equal("bulbasaur", pokemon.Name);
    }

    [Fact]
    public async Task GetByName_NotFoundPokemon_Returns404()
    {
        var serviceMock = new Mock<IPokemonService>();
        serviceMock.Setup(s => s.GetByNameAsync(It.IsAny<string>())).ReturnsAsync((PokemonResponse?)null);
        var controller = new PokemonController(serviceMock.Object);

        var result = await controller.GetByName("fakemon");

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetByName_CallsServiceWithCorrectName()
    {
        var serviceMock = new Mock<IPokemonService>();
        serviceMock.Setup(s => s.GetByNameAsync("charmander")).ReturnsAsync(MakePokemon("charmander"));
        var controller = new PokemonController(serviceMock.Object);

        await controller.GetByName("charmander");

        serviceMock.Verify(s => s.GetByNameAsync("charmander"), Times.Once);
    }

    [Fact]
    public async Task GetByName_ReturnsPokemonWithTypes()
    {
        var serviceMock = new Mock<IPokemonService>();
        serviceMock.Setup(s => s.GetByNameAsync("bulbasaur")).ReturnsAsync(MakePokemon());
        var controller = new PokemonController(serviceMock.Object);

        var result = await controller.GetByName("bulbasaur");

        var okResult = Assert.IsType<OkObjectResult>(result);
        var pokemon = Assert.IsType<PokemonResponse>(okResult.Value);
        Assert.Equal(2, pokemon.Types.Count);
        Assert.Contains(pokemon.Types, t => t.Name == "grass");
    }
}
