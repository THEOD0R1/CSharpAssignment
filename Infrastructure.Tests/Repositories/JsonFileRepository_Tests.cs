using Infrastructure.Repositories;

namespace Infrastructure.Tests.Repositories;


// Created by ChatGPT with some small changes made by me.

// Testerna verifierar funktionaliteten i JsonFileRepository.
// Detta testas:
// - Att data kan skrivas till och läsas från en JSON-fil.
// - Att ett objekt kan uppdateras korrekt i filen.
// - Att ett objekt kan tas bort från filen.
// - Att läsning returnerar rätt data efter skrivning.
//
// Varje test använder en tillfällig JSON-fil.



public class JsonFileRepositoryTests : IDisposable
{
    private readonly string _tempFilePath;
    private readonly JsonFileRepository _repository;

    public JsonFileRepositoryTests()
    {
        string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDirectory);
        _tempFilePath = Path.Combine(tempDirectory, "test-data.json");

        File.WriteAllText(_tempFilePath, "[]"); // initialize empty file
        _repository = new JsonFileRepository("test-data.json");
        typeof(JsonFileRepository)
            .GetField("_filePath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .SetValue(_repository, _tempFilePath);
    }

    public static TheoryData<TestItem[]> GetSampleData()
    {
        var data = new TheoryData<TestItem[]>
        {
            ([new TestItem(1, "Alpha")]),
            ([new TestItem(1, "Alpha"), new TestItem(2, "Beta")])
        };
        return data;
    }

    [Theory]
    [MemberData(nameof(GetSampleData))]
    public async Task WriteAsync_ShouldWriteAndReadBackData(TestItem[] items)
    {
        // Act
        var writeResult = await _repository.WriteAsync(items.ToList());
        var readResult = await _repository.ReadAsync<TestItem>();

        // Assert
        Assert.True(writeResult.Success);
        Assert.NotNull(readResult.Content);
        Assert.Equal(items.Length, readResult.Content.Count);
        Assert.Equal(items.First().Name, readResult.Content.First().Name);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReplaceItem()
    {
        // Arrange
        var initial = new List<TestItem> { new(1, "Alice"), new(2, "Bob") };
        await _repository.WriteAsync(initial);

        // Act
        var updatedItem = new TestItem(2, "Bobby");
        var result = await _repository.UpdateAsync<TestItem>(x => x.Id == 2, updatedItem);

        // Assert
        Assert.True(result.Success);

        var afterUpdate = (await _repository.ReadAsync<TestItem>()).Content!;
        Assert.Equal("Bobby", afterUpdate.First(x => x.Id == 2).Name);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveItem()
    {
        // Arrange
        var initial = new List<TestItem> { new(1, "Alice"), new(2, "Bob") };
        await _repository.WriteAsync(initial);

        // Act
        var result = await _repository.DeleteAsync<TestItem>(x => x.Id == 1);

        // Assert
        Assert.True(result.Success);

        var afterDelete = (await _repository.ReadAsync<TestItem>()).Content!;
        Assert.Single(afterDelete);
        Assert.Equal(2, afterDelete[0].Id);
    }

    [Fact]
    public async Task ReadAsync_ShouldReturnWrittenData()
    {
        // Arrange
        var itemsToWrite = new List<TestItem>
    {
        new(1, "Alice"),
        new(2, "Bob")
    };
        await _repository.WriteAsync(itemsToWrite);

        // Act
        var readResult = await _repository.ReadAsync<TestItem>();

        // Assert
        Assert.True(readResult.Success);
        Assert.NotNull(readResult.Content);
        Assert.Equal(itemsToWrite.Count, readResult.Content.Count);
        Assert.Equal("Alice", readResult.Content[0].Name);
        Assert.Equal("Bob", readResult.Content[1].Name);
    }

    public void Dispose()
    {
        if (File.Exists(_tempFilePath))
            File.Delete(_tempFilePath);

        GC.SuppressFinalize(this);
    }

    public record TestItem(int Id, string Name);
}
