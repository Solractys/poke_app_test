using PokemonApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddHttpClient<IPokemonService, PokemonService>();

var app = builder.Build();

app.UseRouting();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
