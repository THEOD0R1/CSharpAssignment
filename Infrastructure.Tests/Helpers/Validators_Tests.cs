using Infrastructure.Helpers;

namespace Infrastructure.Tests.Helpers;

public class Validators_Tests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Name_Invalid_ReturnsFailed(string? input)
    {
        var result = Validators.Name(input);
        Assert.False(result.IsSuccess);
        Assert.Equal("Please enter a Name.", result.Message);
    }

    [Fact]
    public void Name_Valid_ReturnsSuccess()
    {
        var result = Validators.Name("TestName");
        Assert.True(result.IsSuccess);
        Assert.Equal("TestName", result.Content);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("abc")]
    public void StrToDecimal_Invalid_ReturnsFailed(string? input)
    {
        var result = Validators.StrToDecimal(input);
        Assert.False(result.IsSuccess);
        Assert.Equal("Please enter a valid number", result.Message);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("45,67")]
    [InlineData("0")]
    public void StrToDecimal_Valid_ReturnsSuccess(string input)
    {
        var result = Validators.StrToDecimal(input);
        Assert.True(result.IsSuccess);
        Assert.Equal(decimal.Parse(input), ((ValidatorResponse<decimal>)result).Content);
    }
}