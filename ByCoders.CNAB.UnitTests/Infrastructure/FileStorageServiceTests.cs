using ByCoders.CNAB.API.Configurations;
using ByCoders.CNAB.Infrastructure.Storage;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Text;
using Xunit;

namespace ByCoders.CNAB.UnitTests.Infrastructure;

public class FileStorageServiceTests : IDisposable
{
    private readonly ILogger<FileStorageService> _logger;
    private readonly FileStorageService _fileStorageService;
    private readonly string _testStoragePath;
    private readonly FileStorageConfiguration _configuration;

    public FileStorageServiceTests()
    {
        _testStoragePath = Path.Combine(Path.GetTempPath(), $"FileStorageTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testStoragePath);

        _configuration = new FileStorageConfiguration { StoragePath = _testStoragePath };
        var options = Options.Create(_configuration);
        
        _logger = Substitute.For<ILogger<FileStorageService>>();
        _fileStorageService = new FileStorageService(options, _logger);
    }

    [Fact]
    public async Task SaveFileAsync_WhenValidInput_ShouldSaveFileAndReturnPath()
    {
        // Arrange
        const string fileName = "test.txt";
        const string fileContent = "Test content for CNAB file";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

        // Act
        var result = await _fileStorageService.SaveFileAsync(fileName, stream, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Value.Should().NotBeNullOrWhiteSpace();
        result.Value.Should().Contain("cnab-files");
        result.Value.Should().Contain(fileName);

        var savedFilePath = Path.Combine(_testStoragePath, result.Value);
        File.Exists(savedFilePath).Should().BeTrue();
        
        var savedContent = await File.ReadAllTextAsync(savedFilePath);
        savedContent.Should().Be(fileContent);
    }

    [Fact]
    public async Task SaveFileAsync_WhenFileNameIsNull_ShouldReturnFailure()
    {
        // Arrange
        string? nullFileName = null;
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("content"));

        // Act
        var result = await _fileStorageService.SaveFileAsync(nullFileName!, stream, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.FailureDetails.Should().Contain(x => x.Description != null && x.Description.Contains("Invalid file name"));
    }

    [Fact]
    public async Task SaveFileAsync_WhenFileStreamIsNull_ShouldReturnFailure()
    {
        // Arrange
        const string fileName = "test.txt";
        Stream? nullStream = null;

        // Act
        var result = await _fileStorageService.SaveFileAsync(fileName, nullStream!, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.FailureDetails.Should().Contain(x => x.Description != null && x.Description.Contains("Invalid file stream"));
    }

    [Fact]
    public async Task SaveFileAsync_WhenMultipleFilesWithSameName_ShouldCreateUniqueFiles()
    {
        // Arrange
        const string fileName = "duplicate.txt";
        var stream1 = new MemoryStream(Encoding.UTF8.GetBytes("Content 1"));
        var stream2 = new MemoryStream(Encoding.UTF8.GetBytes("Content 2"));

        // Act
        var result1 = await _fileStorageService.SaveFileAsync(fileName, stream1, CancellationToken.None);
        await Task.Delay(10); // Small delay to ensure different timestamps
        var result2 = await _fileStorageService.SaveFileAsync(fileName, stream2, CancellationToken.None);

        // Assert
        result1.Succeeded.Should().BeTrue();
        result2.Succeeded.Should().BeTrue();
        result1.Value.Should().NotBe(result2.Value);

        File.Exists(Path.Combine(_testStoragePath, result1.Value)).Should().BeTrue();
        File.Exists(Path.Combine(_testStoragePath, result2.Value)).Should().BeTrue();
    }

    [Fact]
    public void ReadFile_WhenFileExists_ShouldReturnFileStream()
    {
        // Arrange
        const string fileContent = "CNAB line content";
        const string relativePath = "cnab-files/test-read.txt";
        var fullPath = Path.Combine(_testStoragePath, relativePath);
        
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        File.WriteAllText(fullPath, fileContent);

        // Act
        var result = _fileStorageService.ReadFile(relativePath, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Value.Should().NotBeNull();
        
        using var reader = new StreamReader(result.Value);
        var readContent = reader.ReadToEnd();
        readContent.Should().Be(fileContent);
        
        result.Value.Dispose();
    }

    [Fact]
    public void ReadFile_WhenFileDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        const string nonExistentPath = "cnab-files/non-existent.txt";

        // Act
        var result = _fileStorageService.ReadFile(nonExistentPath, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.FailureDetails.Should().Contain(x => x.Description != null && x.Description.Contains("File not found"));
    }

    [Fact]
    public void ReadFile_WhenPathIsNull_ShouldReturnFailure()
    {
        // Arrange
        string? nullPath = null;

        // Act
        var result = _fileStorageService.ReadFile(nullPath!, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.FailureDetails.Should().Contain(x => x.Description != null && x.Description.Contains("Invalid file path"));
    }

    [Fact]
    public void FileExists_WhenFileExists_ShouldReturnTrue()
    {
        // Arrange
        const string relativePath = "cnab-files/exists.txt";
        var fullPath = Path.Combine(_testStoragePath, relativePath);
        
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        File.WriteAllText(fullPath, "content");

        // Act
        var exists = _fileStorageService.FileExists(relativePath);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public void FileExists_WhenFileDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        const string nonExistentPath = "cnab-files/does-not-exist.txt";

        // Act
        var exists = _fileStorageService.FileExists(nonExistentPath);

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public void FileExists_WhenPathIsNullOrEmpty_ShouldReturnFalse()
    {
        // Arrange & Act & Assert
        _fileStorageService.FileExists(null!).Should().BeFalse();
        _fileStorageService.FileExists("").Should().BeFalse();
        _fileStorageService.FileExists("  ").Should().BeFalse();
    }

    [Fact]
    public async Task SaveFileAsync_WhenDirectoryDoesNotExist_ShouldCreateDirectory()
    {
        // Arrange
        const string fileName = "nested/folder/test.txt";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("content"));

        // Act
        var result = await _fileStorageService.SaveFileAsync(fileName, stream, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        
        var savedFilePath = Path.Combine(_testStoragePath, result.Value);
        File.Exists(savedFilePath).Should().BeTrue();
        
        var directory = Path.GetDirectoryName(savedFilePath);
        Directory.Exists(directory).Should().BeTrue();
    }

    [Fact]
    public async Task SaveFileAsync_WhenLargeFile_ShouldSaveSuccessfully()
    {
        // Arrange
        const string fileName = "large-file.txt";
        var largeContent = new byte[10 * 1024 * 1024]; // 10MB
        new Random().NextBytes(largeContent);
        var stream = new MemoryStream(largeContent);

        // Act
        var result = await _fileStorageService.SaveFileAsync(fileName, stream, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        
        var savedFilePath = Path.Combine(_testStoragePath, result.Value);
        var fileInfo = new FileInfo(savedFilePath);
        fileInfo.Exists.Should().BeTrue();
        fileInfo.Length.Should().Be(largeContent.Length);
    }

    [Fact]
    public void ReadFile_WhenFileLocked_ShouldStillBeAbleToRead()
    {
        // Arrange
        const string relativePath = "cnab-files/locked-file.txt";
        var fullPath = Path.Combine(_testStoragePath, relativePath);
        
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        File.WriteAllText(fullPath, "locked content");

        // Open file with shared read access
        using var lockingStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);

        // Act
        var result = _fileStorageService.ReadFile(relativePath, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Dispose();
    }

    public void Dispose()
    {
        // Clean up test directory
        if (Directory.Exists(_testStoragePath))
        {
            try
            {
                Directory.Delete(_testStoragePath, true);
            }
            catch
            {
                // Best effort cleanup
            }
        }
    }
}
