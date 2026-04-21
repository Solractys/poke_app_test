using System.Text.Json;
using System.Text.Json.Serialization;
using PokemonApi.Models;

namespace PokemonApi.Services;

public class PokemonService : IPokemonService
{
    private readonly HttpClient _httpClient;
    private static readonly string BaseUrl = "https://pokeapi.co/api/v2/pokemon/";

    public PokemonService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PokemonResponse?> GetByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        var normalizedName = name.Trim().ToLowerInvariant();

        var response = await _httpClient.GetAsync($"{BaseUrl}{normalizedName}");

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        var raw = JsonSerializer.Deserialize<PokeApiRawResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (raw is null)
            return null;

        return MapToPokemonResponse(raw);
    }

    private static PokemonResponse MapToPokemonResponse(PokeApiRawResponse raw)
    {
        return new PokemonResponse
        {
            Id = raw.Id,
            Name = raw.Name,
            Height = raw.Height,
            Weight = raw.Weight,
            BaseExperience = raw.BaseExperience,
            Types = raw.Types?.Select(t => new PokemonType { Name = t.Type?.Name ?? "" }).ToList() ?? new(),
            Stats = raw.Stats?.Select(s => new PokemonStat
            {
                Name = s.Stat?.Name ?? "",
                BaseStat = s.BaseStat
            }).ToList() ?? new(),
            Sprites = new PokemonSprites
            {
                FrontDefault = raw.Sprites?.FrontDefault,
                FrontShiny = raw.Sprites?.FrontShiny
            }
        };
    }

    // Raw PokeAPI DTOs
    private class PokeApiRawResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Height { get; set; }
        public int Weight { get; set; }
        [JsonPropertyName("base_experience")]
        public int BaseExperience { get; set; }
        public List<TypeSlot>? Types { get; set; }
        public List<StatSlot>? Stats { get; set; }
        public RawSprites? Sprites { get; set; }
    }

    private class TypeSlot
    {
        public NamedResource? Type { get; set; }
    }

    private class StatSlot
    {
        [JsonPropertyName("base_stat")]
        public int BaseStat { get; set; }
        public NamedResource? Stat { get; set; }
    }

    private class NamedResource
    {
        public string Name { get; set; } = string.Empty;
    }

    private class RawSprites
    {
        [JsonPropertyName("front_default")]
        public string? FrontDefault { get; set; }
        [JsonPropertyName("front_shiny")]
        public string? FrontShiny { get; set; }
    }
}
