using ByCoders.CNAB.Application.Files.CNAB.Upload;
using ByCoders.CNAB.UnitTests.Builders.Application;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Xunit;

namespace ByCoders.CNAB.UnitTests.Application;

public class UploadCNABFileRequestValidatorTests
{
    private readonly UploadCNABFileRequestRequestValidator _validator;

    public UploadCNABFileRequestValidatorTests()
    {
        _validator = new UploadCNABFileRequestRequestValidator();
    }

    [Fact]
    public void Validate_WhenFileIsValid_ShouldReturnSuccess()
    {
        // Arrange
        var request = UploadCNABFileRequestBuilder.New
            .WithFileName("test.txt")
            .Build();

        // Act
        var result = _validator.TryValidate(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.FailureDetails.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WhenFileIsNull_ShouldReturnFailure()
    {
        // Arrange
        var request = UploadCNABFileRequestBuilder.New
            .WithNullFile()
            .Build();

        // Act
        var result = _validator.TryValidate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.FailureDetails.Should().NotBeEmpty();
    }

    [Fact]
    public void Validate_WhenFileIsEmpty_ShouldReturnFailure()
    {
        // Arrange
        var request = UploadCNABFileRequestBuilder.New
            .WithFileName("test.txt")
            .WithEmptyFile()
            .Build();

        // Act
        var result = _validator.TryValidate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.FailureDetails.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("test.pdf")]
    [InlineData("test.doc")]
    [InlineData("test.exe")]
    [InlineData("test")]
    [InlineData("test.TXT")] // Should fail as validator uses ToLowerInvariant
    public void Validate_WhenFileExtensionIsNotTxt_ShouldReturnFailure(string fileName)
    {
        // Arrange
        var request = UploadCNABFileRequestBuilder.New
            .WithFileName(fileName)
            .WithContent("test content")
            .Build();

        // Act
        var result = _validator.TryValidate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.FailureDetails.Should().Contain(x => x.Description != null && 
            x.Description.Contains("Invalid file type"));
    }

    [Theory]
    [InlineData("cnab.txt")]
    [InlineData("CNAB_20240101.txt")]
    [InlineData("file.with.dots.txt")]
    [InlineData("文件.txt")] // Unicode filename
    public void Validate_WhenFileNameIsValid_ShouldReturnSuccess(string fileName)
    {
        // Arrange
        var request = UploadCNABFileRequestBuilder.New
            .WithFileName(fileName)
            .Build();

        // Act
        var result = _validator.TryValidate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WhenFileSizeIsLarge_ShouldReturnSuccess()
    {
        // Arrange
        var request = UploadCNABFileRequestBuilder.New
            .WithFileName("large.txt")
            .WithLargeFile(10) // 10MB
            .Build();

        // Act
        var result = _validator.TryValidate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WhenMultipleValidationsFail_ShouldReturnAllErrors()
    {
        // Arrange
        var request = UploadCNABFileRequestBuilder.New
            .WithFileName("test.pdf")
            .WithEmptyFile()
            .Build();

        // Act
        var result = _validator.TryValidate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.FailureDetails.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void Validate_WithValidCNABContent_ShouldReturnSuccess()
    {
        // Arrange
        var request = UploadCNABFileRequestBuilder.New
            .WithValidCNABContent()
            .Build();

        // Act
        var result = _validator.TryValidate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithMixedContent_ShouldReturnSuccess()
    {
        // Arrange
        var request = UploadCNABFileRequestBuilder.New
            .WithMixedValidAndInvalidContent()
            .Build();

        // Act
        var result = _validator.TryValidate(request);

        // Assert
        result.IsValid.Should().BeTrue();
        // Validator only checks file type, not content validity
    }

    [Theory]
    [InlineData("application/pdf")]
    [InlineData("application/msword")]
    [InlineData("image/jpeg")]
    [InlineData("video/mp4")]
    public void Validate_WithDifferentContentTypes_ShouldStillCheckExtension(string contentType)
    {
        // Arrange
        var request = UploadCNABFileRequestBuilder.New
            .WithFileName("file.txt")
            .WithContentType(contentType)
            .Build();

        // Act
        var result = _validator.TryValidate(request);

        // Assert
        result.IsValid.Should().BeTrue();
        // Validator checks file extension, not content type
    }

    [Fact]
    public void Validate_WithRandomData_ShouldReturnSuccess()
    {
        // Arrange
        var request = UploadCNABFileRequestBuilder.New
            .WithRandomData()
            .Build();

        // Act
        var result = _validator.TryValidate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(1)]     // 1 byte
    [InlineData(100)]   // 100 bytes
    [InlineData(1024)]  // 1 KB
    [InlineData(1048576)] // 1 MB
    public void Validate_WithVariousFileSizes_ShouldReturnSuccess(long fileSize)
    {
        // Arrange
        var content = new string('A', (int)Math.Min(fileSize, int.MaxValue));
        var request = UploadCNABFileRequestBuilder.New
            .WithFileName("test.txt")
            .WithContent(content)
            .Build();

        // Act
        var result = _validator.TryValidate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithInvalidExtension_UsingBuilder_ShouldReturnFailure()
    {
        // Arrange
        var request = UploadCNABFileRequestBuilder.New
            .WithInvalidExtension()
            .Build();

        // Act
        var result = _validator.TryValidate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.FailureDetails.Should().Contain(x => x.Description!.Contains("Invalid file type"));
    }
}
