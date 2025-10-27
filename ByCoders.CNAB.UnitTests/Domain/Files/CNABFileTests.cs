using ByCoders.CNAB.Domain.Files.Models;
using FluentAssertions;
using Xunit;

namespace ByCoders.CNAB.UnitTests.Domain.Files;

/// <summary>
/// Testes unitários para o agregado CNABFile
/// </summary>
public class CNABFileTests
{
    [Fact]
    public void CNABFile_WhenCreatedWithValidParameters_ShouldInitializeWithCorrectDefaultValues()
    {
        // Arrange
        const string expectedFileName = "CNAB.txt";
        const string expectedFilePath = "/storage/cnab-files/20241024_123456_abc123_CNAB.txt";
        const long expectedFileSize = 1024L;

        // Act
        var cnabFile = new CNABFile(expectedFileName, expectedFilePath, expectedFileSize);

        // Assert
        cnabFile.FileName.Should().Be(expectedFileName);
        cnabFile.FilePath.Should().Be(expectedFilePath);
        cnabFile.FileSize.Should().Be(expectedFileSize);
        cnabFile.UploadedOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        cnabFile.TransactionCount.Should().Be(0);
        cnabFile.ProcessedOn.Should().BeNull();
        cnabFile.FailedOn.Should().BeNull();
        cnabFile.ErrorMessage.Should().BeNull();
        cnabFile.Status.Should().Be(CNABFileStatus.Uploaded);
        cnabFile.Id.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void CNABFile_WhenCreatedWithInvalidFileName_ShouldCreateWithoutValidation(string? invalidFileName)
    {
        // Note: The current CNABFile implementation doesn't validate constructor parameters
        // This test documents the actual behavior
        
        // Act
        var cnabFile = new CNABFile(invalidFileName!, "/path/to/file", 1024);

        // Assert
        cnabFile.Should().NotBeNull();
        cnabFile.FileName.Should().Be(invalidFileName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void CNABFile_WhenCreatedWithInvalidFilePath_ShouldCreateWithoutValidation(string? invalidFilePath)
    {
        // Note: The current CNABFile implementation doesn't validate constructor parameters
        // This test documents the actual behavior
        
        // Act
        var cnabFile = new CNABFile("test.txt", invalidFilePath!, 1024);

        // Assert
        cnabFile.Should().NotBeNull();
        cnabFile.FilePath.Should().Be(invalidFilePath);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(-9999)]
    public void CNABFile_WhenCreatedWithInvalidFileSize_ShouldCreateWithoutValidation(long invalidFileSize)
    {
        // Note: The current CNABFile implementation doesn't validate constructor parameters
        // This test documents the actual behavior
        
        // Act
        var cnabFile = new CNABFile("test.txt", "/path/to/file", invalidFileSize);

        // Assert
        cnabFile.Should().NotBeNull();
        cnabFile.FileSize.Should().Be(invalidFileSize);
    }

    [Fact]
    public void Processed_WhenCalledWithValidTransactionCount_ShouldUpdateStatusAndMetadata()
    {
        // Arrange
        var uploadedFile = new CNABFile("test.txt", "/path/to/file", 1024);
        const int expectedTransactionCount = 21;

        // Act
        uploadedFile.Processed(expectedTransactionCount);

        // Assert
        uploadedFile.Status.Should().Be(CNABFileStatus.Processed);
        uploadedFile.TransactionCount.Should().Be(expectedTransactionCount);
        uploadedFile.ProcessedOn.Should().NotBeNull();
        uploadedFile.ProcessedOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        uploadedFile.FailedOn.Should().BeNull();
        uploadedFile.ErrorMessage.Should().BeNull();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    [InlineData(-999)]
    public void Processed_WhenCalledWithNegativeTransactionCount_ShouldReturnFailure(int negativeCount)
    {
        // Arrange
        var uploadedFile = new CNABFile("test.txt", "/path/to/file", 1024);

        // Act
        var result = uploadedFile.Processed(negativeCount);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.FailureDetails.Should().Contain(x => x.Description != null && x.Description.Contains("Transaction count cannot be negative"));
    }

    [Fact]
    public void Processed_WhenFileAlreadyProcessed_ShouldReturnSuccess()
    {
        // Arrange
        var alreadyProcessedFile = new CNABFile("test.txt", "/path/to/file", 1024);
        alreadyProcessedFile.Processed(10); // Process first time

        // Act
        var result = alreadyProcessedFile.Processed(20);

        // Assert
        result.Succeeded.Should().BeTrue(); // Returns success without modifying
        alreadyProcessedFile.TransactionCount.Should().Be(10); // Original count remains
    }

    [Fact]
    public void Processed_AfterFailedState_ShouldSuccessfullyProcessAndClearErrorState()
    {
        // Arrange
        var failedFile = new CNABFile("test.txt", "/path/to/file", 1024);
        failedFile.Failed("Parse error at line 10");
        const int transactionCountAfterReprocessing = 15;

        // Act
        var result = failedFile.Processed(transactionCountAfterReprocessing);

        // Assert
        result.Succeeded.Should().BeTrue();
        failedFile.Status.Should().Be(CNABFileStatus.Processed);
        failedFile.FailedOn.Should().BeNull();
        failedFile.ErrorMessage.Should().BeNull();
        failedFile.ProcessedOn.Should().NotBeNull();
        failedFile.TransactionCount.Should().Be(transactionCountAfterReprocessing);
    }

    [Fact]
    public void Failed_WhenCalledWithValidErrorMessage_ShouldUpdateStatusAndErrorDetails()
    {
        // Arrange
        var uploadedFile = new CNABFile("test.txt", "/path/to/file", 1024);
        const string expectedErrorMessage = "Parse error on line 5: Invalid transaction type";

        // Act
        uploadedFile.Failed(expectedErrorMessage);

        // Assert
        uploadedFile.Status.Should().Be(CNABFileStatus.Failed);
        uploadedFile.ErrorMessage.Should().Be(expectedErrorMessage);
        uploadedFile.FailedOn.Should().NotBeNull();
        uploadedFile.FailedOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        uploadedFile.ProcessedOn.Should().BeNull();
        uploadedFile.TransactionCount.Should().Be(0);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Failed_WhenCalledWithInvalidErrorMessage_ShouldReturnFailure(string? invalidErrorMessage)
    {
        // Arrange
        var uploadedFile = new CNABFile("test.txt", "/path/to/file", 1024);

        // Act
        var result = uploadedFile.Failed(invalidErrorMessage!);

        // Assert
        result.Succeeded.Should().BeFalse();
    }

    [Fact]
    public void Failed_WhenFileAlreadyFailed_ShouldReturnSuccess()
    {
        // Arrange
        var alreadyFailedFile = new CNABFile("test.txt", "/path/to/file", 1024);
        alreadyFailedFile.Failed("Initial parsing error");

        // Act
        var result = alreadyFailedFile.Failed("Another error occurred");

        // Assert
        result.Succeeded.Should().BeTrue();
        alreadyFailedFile.ErrorMessage.Should().Be("Another error occurred"); 
    }


    [Fact]
    public void Status_ShouldReflectCorrectStateBasedOnFileHistory()
    {
        // Arrange & Act & Assert - Uploaded Status
        var uploadedFile = new CNABFile("test.txt", "/path/to/file", 1024);
        uploadedFile.Status.Should().Be(CNABFileStatus.Uploaded);

        // Arrange & Act & Assert - Processed Status
        var processedFile = new CNABFile("test.txt", "/path/to/file", 1024);
        processedFile.Processed(10);
        processedFile.Status.Should().Be(CNABFileStatus.Processed);

        // Arrange & Act & Assert - Failed Status
        var failedFile = new CNABFile("test.txt", "/path/to/file", 1024);
        failedFile.Failed("Validation error");
        failedFile.Status.Should().Be(CNABFileStatus.Failed);
    }

    [Fact]
    public void Processing_WhenFileUploaded_ShouldUpdateStatus()
    {
        // Arrange
        var uploadedFile = new CNABFile("test.txt", "/path/to/file", 1024);

        // Act
        var result = uploadedFile.Processing();

        // Assert
        result.Succeeded.Should().BeTrue();
        uploadedFile.Status.Should().Be(CNABFileStatus.Processing);
        uploadedFile.ProcessingStartedOn.Should().NotBeNull();
        uploadedFile.ProcessingStartedOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Processing_WhenAlreadyProcessed_ShouldReturnSuccess()
    {
        // Arrange
        var processedFile = new CNABFile("test.txt", "/path/to/file", 1024);
        processedFile.Processed(10);

        // Act
        var result = processedFile.Processing();

        // Assert
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void CNABFile_WithSmallFileSize_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var smallFile = new CNABFile("small.txt", "/path/to/small", 100L);

        // Assert
        smallFile.FileSize.Should().Be(100L);
    }

    [Fact]
    public void CNABFile_WithLargeFileSize_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var largeFile = new CNABFile("large.txt", "/path/to/large", 10485760L);

        // Assert
        largeFile.FileSize.Should().Be(10485760L); // 10MB
    }

    [Fact]
    public void CNABFile_WithRandomData_ShouldCreateValidFile()
    {
        // Arrange & Act
        var randomFile = new CNABFile(
            $"CNAB_{DateTime.Now:yyyyMMdd_HHmmss}.txt",
            $"/storage/cnab-files/CNAB_{Guid.NewGuid()}.txt",
            Random.Shared.Next(100, 100000));

        // Assert
        randomFile.Should().NotBeNull();
        randomFile.FileName.Should().NotBeNullOrWhiteSpace();
        randomFile.FileName.Should().Contain("CNAB_");
        randomFile.FilePath.Should().NotBeNullOrWhiteSpace();
        randomFile.FilePath.Should().Contain("/storage/cnab-files/");
        randomFile.FileSize.Should().BeGreaterThan(0);
        randomFile.Status.Should().Be(CNABFileStatus.Uploaded);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(999)]
    public void Processed_WithVariousTransactionCounts_ShouldSetCorrectCount(int transactionCount)
    {
        // Arrange
        var file = new CNABFile("test.txt", "/path/to/file", 1024);

        // Act
        file.Processed(transactionCount);

        // Assert
        file.TransactionCount.Should().Be(transactionCount);
        file.Status.Should().Be(CNABFileStatus.Processed);
    }

    [Fact]
    public void Failed_WhenFileIsUploaded_ShouldTransitionToFailedState()
    {
        // Arrange
        var uploadedFile = new CNABFile("test.txt", "/path/to/file", 1024);
        
        // Act
        uploadedFile.Failed("File format is invalid");

        // Assert
        uploadedFile.Status.Should().Be(CNABFileStatus.Failed);
        uploadedFile.ErrorMessage.Should().Be("File format is invalid");
    }

    [Fact]
    public void Id_ShouldBeGeneratedAutomatically()
    {
        // Arrange & Act
        var file1 = new CNABFile("test1.txt", "/path/to/file1", 1024);
        var file2 = new CNABFile("test2.txt", "/path/to/file2", 2048);

        // Assert
        file1.Id.Should().NotBeEmpty();
        file2.Id.Should().NotBeEmpty();
        file1.Id.Should().NotBe(file2.Id);
    }
}
