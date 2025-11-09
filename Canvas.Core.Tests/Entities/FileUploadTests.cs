using Canvas.Core.Entities;
using System.IO;

namespace Canvas.Core.Tests.Entities;

[TestSubject(typeof(FileUpload))]
public class FileUploadTests
{
    [Fact]
    public void GenerateUploadArgsIncludesAllItems()
    {
        // Arrange
        var item = new FileUpload
        {
            Name = "the_file.txt",
            Stream = new MemoryStream(),
            Type = "the-type",
            Size = 1234,
        };

        // Act
        var args = item.GenerateUploadArgs();

        // Assert
        args.ShouldContainKeyAndValue("name", "the_file.txt");
        args.ShouldContainKeyAndValue("size", "1234");
        args.ShouldContainKeyAndValue("content_type", "the-type");
    }

    [Fact]
    public void GenerateUploadArgsIncludesNames()
    {
        // Arrange
        var item = new FileUpload
        {
            Name = "the_file.txt",
            Stream = new MemoryStream(),
        };

        // Act
        var args = item.GenerateUploadArgs();

        // Assert
        args.ShouldContainKeyAndValue("name", "the_file.txt");
        args.ShouldNotContainKey("size");
        args.ShouldNotContainKey("content_type");
    }

    [Fact]
    public void NewViaFileInfoChecksThatFileExists()
    {
        // Arrange
        var path = Path.Combine(Path.GetTempPath(), "should_not_exist.txt");
        if (File.Exists(path)) File.Delete(path);

        // Act
        var ex = Assert.Throws<FileNotFoundException>(() => FileUpload.New(new FileInfo(path)));

        // Assert
        ex.FileName.ShouldBe(path);
    }

    [Fact]
    public void NewViaFileInfoOpensFile()
    {
        // Arrange
        var path = Path.Combine(Path.GetTempPath(), "temp_file.txt");
        try
        {
            File.WriteAllText(path, "Some Data");

            // Act
            using var item = FileUpload.New(new FileInfo(path));

            // Assert
            item.ShouldSatisfyAllConditions(
                () => item.Name.ShouldBe(Path.GetFileName(path)),
                () => item.Size.ShouldBe(9),
                () => item.Type.ShouldBeNull()
            );

            var reader = new StreamReader(item.Stream);
            var actual = reader.ReadToEnd();
            actual.ShouldBe("Some Data");
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    [Fact]
    public void NewViaFileInfoSetsType()
    {
        // Arrange
        var path = Path.Combine(Path.GetTempPath(), "temp_file.txt");
        try
        {
            File.WriteAllText(path, "Some Data");

            // Act
            using var item = FileUpload.New(new FileInfo(path), "the-type");

            // Assert
            item.ShouldSatisfyAllConditions(
                () => item.Name.ShouldBe(Path.GetFileName(path)),
                () => item.Size.ShouldBe(9),
                () => item.Type.ShouldBe("the-type")
            );

            var reader = new StreamReader(item.Stream);
            var actual = reader.ReadToEnd();
            actual.ShouldBe("Some Data");
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    [Fact]
    public void NewViaFilePathChecksThatFileExists()
    {
        // Arrange
        var path = Path.Combine(Path.GetTempPath(), "should_not_exist.txt");
        if (File.Exists(path)) File.Delete(path);

        // Act
        var ex = Assert.Throws<FileNotFoundException>(() => FileUpload.New(path));

        // Assert
        ex.FileName.ShouldBe(path);
    }

    [Fact]
    public void NewViaFilePathOpensFile()
    {
        // Arrange
        var path = Path.Combine(Path.GetTempPath(), "temp_file.txt");
        try
        {
            File.WriteAllText(path, "Some Data");

            // Act
            using var item = FileUpload.New(path);

            // Assert
            item.ShouldSatisfyAllConditions(
                () => item.Name.ShouldBe(Path.GetFileName(path)),
                () => item.Size.ShouldBe(9),
                () => item.Type.ShouldBeNull()
            );

            var reader = new StreamReader(item.Stream);
            var actual = reader.ReadToEnd();
            actual.ShouldBe("Some Data");
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    [Fact]
    public void NewViaFilePathSetsType()
    {
        // Arrange
        var path = Path.Combine(Path.GetTempPath(), "temp_file.txt");
        try
        {
            File.WriteAllText(path, "Some Data");

            // Act
            using var item = FileUpload.New(path, "the-type");

            // Assert
            item.ShouldSatisfyAllConditions(
                () => item.Name.ShouldBe(Path.GetFileName(path)),
                () => item.Size.ShouldBe(9),
                () => item.Type.ShouldBe("the-type")
            );

            var reader = new StreamReader(item.Stream);
            var actual = reader.ReadToEnd();
            actual.ShouldBe("Some Data");
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }
}