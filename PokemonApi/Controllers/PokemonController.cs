using Microsoft.AspNetCore.Mvc;
using PokemonApi.Services;

namespace PokemonApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PokemonController : ControllerBase
{
    private readonly IPokemonService _pokemonService;

    public PokemonController(IPokemonService pokemonService)
    {
        _pokemonService = pokemonService;
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest("Pokemon name is required.");

        var pokemon = await _pokemonService.GetByNameAsync(name);

        if (pokemon is null)
            return NotFound($"Pokemon '{name}' not found.");

        return Ok(pokemon);
    }
}
