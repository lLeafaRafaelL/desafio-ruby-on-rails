using ByCoders.CNAB.Application.Files.CNAB.Upload;
using ByCoders.CNAB.Core;
using ByCoders.CNAB.Core.Results;
using ByCoders.CNAB.Core.Validators;
using ByCoders.CNAB.Domain.Files;
using ByCoders.CNAB.Domain.Files.Models;
using ByCoders.CNAB.UnitTests.Builders.Application;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace ByCoders.CNAB.UnitTests.Application;

public class UploadCNABFileHandlerTests
{
    private readonly IFileStorageService _fileStorage;
    private readonly ICNABFileRepository _fileRepository;
    private readonly ILogger<UploadCNABFileHandler> _logger;
    private readonly IDtoValidator<UploadCNABFileRequest> _validator;
    private readonly UploadCNABFileHandler _handler;

    public UploadCNABFileHandlerTests()
    {
        _fileStorage = Substitute.For<IFileStorageService>();
        _fileRepository = Substitute.For<ICNABFileRepository>();
        _logger = Substitute.For<ILogger<UploadCNABFileHandler>>();
        _validator = Substitute.For<IDtoValidator<UploadCNABFileRequest>>();

        _handler = new UploadCNABFileHandler(
            _fileStorage,
            _fileRepository,
            _logger,
            _validator
        );
    }

    [Fact]
    public async Task HandleAsync_WhenValidationFails_ShouldReturnFailure()
    {
        // Arrange
        var request = UploadCNABFileRequestBuilder.New
            .WithNullFile()
            .Build();
        
        _validator.TryValidate(request).Returns(ValidationResult.Failed("File is required"));

        // Act
        var result = await _handler.HandleAsync(request, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.FailureDetails.Should().Contain(x => x.Description == "File is required");
        await _fileStorage.DidNotReceive().SaveFileAsync(Arg.Any<string>(), Arg.Any<Stream>(), Arg.Any<CancellationToken>());
        await _fileRepository.DidNotReceive().AddAsync(Arg.Any<CNABFile>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenFileStorageFails_ShouldReturnFailure()
    {
        // Arrange
        var request = UploadCNABFileRequestBuilder.New
            .WithFileName("test.txt")
            .WithContent("content")
            .Build();
            
        _validator.TryValidate(request).Returns(ValidationResult.Success());
        _fileStorage.SaveFileAsync(Arg.Any<string>(), Arg.Any<Stream>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<string>.Failure("Storage error")));

        // Act
        var result = await _handler.HandleAsync(request, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.FailureDetails.Should().Contain(x => x.Description != null && x.Description.Contains("Storage error"));
        await _fileRepository.DidNotReceive().AddAsync(Arg.Any<CNABFile>(), Arg.Any<CancellationToken>());
    }


    [Fact]
    public async Task HandleAsync_WhenSuccessful_ShouldCreateCNABFileAndReturn202Accepted()
    {
        // Arrange
        const string storedFilePath = "/storage/cnab-files/20240101_123456_abc123_CNAB.txt";
        
        var request = UploadCNABFileRequestBuilder.New
            .WithFileName("CNAB_20240101.txt")
            .WithValidCNABContent()
            .Build();
            
        _validator.TryValidate(request).Returns(ValidationResult.Success());
        
        _fileStorage.SaveFileAsync(request.File.FileName, Arg.Any<Stream>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<string>.Success(storedFilePath)));

        // Act
        var result = await _handler.HandleAsync(request, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Status.Should().Be(RequestHandlerStatus.Accepted);
        result.Value.Should().NotBeNull();
        result.Value!.FileName.Should().Be("CNAB_20240101.txt");
        result.Value.Status.Should().Be(CNABFileStatus.Uploaded);
        result.Value.FileId.Should().NotBeEmpty();

        // Handler's responsibility is to call repository methods, not test CNABFile properties
        await _fileRepository.Received(1).AddAsync(Arg.Any<CNABFile>(), Arg.Any<CancellationToken>());
        await _fileRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithLargeFile_ShouldProcessSuccessfully()
    {
        // Arrange
        var request = UploadCNABFileRequestBuilder.New
            .WithFileName("large_cnab.txt")
            .WithLargeFile(10) // 10MB file
            .Build();
        
        _validator.TryValidate(request).Returns(ValidationResult.Success());
        _fileStorage.SaveFileAsync(Arg.Any<string>(), Arg.Any<Stream>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<string>.Success("/storage/large_cnab.txt")));

        // Act
        var result = await _handler.HandleAsync(request, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Value!.FileName.Should().Be("large_cnab.txt");
        // Handler's responsibility is to add file, not to test CNABFile internal state
        await _fileRepository.Received(1).AddAsync(
            Arg.Any<CNABFile>(),
            Arg.Any<CancellationToken>()
        );
    }

    [Fact]
    public async Task HandleAsync_WhenCancellationRequested_ShouldRespectCancellation()
    {
        // Arrange
        var request = UploadCNABFileRequestBuilder.New.Build();
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        
        _validator.TryValidate(request).Returns(ValidationResult.Success());
        _fileStorage.SaveFileAsync(Arg.Any<string>(), Arg.Any<Stream>(), Arg.Any<CancellationToken>())
            .Returns(async (callInfo) =>
            {
                var token = callInfo.ArgAt<CancellationToken>(2);
                await Task.Delay(100, token);
                return Result<string>.Success("/storage/test.txt");
            });

        // Act
        var act = async () => await _handler.HandleAsync(request, cancellationTokenSource.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public void Constructor_WhenValidatorIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act
        var act = () => new UploadCNABFileHandler(
            _fileStorage,
            _fileRepository,
            _logger,
            null!
        );

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("validator");
    }

    [Fact]
    public async Task HandleAsync_WhenMultipleFilesUploaded_ShouldCreateUniqueFileIds()
    {
        // Arrange
        var request1 = UploadCNABFileRequestBuilder.New
            .WithFileName("file1.txt")
            .WithContent("content1")
            .Build();
        var request2 = UploadCNABFileRequestBuilder.New
            .WithFileName("file2.txt")
            .WithContent("content2")
            .Build();
        
        _validator.TryValidate(Arg.Any<UploadCNABFileRequest>()).Returns(ValidationResult.Success());
        _fileStorage.SaveFileAsync(Arg.Any<string>(), Arg.Any<Stream>(), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult(Result<string>.Success("/path/file1.txt")),
                Task.FromResult(Result<string>.Success("/path/file2.txt"))
            );

        // Act
        var result1 = await _handler.HandleAsync(request1, CancellationToken.None);
        var result2 = await _handler.HandleAsync(request2, CancellationToken.None);

        // Assert
        result1.Value!.FileId.Should().NotBe(result2.Value!.FileId);
    }

    [Theory]
    [InlineData("")]  // Empty content
    [InlineData(" ")]  // Whitespace  
    [InlineData("\n\n\n")]  // Only newlines
    public async Task HandleAsync_WithEmptyOrWhitespaceContent_ShouldProcessSuccessfully(string content)
    {
        // Arrange
        var request = UploadCNABFileRequestBuilder.New
            .WithContent(content)
            .Build();
            
        _validator.TryValidate(request).Returns(ValidationResult.Success());
        _fileStorage.SaveFileAsync(Arg.Any<string>(), Arg.Any<Stream>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<string>.Success("/storage/test.txt")));

        // Act
        var result = await _handler.HandleAsync(request, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        await _fileRepository.Received(1).AddAsync(Arg.Any<CNABFile>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithMixedValidAndInvalidContent_ShouldUploadSuccessfully()
    {
        // Arrange
        var request = UploadCNABFileRequestBuilder.New
            .WithMixedValidAndInvalidContent()
            .Build();
            
        _validator.TryValidate(request).Returns(ValidationResult.Success());
        _fileStorage.SaveFileAsync(Arg.Any<string>(), Arg.Any<Stream>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<string>.Success("/storage/test.txt")));

        // Act
        var result = await _handler.HandleAsync(request, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Value!.Status.Should().Be(CNABFileStatus.Uploaded);
        // Upload should succeed regardless of content validity (processing happens later)
    }

    [Fact]
    public async Task HandleAsync_WithInvalidFileExtension_WhenValidatorRejects_ShouldReturnFailure()
    {
        // Arrange
        var request = UploadCNABFileRequestBuilder.New
            .WithInvalidExtension()
            .Build();
            
        _validator.TryValidate(request)
            .Returns(ValidationResult.Failed("Invalid file type. Only .txt files are allowed"));

        // Act
        var result = await _handler.HandleAsync(request, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.FailureDetails.Should().Contain(x => x.Description!.Contains("Invalid file type"));
        await _fileStorage.DidNotReceive().SaveFileAsync(Arg.Any<string>(), Arg.Any<Stream>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithRandomData_ShouldProcessSuccessfully()
    {
        // Arrange
        var request = UploadCNABFileRequestBuilder.New
            .WithRandomData()
            .Build();
            
        _validator.TryValidate(request).Returns(ValidationResult.Success());
        _fileStorage.SaveFileAsync(Arg.Any<string>(), Arg.Any<Stream>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<string>.Success("/storage/random.txt")));

        // Act
        var result = await _handler.HandleAsync(request, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Value!.FileName.Should().StartWith("CNAB_");
        result.Value.FileName.Should().EndWith(".txt");
    }

    [Fact]
    public void Constructor_WhenFileStorageIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act
        var act = () => new UploadCNABFileHandler(
            null!,
            _fileRepository,
            _logger,
            _validator
        );

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("fileStorage");
    }

    [Fact]
    public void Constructor_WhenFileRepositoryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act
        var act = () => new UploadCNABFileHandler(
            _fileStorage,
            null!,
            _logger,
            _validator
        );

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("fileRepository");
    }

    [Fact]
    public void Constructor_WhenLoggerIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act
        var act = () => new UploadCNABFileHandler(
            _fileStorage,
            _fileRepository,
            null!,
            _validator
        );

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("logger");
    }
}
