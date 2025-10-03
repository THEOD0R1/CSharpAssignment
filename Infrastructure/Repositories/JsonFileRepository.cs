using System.Text.Json;

namespace Infrastructure.Repositories;

public interface IJsonFileRepository
{
    Task WriteAsync<T>(IEnumerable<T> jsonContent, CancellationToken cancellationToken = default);

    ValueTask<IReadOnlyList<T>> ReadAsync<T>(CancellationToken cancellationToken = default);
}
public class JsonFileRepository : IJsonFileRepository
{
    private readonly string _filePath;
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
    };
    public JsonFileRepository(string fileName = "data.json")
    {
        string baseDirectory = AppContext.BaseDirectory;
        string dataDirectory = Path.Combine(baseDirectory, fileName);
        _filePath = Path.Combine(dataDirectory, fileName);
    }

    public static void EnsureInitialized(string dataDirector, string filePath)
    {
        if(!Directory.Exists(dataDirector))
            Directory.CreateDirectory(dataDirector);

        if (!File.Exists(filePath))
            File.WriteAllText(filePath, "[]");
    }

    public async ValueTask<IReadOnlyList<T>> ReadAsync<T>(CancellationToken cancellationToken = default)
    {
        try
        {
            await using FileStream stream = File.OpenRead(_filePath);
            IReadOnlyList<T>? content = await JsonSerializer.DeserializeAsync<IReadOnlyList<T>>(stream, _jsonOptions, cancellationToken);
            return content ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task WriteAsync<T>(IEnumerable<T> products, CancellationToken cancellationToken = default)
    {
        await using FileStream stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, products, _jsonOptions, cancellationToken);
    }
}
