using ByCoders.CNAB.AppService.Transactions.CNAB.Import;
using ByCoders.CNAB.Domain.Transactions;
using ByCoders.CNAB.Domain.Transactions.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using System.Text;
using Xunit;

namespace ByCoders.CNAB.UnitTests.AppService;

public class ImportCNABRequestHandlerTests
{
    private readonly ImportCNABRequestHandler _sut;
    private readonly ITransactionRepository _repository;
    private readonly ITransactionFactory _transactionFactory;
    private readonly CNABLineParser _parser;

    public ImportCNABRequestHandlerTests()
    {
        _repository = Substitute.For<ITransactionRepository>();
        _transactionFactory = new TransactionFactory();
        _parser = new CNABLineParser();
        _sut = new ImportCNABRequestHandler(_repository, _transactionFactory, _parser);
    }

    [Fact]
    public async Task Handle_ValidCNABFile_ShouldReturnSuccessResponse()
    {
        // Arrange
        var cnabContent = @"3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO        
5201903010000013200556418150633123****7687145607MARIA JOSEFINALOJA DO Ó - MATRIZ 
1201903010000015200096206760171234****7890233000JOÃO MACEDO   BAR DO JOÃO        ";

        var formFile = CreateMockFormFile(cnabContent);
        var request = new ImportCNABRequest { CNABFile = formFile };

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Errors.Should().BeEmpty($"Found errors: {string.Join(", ", result.Errors)}");
        result.Success.Should().BeTrue();
        result.TransactionsImported.Should().Be(3);
        
        // Verify repository interaction
        await _repository.Received(1).BulkInsertAsync(
            Arg.Is<IEnumerable<Transaction>>(t => t.Count() == 3),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_EmptyFile_ShouldReturnZeroTransactions()
    {
        // Arrange
        var formFile = CreateMockFormFile("");
        var request = new ImportCNABRequest { CNABFile = formFile };

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.TransactionsImported.Should().Be(0);
        
        // Verify repository was NOT called
        await _repository.DidNotReceive().BulkInsertAsync(
            Arg.Any<IEnumerable<Transaction>>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_FileWithOnlyWhitespace_ShouldReturnZeroTransactions()
    {
        // Arrange
        var cnabContent = @"
        
        
        ";
        var formFile = CreateMockFormFile(cnabContent);
        var request = new ImportCNABRequest { CNABFile = formFile };

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.TransactionsImported.Should().Be(0);
        
        // Verify repository was NOT called
        await _repository.DidNotReceive().BulkInsertAsync(
            Arg.Any<IEnumerable<Transaction>>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_FileWithMultipleLines_ShouldImportAll()
    {
        // Arrange
        var cnabContent = @"3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO        
5201903010000013200556418150633123****7687145607MARIA JOSEFINALOJA DO Ó - MATRIZ 
3201903010000012200845152540736777****1313172712MARCOS PEREIRAMERCADO DA AVENIDA 
2201903010000011200096206760173648****0099234234JOÃO MACEDO   BAR DO JOÃO        
1201903010000015200096206760171234****7890233000JOÃO MACEDO   BAR DO JOÃO        ";

        var formFile = CreateMockFormFile(cnabContent);
        var request = new ImportCNABRequest { CNABFile = formFile };

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.TransactionsImported.Should().Be(5);
        
        // Verify repository interaction
        await _repository.Received(1).BulkInsertAsync(
            Arg.Is<IEnumerable<Transaction>>(t => t.Count() == 5),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_AllTransactionTypes_ShouldImportCorrectly()
    {
        // Arrange - One line for each transaction type (1-9)
        var cnabContent = @"1201903010000015200096206760171234****7890233000JOÃO MACEDO   BAR DO JOÃO        
2201903010000011200096206760173648****0099234234JOÃO MACEDO   BAR DO JOÃO        
3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO        
4201903010000015232556418150631234****6678100000MARIA JOSEFINALOJA DO Ó - FILIAL 
5201903010000013200556418150633123****7687145607MARIA JOSEFINALOJA DO Ó - MATRIZ 
6201906010000050617845152540731234****2231100000MARCOS PEREIRAMERCADO DA AVENIDA 
7201903010000020000556418150631234****3324090002MARIA JOSEFINALOJA DO Ó - MATRIZ 
8201903010000010203845152540732344****1222123222MARCOS PEREIRAMERCADO DA AVENIDA 
9201903010000010200556418150636228****9090090000MARIA JOSEFINALOJA DO Ó - MATRIZ ";

        var formFile = CreateMockFormFile(cnabContent);
        var request = new ImportCNABRequest { CNABFile = formFile };

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        if (result.Errors.Any())
        {
            var errorDetails = string.Join("; ", result.Errors);
            throw new Exception($"Unexpected errors: {errorDetails}");
        }
        result.TransactionsImported.Should().Be(9);
        result.Success.Should().BeTrue();
        
        // Verify repository interaction with all 9 transaction types
        await _repository.Received(1).BulkInsertAsync(
            Arg.Is<IEnumerable<Transaction>>(t => t.Count() == 9),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_FileWithBlankLinesBetweenData_ShouldSkipBlankLines()
    {
        // Arrange
        var cnabContent = @"3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO        

5201903010000013200556418150633123****7687145607MARIA JOSEFINALOJA DO Ó - MATRIZ 

1201903010000015200096206760171234****7890233000JOÃO MACEDO   BAR DO JOÃO        ";

        var formFile = CreateMockFormFile(cnabContent);
        var request = new ImportCNABRequest { CNABFile = formFile };

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.TransactionsImported.Should().Be(3);
        
        // Verify repository interaction (blank lines should be ignored)
        await _repository.Received(1).BulkInsertAsync(
            Arg.Is<IEnumerable<Transaction>>(t => t.Count() == 3),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_LargeFile_ShouldProcessEfficiently()
    {
        // Arrange - Create 100 lines
        var lines = new List<string>();
        for (int i = 0; i < 100; i++)
        {
            lines.Add("3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO        ");
        }
        var cnabContent = string.Join(Environment.NewLine, lines);

        var formFile = CreateMockFormFile(cnabContent);
        var request = new ImportCNABRequest { CNABFile = formFile };

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.TransactionsImported.Should().Be(100);
        result.Success.Should().BeTrue();
        
        // Verify repository interaction - BulkInsert should be called once with all 100 transactions
        await _repository.Received(1).BulkInsertAsync(
            Arg.Is<IEnumerable<Transaction>>(t => t.Count() == 100),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithCancellationToken_ShouldRespectCancellation()
    {
        // Arrange
        var cnabContent = @"3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO        ";
        var formFile = CreateMockFormFile(cnabContent);
        var request = new ImportCNABRequest { CNABFile = formFile };
        var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancel immediately

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            await _sut.Handle(request, cts.Token));
    }

    [Fact]
    public async Task Handle_FileWithDifferentStores_ShouldImportAll()
    {
        // Arrange
        var cnabContent = @"3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO        
5201903010000013200556418150633123****7687145607MARIA JOSEFINALOJA DO Ó - MATRIZ 
3201903010000012200845152540736777****1313172712MARCOS PEREIRAMERCADO DA AVENIDA 
3201903010000060200232702980566777****1313172712JOSÉ COSTA    MERCEARIA 3 IRMÃOS ";

        var formFile = CreateMockFormFile(cnabContent);
        var request = new ImportCNABRequest { CNABFile = formFile };

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.TransactionsImported.Should().Be(4);
        
        // Verify repository interaction
        await _repository.Received(1).BulkInsertAsync(
            Arg.Is<IEnumerable<Transaction>>(t => t.Count() == 4),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_FileWithSameStoreMultipleTransactions_ShouldImportAll()
    {
        // Arrange
        var cnabContent = @"3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO        
2201903010000011200096206760173648****0099234234JOÃO MACEDO   BAR DO JOÃO        
1201903010000015200096206760171234****7890233000JOÃO MACEDO   BAR DO JOÃO        ";

        var formFile = CreateMockFormFile(cnabContent);
        var request = new ImportCNABRequest { CNABFile = formFile };

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.TransactionsImported.Should().Be(3);
        
        // Verify repository interaction
        await _repository.Received(1).BulkInsertAsync(
            Arg.Is<IEnumerable<Transaction>>(t => t.Count() == 3),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidFile_ShouldProcessInParallel()
    {
        // Arrange - Create enough lines to test parallel processing
        var lines = new List<string>();
        for (int i = 0; i < 20; i++)
        {
            var type = (i % 9) + 1; // Cycle through types 1-9
            lines.Add($"{type}201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO        ");
        }
        var cnabContent = string.Join(Environment.NewLine, lines);

        var formFile = CreateMockFormFile(cnabContent);
        var request = new ImportCNABRequest { CNABFile = formFile };

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.TransactionsImported.Should().Be(20);
        
        // Verify repository interaction - BulkInsert should be called once with all transactions
        await _repository.Received(1).BulkInsertAsync(
            Arg.Is<IEnumerable<Transaction>>(t => t.Count() == 20),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_RealCNABSampleFile_ShouldImportAll()
    {
        // Arrange - Using actual sample from CNAB.txt
        var cnabContent = @"3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO        
5201903010000013200556418150633123****7687145607MARIA JOSEFINALOJA DO Ó - MATRIZ 
3201903010000012200845152540736777****1313172712MARCOS PEREIRAMERCADO DA AVENIDA 
2201903010000011200096206760173648****0099234234JOÃO MACEDO   BAR DO JOÃO        
1201903010000015200096206760171234****7890233000JOÃO MACEDO   BAR DO JOÃO        
2201903010000010700845152540738723****9987123333MARCOS PEREIRAMERCADO DA AVENIDA 
2201903010000050200845152540738473****1231231233MARCOS PEREIRAMERCADO DA AVENIDA 
3201903010000060200232702980566777****1313172712JOSÉ COSTA    MERCEARIA 3 IRMÃOS 
1201903010000020000556418150631234****3324090002MARIA JOSEFINALOJA DO Ó - MATRIZ 
5201903010000080200845152540733123****7687145607MARCOS PEREIRAMERCADO DA AVENIDA 
2201903010000010200232702980568473****1231231233JOSÉ COSTA    MERCEARIA 3 IRMÃOS 
3201903010000610200232702980566777****1313172712JOSÉ COSTA    MERCEARIA 3 IRMÃOS 
4201903010000015232556418150631234****6678100000MARIA JOSEFINALOJA DO Ó - FILIAL 
8201903010000010203845152540732344****1222123222MARCOS PEREIRAMERCADO DA AVENIDA 
3201903010000010300232702980566777****1313172712JOSÉ COSTA    MERCEARIA 3 IRMÃOS 
9201903010000010200556418150636228****9090090000MARIA JOSEFINALOJA DO Ó - MATRIZ 
4201906010000050617845152540731234****2231100000MARCOS PEREIRAMERCADO DA AVENIDA 
2201903010000010900232702980568723****9987123333JOSÉ COSTA    MERCEARIA 3 IRMÃOS 
8201903010000000200845152540732344****1222123222MARCOS PEREIRAMERCADO DA AVENIDA 
2201903010000000500232702980567677****8778141808JOSÉ COSTA    MERCEARIA 3 IRMÃOS 
3201903010000019200845152540736777****1313172712MARCOS PEREIRAMERCADO DA AVENIDA ";

        var formFile = CreateMockFormFile(cnabContent);
        var request = new ImportCNABRequest { CNABFile = formFile };

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        if (result.Errors.Any())
        {
            var errorDetails = string.Join("; ", result.Errors);
            throw new Exception($"Unexpected errors: {errorDetails}");
        }
        result.TransactionsImported.Should().Be(21);
        result.Success.Should().BeTrue();
        
        // Verify repository interaction with real CNAB file data
        await _repository.Received(1).BulkInsertAsync(
            Arg.Is<IEnumerable<Transaction>>(t => t.Count() == 21),
            Arg.Any<CancellationToken>());
    }

    // Helper method to create mock IFormFile
    private IFormFile CreateMockFormFile(string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);

        var formFile = Substitute.For<IFormFile>();
        formFile.OpenReadStream().Returns(stream);
        formFile.Length.Returns(bytes.Length);
        formFile.FileName.Returns("CNAB.txt");

        return formFile;
    }
}
