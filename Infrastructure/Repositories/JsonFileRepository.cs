using Infrastructure.Models;
using System.Text.Json;

namespace Infrastructure.Repositories;

public interface IJsonFileRepository
{
    Task<ResponseResult> WriteAsync<T>(IEnumerable<T> jsonContent, CancellationToken cancellationToken = default);
    ValueTask<ResponseResult<IReadOnlyList<T>>> ReadAsync<T>(CancellationToken cancellationToken = default);

    Task<ResponseResult> UpdateAsync<T>(Func<T, bool> predicate, T updatedItem, CancellationToken cancellationToken = default);
    Task<ResponseResult> DeleteAsync<T>(Func<T, bool> predicate, CancellationToken cancellationToken = default);


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

        EnsureInitialized(dataDirectory, _filePath);
    }

    public static void EnsureInitialized(string dataDirector, string filePath)
    {
        if (!Directory.Exists(dataDirector))
            Directory.CreateDirectory(dataDirector);

        if (!File.Exists(filePath))
            File.WriteAllText(filePath, "[]");
    }

    public async ValueTask<ResponseResult<IReadOnlyList<T>>> ReadAsync<T>(CancellationToken cancellationToken = default)
    {
        try
        {
            await using FileStream stream = File.OpenRead(_filePath);
            IReadOnlyList<T> content = await JsonSerializer.DeserializeAsync<IReadOnlyList<T>>(stream, _jsonOptions, cancellationToken) ?? [];

            return ResponseResult<IReadOnlyList<T>>.Ok(content);
        }
        catch
        {
            return ResponseResult<IReadOnlyList<T>>.Fail(500);
        }
    }

    public async Task<ResponseResult> WriteAsync<T>(IEnumerable<T> jsonContent, CancellationToken cancellationToken = default)
    {
        try
        {
            await using FileStream stream = File.Create(_filePath);
            await JsonSerializer.SerializeAsync(stream, jsonContent, _jsonOptions, cancellationToken);

            return ResponseResult.Ok();
        }
        catch (Exception ex)
        {
            return ResponseResult.Fail(500, $"Unexpected error: {ex.Message}");
        }
    }

    public async Task<ResponseResult> UpdateAsync<T>(Func<T, bool> predicate, T updatedItem, CancellationToken cancellationToken = default)
    {
        try
        {
            var items = (await ReadAsync<T>(cancellationToken)).Content?.ToList();

            if(items == null)
                return ResponseResult.Fail(404, "File not found or empty");

            int index = items.FindIndex(item => predicate(item));

            if (index == -1) 
                return ResponseResult.Fail(404, "Item not found");

            items[index] = updatedItem;
            await WriteAsync(items, cancellationToken);

            return ResponseResult.Ok();
        }
        catch (Exception ex)
        {
            return ResponseResult.Fail(500, $"Failed to update item of type {typeof(T).Name}: {ex.Message}");
        }
        
    }

    public async Task<ResponseResult> DeleteAsync<T>(Func<T, bool> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            var items = (await ReadAsync<T>(cancellationToken)).Content?.ToList();

            if (items == null)
                return ResponseResult.Fail(404, "File not found or empty");

            items.RemoveAll(item => predicate(item));
            await WriteAsync(items, cancellationToken);

            return ResponseResult.Ok();
        }
        catch (Exception ex)
        {
            return ResponseResult.Fail(500, $"Failed to delte item of type {typeof(T).Name}: {ex.Message}");
        }
    
    }
}
