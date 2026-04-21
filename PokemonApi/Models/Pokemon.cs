namespace PokemonApi.Models;

public class PokemonResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Height { get; set; }
    public int Weight { get; set; }
    public int BaseExperience { get; set; }
    public List<PokemonType> Types { get; set; } = new();
    public List<PokemonStat> Stats { get; set; } = new();
    public PokemonSprites Sprites { get; set; } = new();
}

public class PokemonType
{
    public string Name { get; set; } = string.Empty;
}

public class PokemonStat
{
    public string Name { get; set; } = string.Empty;
    public int BaseStat { get; set; }
}

public class PokemonSprites
{
    public string? FrontDefault { get; set; }
    public string? FrontShiny { get; set; }
}
