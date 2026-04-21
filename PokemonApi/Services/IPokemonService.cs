using PokemonApi.Models;

namespace PokemonApi.Services;

public interface IPokemonService
{
    Task<PokemonResponse?> GetByNameAsync(string name);
}
