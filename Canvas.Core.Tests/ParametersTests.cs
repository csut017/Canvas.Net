using System;

namespace Canvas.Core.Tests;

[TestSubject(typeof(Parameters))]
public class ParametersTests
{
    private enum TestEnum
    {
        Value,
    }

    [Flags]
    private enum TestFlagsEnum
    {
        None = 0,
        First = 1,
        Third = 4,
    }

    [Theory]
    [InlineData(true, "true")]
    [InlineData(false, "false")]
    public void AddHandlesBoolean(bool input, string expected)
    {
        // Arrange and act
        var parameters = new Parameters {
            { "bool", input }
        };

        // Assert
        parameters.ShouldContain(p => p.Name == "bool" && p.Value == expected);
    }

    [Fact]
    public void AddHandlesEnum()
    {
        // Arrange and act
        var parameters = new Parameters
        {
            { "single", TestEnum.Value },
        };

        // Assert
        parameters.ShouldContain(p => p.Name == "single" && p.Value == "value");
    }

    [Fact]
    public void AddHandlesInt32()
    {
        // Arrange and act
        var parameters = new Parameters {
            { "int", 5 }
        };

        // Assert
        parameters.ShouldContain(p => p.Name == "int" && p.Value == "5");
    }

    [Fact]
    public void AddHandlesMultipleFlagsEnums()
    {
        // Arrange and act
        var parameters = new Parameters
        {
            { "include[]", TestFlagsEnum.First | TestFlagsEnum.Third },
        };

        // Assert
        parameters.ShouldContain(p => p.Name == "include[]" && p.Value == "first");
        parameters.ShouldContain(p => p.Name == "include[]" && p.Value == "third");
    }

    [Fact]
    public void AddHandlesSingleFlagsEnum()
    {
        // Arrange and act
        var parameters = new Parameters
        {
            { "include[]", TestFlagsEnum.First },
        };

        // Assert
        parameters.ShouldContain(p => p.Name == "include[]" && p.Value == "first");
    }

    [Fact]
    public void AddHandlesString()
    {
        // Arrange and act
        var parameters = new Parameters {
            { "string", "value" }
        };

        // Assert
        parameters.ShouldContain(p => p.Name == "string" && p.Value == "value");
    }

    [Fact]
    public void AddHandlesUnsignedLong()
    {
        // Arrange and act
        const ulong value = 123456;
        var parameters = new Parameters {
            { "int", value }
        };

        // Assert
        parameters.ShouldContain(p => p.Name == "int" && p.Value == value.ToString());
    }

    [Fact]
    public void AddIgnoresDefaultValue()
    {
        // Arrange and act
        var parameters = new Parameters
        {
            { "include[]", TestFlagsEnum.None },
        };

        // Assert
        parameters.ShouldNotContain(p => p.Name == "include[]" && p.Value == "none");
    }

    [Fact]
    public void ToStringHandlesEmptyParameters()
    {
        // act
        var asString = new Parameters().ToString();

        // Assert
        asString.ShouldBeEmpty();
    }

    [Fact]
    public void ToStringHandlesMultipleParameters()
    {
        // Arrange
        var parameters = new Parameters
        {
            { "one", 1 },
            { "two", "second" },
        };

        // act
        var asString = parameters.ToString();

        // Assert
        asString.ShouldBe("?one=1&two=second");
    }

    [Fact]
    public void ToStringHandlesOneParameter()
    {
        // Arrange
        var parameters = new Parameters
        {
            { "one", 1 },
        };

        // act
        var asString = parameters.ToString();

        // Assert
        asString.ShouldBe("?one=1");
    }

    [Fact]
    public void ToStringHandlesWebEncoding()
    {
        // Arrange
        var parameters = new Parameters
        {
            { "encoded", "1 = 2" },
        };

        // act
        var asString = parameters.ToString();

        // Assert
        asString.ShouldBe("?encoded=1+%3D+2");
    }
}