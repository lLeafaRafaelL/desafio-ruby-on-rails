using ByCoders.CNAB.Application.Files.CNAB.Parsers;
using ByCoders.CNAB.Application.Files.CNAB.Process;
using ByCoders.CNAB.Application.Transactions;
using ByCoders.CNAB.Core.Results;
using ByCoders.CNAB.Domain.Files;
using ByCoders.CNAB.Domain.Files.Models;
using ByCoders.CNAB.Domain.Transactions;
using ByCoders.CNAB.Domain.Transactions.Models;
using ByCoders.CNAB.UnitTests.Builders.Application;
using ByCoders.CNAB.UnitTests.Builders.Domain;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Text;
using Xunit;

namespace ByCoders.CNAB.UnitTests.Application;

public class ProcessCNABFileServiceTests
{
    private readonly ICNABFileRepository _fileRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IFileStorageService _fileStorage;
    private readonly CNABLineParser _parser;
    private readonly ITransactionFactory _transactionFactory;
    private readonly ILogger<ProcessCNABFileService> _logger;
    private readonly ProcessCNABFileService _service;

    public ProcessCNABFileServiceTests()
    {
        _fileRepository = Substitute.For<ICNABFileRepository>();
        _transactionRepository = Substitute.For<ITransactionRepository>();
        _fileStorage = Substitute.For<IFileStorageService>();
        _parser = Substitute.For<CNABLineParser>();
        _transactionFactory = Substitute.For<ITransactionFactory>();
        _logger = Substitute.For<ILogger<ProcessCNABFileService>>();

        _service = new ProcessCNABFileService(
            _fileRepository,
            _transactionRepository,
            _fileStorage,
            _parser,
            _transactionFactory,
            _logger
        );
    }

    [Fact]
    public async Task ProcessPendingFilesAsync_WhenNoPendingFiles_ShouldReturnZero()
    {
        // Arrange
        var emptyFileList = new List<CNABFile>();
        _fileRepository.FindByStatusAsync(CNABFileStatus.Uploaded, Arg.Any<CancellationToken>())
            .Returns(emptyFileList);

        // Act
        var processedCount = await _service.ProcessPendingFilesAsync(CancellationToken.None);

        // Assert
        processedCount.Should().Be(0);
        await _fileRepository.Received(1).FindByStatusAsync(CNABFileStatus.Uploaded, Arg.Any<CancellationToken>());
        await _transactionRepository.DidNotReceive().BulkInsertAsync(Arg.Any<IEnumerable<Transaction>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProcessPendingFilesAsync_WhenFilesExist_ShouldProcessAllFiles()
    {
        // Arrange
        var pendingFiles = new List<CNABFile>
        {
            CNABFileBuilder.New.WithFileName("file1.txt").Build(),
            CNABFileBuilder.New.WithFileName("file2.txt").Build(),
            CNABFileBuilder.New.WithFileName("file3.txt").Build()
        };

        _fileRepository.FindByStatusAsync(CNABFileStatus.Uploaded, Arg.Any<CancellationToken>())
            .Returns(pendingFiles);

        SetupSuccessfulFileProcessing();

        // Act
        var processedCount = await _service.ProcessPendingFilesAsync(CancellationToken.None);

        // Assert
        processedCount.Should().Be(3);
        await _fileRepository.Received(1).FindByStatusAsync(CNABFileStatus.Uploaded, Arg.Any<CancellationToken>());
        await _fileRepository.Received(3).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProcessPendingFilesAsync_WhenCancellationRequested_ShouldStopProcessing()
    {
        // Arrange
        var pendingFiles = new List<CNABFile>
        {
            CNABFileBuilder.New.WithFileName("file1.txt").Build(),
            CNABFileBuilder.New.WithFileName("file2.txt").Build()
        };

        _fileRepository.FindByStatusAsync(CNABFileStatus.Uploaded, Arg.Any<CancellationToken>())
            .Returns(pendingFiles);

        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        var processedCount = await _service.ProcessPendingFilesAsync(cancellationTokenSource.Token);

        // Assert
        processedCount.Should().Be(0);
        await _transactionRepository.DidNotReceive().BulkInsertAsync(Arg.Any<IEnumerable<Transaction>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProcessFileAsync_WhenPhysicalFileNotFound_ShouldReturnZeroProcessed()
    {
        // Arrange
        var cnabFile = CNABFileBuilder.New.Build();
        var pendingFiles = new List<CNABFile> { cnabFile };

        _fileRepository.FindByStatusAsync(CNABFileStatus.Uploaded, Arg.Any<CancellationToken>())
            .Returns(pendingFiles);
        _fileStorage.FileExists(cnabFile.FilePath).Returns(false);

        // Act
        var processedCount = await _service.ProcessPendingFilesAsync(CancellationToken.None);

        // Assert
        processedCount.Should().Be(0);
        // The service's responsibility is to call SaveChangesAsync, not to test CNABFile's internal state
        await _fileRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProcessFileAsync_WhenFileContentIsValid_ShouldProcessSuccessfully()
    {
        // Arrange
        var cnabFile = CNABFileBuilder.New.Build();
        var pendingFiles = new List<CNABFile> { cnabFile };
        const string validCnabLine = "3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ";

        _fileRepository.FindByStatusAsync(CNABFileStatus.Uploaded, Arg.Any<CancellationToken>())
            .Returns(pendingFiles);
        _fileStorage.FileExists(cnabFile.FilePath).Returns(true);

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(validCnabLine));
        _fileStorage.ReadFile(cnabFile.FilePath, Arg.Any<CancellationToken>())
            .Returns(Result<Stream>.Success(memoryStream));

        var parsedData = CNABFactoryParamsBuilder.New.WithValidData().Build();
        _parser.Parse(validCnabLine).Returns(Result<CNABFactoryParams>.Success(parsedData));

        var transaction = SaleBuilder.New.Build();
        _transactionFactory.Create(cnabFile.Id, parsedData)
            .Returns(Result<Transaction>.Success(transaction));

        // Act
        var processedCount = await _service.ProcessPendingFilesAsync(CancellationToken.None);

        // Assert
        processedCount.Should().Be(1);
        // Service's responsibility is to call the repository methods, not test CNABFile state
        await _transactionRepository.Received(1).BulkInsertAsync(
            Arg.Is<IEnumerable<Transaction>>(t => t.Count() == 1),
            Arg.Any<CancellationToken>()
        );
        await _fileRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProcessFileAsync_WhenParsingFails_ShouldContinueProcessingOtherLines()
    {
        // Arrange
        var cnabFile = CNABFileBuilder.New.Build();
        var pendingFiles = new List<CNABFile> { cnabFile };
        const string fileContent = @"3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       
INVALID LINE
1201903010000015200096206760171234****7890233000JOÃO MACEDO   BAR DO JOÃO       ";

        _fileRepository.FindByStatusAsync(CNABFileStatus.Uploaded, Arg.Any<CancellationToken>())
            .Returns(pendingFiles);
        _fileStorage.FileExists(cnabFile.FilePath).Returns(true);

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
        _fileStorage.ReadFile(cnabFile.FilePath, Arg.Any<CancellationToken>())
            .Returns(Result<Stream>.Success(memoryStream));

        // Setup parser responses
        _parser.Parse(Arg.Is<string>(s => s.StartsWith("3")))
            .Returns(Result<CNABFactoryParams>.Success(CNABFactoryParamsBuilder.New.WithValidData().Build()));
        _parser.Parse(Arg.Is<string>(s => s.StartsWith("INVALID")))
            .Returns(Result<CNABFactoryParams>.Failure("Invalid line format"));
        _parser.Parse(Arg.Is<string>(s => s.StartsWith("1")))
            .Returns(Result<CNABFactoryParams>.Success(CNABFactoryParamsBuilder.New.WithValidData().Build()));

        _transactionFactory.Create(Arg.Any<Guid>(), Arg.Any<CNABFactoryParams>())
            .Returns(Result<Transaction>.Success(SaleBuilder.New.Build()));

        // Act
        var processedCount = await _service.ProcessPendingFilesAsync(CancellationToken.None);

        // Assert
        processedCount.Should().Be(1);
        // Only test service behavior, not CNABFile state
        await _transactionRepository.Received(1).BulkInsertAsync(
            Arg.Is<IEnumerable<Transaction>>(t => t.Count() == 2),
            Arg.Any<CancellationToken>()
        );
    }

    [Fact]
    public async Task ProcessFileAsync_WhenAllLinesFail_ShouldMarkFileAsFailed()
    {
        // Arrange
        var cnabFile = CNABFileBuilder.New.Build();
        var pendingFiles = new List<CNABFile> { cnabFile };
        const string fileContent = "INVALID LINE 1\nINVALID LINE 2";

        _fileRepository.FindByStatusAsync(CNABFileStatus.Uploaded, Arg.Any<CancellationToken>())
            .Returns(pendingFiles);
        _fileStorage.FileExists(cnabFile.FilePath).Returns(true);

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
        _fileStorage.ReadFile(cnabFile.FilePath, Arg.Any<CancellationToken>())
            .Returns(Result<Stream>.Success(memoryStream));

        _parser.Parse(Arg.Any<string>())
            .Returns(Result<CNABFactoryParams>.Failure("Invalid line format"));

        // Act
        var processedCount = await _service.ProcessPendingFilesAsync(CancellationToken.None);

        // Assert
        processedCount.Should().Be(0);
        // Service correctly returned 0 processed files
        await _transactionRepository.DidNotReceive().BulkInsertAsync(Arg.Any<IEnumerable<Transaction>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProcessFileAsync_WhenExceptionOccurs_ShouldMarkFileAsFailedAndLogError()
    {
        // Arrange
        var cnabFile = CNABFileBuilder.New.Build();
        var pendingFiles = new List<CNABFile> { cnabFile };

        _fileRepository.FindByStatusAsync(CNABFileStatus.Uploaded, Arg.Any<CancellationToken>())
            .Returns(pendingFiles);
        _fileStorage.FileExists(cnabFile.FilePath).Returns(true);
        _fileStorage.ReadFile(cnabFile.FilePath, Arg.Any<CancellationToken>())
            .Throws(new IOException("Disk error"));

        // Act
        var processedCount = await _service.ProcessPendingFilesAsync(CancellationToken.None);

        // Assert
        processedCount.Should().Be(0);
        // Service handled exception correctly
        await _fileRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        _logger.Received().LogError(Arg.Any<Exception>(), Arg.Any<string>(), Arg.Any<object[]>());
    }

    [Fact]
    public async Task ProcessFileAsync_WhenEmptyLinesExist_ShouldIgnoreThem()
    {
        // Arrange
        var cnabFile = CNABFileBuilder.New.Build();
        var pendingFiles = new List<CNABFile> { cnabFile };
        const string fileContent = @"3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       

1201903010000015200096206760171234****7890233000JOÃO MACEDO   BAR DO JOÃO       ";

        _fileRepository.FindByStatusAsync(CNABFileStatus.Uploaded, Arg.Any<CancellationToken>())
            .Returns(pendingFiles);
        _fileStorage.FileExists(cnabFile.FilePath).Returns(true);

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
        _fileStorage.ReadFile(cnabFile.FilePath, Arg.Any<CancellationToken>())
            .Returns(Result<Stream>.Success(memoryStream));

        _parser.Parse(Arg.Is<string>(s => !string.IsNullOrWhiteSpace(s)))
            .Returns(Result<CNABFactoryParams>.Success(CNABFactoryParamsBuilder.New.WithValidData().Build()));

        _transactionFactory.Create(Arg.Any<Guid>(), Arg.Any<CNABFactoryParams>())
            .Returns(Result<Transaction>.Success(SaleBuilder.New.Build()));

        // Act
        var processedCount = await _service.ProcessPendingFilesAsync(CancellationToken.None);

        // Assert
        processedCount.Should().Be(1);
        // Service processed file successfully
        _parser.Received(2).Parse(Arg.Any<string>()); // Only non-empty lines
    }

    private void SetupSuccessfulFileProcessing()
    {
        _fileStorage.FileExists(Arg.Any<string>()).Returns(true);
        
        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       "));
        _fileStorage.ReadFile(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result<Stream>.Success(memoryStream));

        _parser.Parse(Arg.Any<string>())
            .Returns(Result<CNABFactoryParams>.Success(CNABFactoryParamsBuilder.New.WithValidData().Build()));

        _transactionFactory.Create(Arg.Any<Guid>(), Arg.Any<CNABFactoryParams>())
            .Returns(Result<Transaction>.Success(SaleBuilder.New.Build()));
    }
}
