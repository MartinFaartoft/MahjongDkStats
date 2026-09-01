using MahjongDkStatsCalculators;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MahjongDkStats.CLI;

public class GamesLoader
{
    private readonly JsonSerializerOptions _options;

    public GamesLoader()
    {
        _options = new JsonSerializerOptions();
        _options.Converters.Add(new RoundedDecimalConverter(2));
        _options.Converters.Add(new RoundedNullableDecimalConverter(2));
    }

    public async Task<IEnumerable<Game>> LoadGamesAsync(string url)
    {
        var httpClient = new HttpClient();
        
        var json = await httpClient.GetStringAsync(url);
        return JsonSerializer.Deserialize<IEnumerable<Game>>(json, _options)!.OrderBy(g => g.Id); 
    }

    public async Task<IEnumerable<Game>> LoadGamesFromFileAsync(string path)
    {
        var json = await File.ReadAllTextAsync(path);

        return JsonSerializer.Deserialize<IEnumerable<Game>>(json, _options)!.OrderBy(g => g.Id);
    }
    
    internal sealed class RoundedDecimalConverter(int digits = 2, MidpointRounding mode = MidpointRounding.ToEven)
        : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type _, JsonSerializerOptions __)
            => Math.Round(reader.GetDecimal(), digits, mode);

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions _)
            => writer.WriteNumberValue(Math.Round(value, digits, mode));
    }
    
    public sealed class RoundedNullableDecimalConverter(int digits = 2) : JsonConverter<decimal?>
    {
        public override decimal? Read(ref Utf8JsonReader reader, Type _, JsonSerializerOptions __)
            => reader.TokenType == JsonTokenType.Null ? null
                : Math.Round(reader.GetDecimal(), digits, MidpointRounding.ToEven);

        public override void Write(Utf8JsonWriter writer, decimal? value, JsonSerializerOptions _)
        {
            if (value is null) writer.WriteNullValue();
            else writer.WriteNumberValue(Math.Round(value.Value, digits, MidpointRounding.ToEven));
        }
    }
}
