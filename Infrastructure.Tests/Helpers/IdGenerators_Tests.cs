using Infrastructure.Helpers.Generators;

namespace Infrastructure.Tests.Helpers;

public class IdGenerators_Tests
{
    [Fact]
    public void GUIDGenerator_ReturnsValidGuid()
    {
        var result = IdGenerators.GUIDGenerator();

        Assert.True(Guid.TryParse(result, out _));
    }

    [Fact]
    public void Product_ReturnsStringWithDefaultPrefix()
    {
        var result = IdGenerators.Product();

        Assert.StartsWith("pr-", result);
        Assert.True(Guid.TryParse(result[3..], out _));
    }

    [Theory]
    [InlineData("custom-")]
    [InlineData("test-")]
    [InlineData("product-")]
    [InlineData("ranDom")]
    public void Product_ReturnsStringWithCustomPrefix(string prefix)
    {
        var result = IdGenerators.Product(prefix);

        Assert.StartsWith(prefix, result);
        Assert.True(Guid.TryParse(result[prefix.Length..], out _));
    }

    [Fact]
    public void Product_GeneratesUniqueIds()
    {
        var id1 = IdGenerators.Product();
        var id2 = IdGenerators.Product();

        Assert.NotEqual(id1, id2);
    }

}
