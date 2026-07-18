using Musica.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Define o endereço e a porta em que a aplicação irá escutar requisições HTTP
builder.WebHost.UseUrls("http://localhost:8000");

// Constrói a aplicação web a partir das configurações definidas no builder
var app = builder.Build();

app.UseCors("AllowAll");
app.UseDefaultFiles();
app.UseStaticFiles();

MusicaModel[] musicas = new MusicaModel[100];
int totalMusicas = 0;

app.MapGet("/api", () =>
{
    return Results.Ok("API Esound funcionando com sucesso!");
});

app.MapPost("/Musicas", (JsonElement body) =>
{
    Random random = new();

    MusicaModel NovaMusica = new MusicaModel();

    NovaMusica.Id = random.Next(0,9999);
    NovaMusica.Titulo = body.GetProperty("titulo").GetString();
    NovaMusica.Artista = body.GetProperty("artista").GetString();
    NovaMusica.Compositor = body.GetProperty("compositor").GetString();
    NovaMusica.Genero = body.GetProperty("genero").GetString();
    NovaMusica.Ano = body.GetProperty("ano").GetInt32();

    musicas[totalMusicas] = NovaMusica;
    totalMusicas++;

    return Results.Ok(
        new {
            musica = NovaMusica
        }
    );
});

app.MapGet("/Musicas", () =>
{
    MusicaModel[] musicasCadastradas = new MusicaModel[totalMusicas];

    for (int i = 0; i < totalMusicas; i++)
    {
        musicasCadastradas[i] = musicas[i];
    }

    return Results.Ok(new {
        musicas = musicasCadastradas
    });

});

app.MapGet("/Musicas/busca/{titulo}", (string titulo) =>
{
    MusicaModel[] musicasEncontradas = new MusicaModel[totalMusicas];

    int totalEncontradas = 0;

    for (int i = 0; i < totalMusicas; i++)
    {
        if (musicas[i].Titulo.ToLower().Contains(titulo.ToLower()))
        {
            musicasEncontradas[totalEncontradas] = musicas[i];
            totalEncontradas++;
        }
    }

    if (totalEncontradas > 0)
    {
        MusicaModel[] resultado = new MusicaModel[totalEncontradas];

        for (int i = 0; i < totalEncontradas; i++)
        {
            resultado[i] = musicasEncontradas[i];
        }

        return Results.Ok(new
        {
            titulo,
            musicas = resultado
        });
    }

    return Results.NotFound(new
    {
        message = "Nenhuma Música com esse título foi encontrada."
    });
});

app.MapPatch("/Musicas/{id}", (int id, JsonElement body) =>
{
    string novo_titulo = body.GetProperty("titulo").GetString();

    for(int i = 0; i < totalMusicas; i++)
    {
        if (musicas[i].Id == id)
        {
            musicas[i].Titulo = novo_titulo;
            return Results.Ok(new
            {
                musica = musicas[i]

            });
        }
    }

    return Results.NotFound(new
    {
        message = "Música não encontrada."

    });
});

app.MapDelete("/Musicas/{id}", (int id) =>
{
    for (int i = 0; i < totalMusicas; i++)
    {
        if (musicas[i].Id == id)
        {
            MusicaModel musicaRemovida = musicas[i];

            for (int j = i; j < totalMusicas - 1; j++)
            {
                musicas[j] = musicas[j + 1];
            }

            totalMusicas--;

            return Results.Ok(new
            {
                message = "Música removida com sucesso!",
                musica = musicaRemovida
            });
        }
    }

    return Results.NotFound(new
    {
        message = "Música não encontrada."
    });
});

app.MapGet("/Musicas/artista/{artista}", (string artista) =>
{
    MusicaModel[] musicasEncontradas = new MusicaModel[totalMusicas];
    int totalEncontradas = 0;

    for (int i = 0; i < totalMusicas; i++)
    {
        if (musicas[i].Artista.ToLower().Contains(artista.ToLower()))
        {
            musicasEncontradas[totalEncontradas] = musicas[i];
            totalEncontradas++;
        }
    }

    if (totalEncontradas > 0)
    {
        MusicaModel[] resultado = new MusicaModel[totalEncontradas];

        for (int i = 0; i < totalEncontradas; i++)
        {
            resultado[i] = musicasEncontradas[i];
        }

        return Results.Ok(new
        {
            artista,
            musicas = resultado
        });
    }

    return Results.NotFound(new
    {
        message = "Nenhuma música desse artista foi encontrada."
    });
});

app.MapGet("/Musicas/genero/{genero}", (string genero) =>
{
    MusicaModel[] musicasEncontradas = new MusicaModel[totalMusicas];
    int totalEncontradas = 0;

    for (int i = 0; i < totalMusicas; i++)
    {
        if (musicas[i].Genero.ToLower().Contains(genero.ToLower()))
        {
            musicasEncontradas[totalEncontradas] = musicas[i];
            totalEncontradas++;
        }
    }

    if (totalEncontradas > 0)
    {
        MusicaModel[] resultado = new MusicaModel[totalEncontradas];

        for (int i = 0; i < totalEncontradas; i++)
        {
            resultado[i] = musicasEncontradas[i];
        }

        return Results.Ok(new
        {
            genero,
            musicas = resultado
        });
    }

    return Results.NotFound(new
    {
        message = "Nenhuma música desse gênero foi encontrada."
    });
});

app.Run();