using ByCoders.CNAB.Domain.Transactions.Models;
using FluentAssertions;
using Xunit;

namespace ByCoders.CNAB.UnitTests.Domain;

public class TransactionTests
{

    [Fact]
    public void Transaction_WhenCreated_ShouldGenerateUniqueId()
    {
        // Arrange & Act
        var saleTransaction = new Sale(
            Guid.NewGuid(),
            new DateOnly(2019, 03, 01),
            new TimeOnly(15, 30, 45),
            14200m,
            new Beneficiary("09620676017"),
            new Card("4753****3153"),
            new Store("BAR DO JOÃO", "JOÃO MACEDO"));

        // Assert
        saleTransaction.Id.Should().NotBeEmpty();
        saleTransaction.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Transaction_WhenCreated_ShouldSetCreatedOnToCurrentUtcTime()
    {
        // Arrange
        var timestampBeforeCreation = DateTimeOffset.UtcNow;

        // Act
        var saleTransaction = new Sale(
            Guid.NewGuid(),
            new DateOnly(2019, 03, 01),
            new TimeOnly(15, 30, 45),
            14200m,
            new Beneficiary("09620676017"),
            new Card("4753****3153"),
            new Store("BAR DO JOÃO", "JOÃO MACEDO"));

        // Assert
        var timestampAfterCreation = DateTimeOffset.UtcNow;
        saleTransaction.CreatedOn.Should().BeOnOrAfter(timestampBeforeCreation);
        saleTransaction.CreatedOn.Should().BeOnOrBefore(timestampAfterCreation);
    }

    [Fact]
    public void Transaction_WhenCreated_ShouldSetAllPropertiesCorrectly()
    {
        // Arrange
        var expectedCnabFileId = Guid.NewGuid();
        var expectedDate = new DateOnly(2019, 03, 01);
        var expectedTime = new TimeOnly(15, 30, 45);
        var expectedAmountCNAB = 14200m;
        var expectedBeneficiary = new Beneficiary("09620676017");
        var expectedCard = new Card("4753****3153");
        var expectedStore = new Store("BAR DO JOÃO", "JOÃO MACEDO");

        // Act
        var saleTransaction = new Sale(
            expectedCnabFileId,
            expectedDate,
            expectedTime,
            expectedAmountCNAB,
            expectedBeneficiary,
            expectedCard,
            expectedStore);

        // Assert
        saleTransaction.CNABFileId.Should().Be(expectedCnabFileId);
        saleTransaction.TransactionDate.Should().Be(expectedDate);
        saleTransaction.TransactionTime.Should().Be(expectedTime);
        saleTransaction.AmountCNAB.Should().Be(expectedAmountCNAB);
        saleTransaction.Beneficiary.Should().Be(expectedBeneficiary);
        saleTransaction.Card.Should().Be(expectedCard);
        saleTransaction.Store.Should().Be(expectedStore);
    }

    [Fact]
    public void Transaction_TransactionValue_ShouldDivideAmountCNABBy100()
    {
        // Arrange
        const decimal amountInCents = 14200m;
        const decimal expectedValueInReais = 142m;

        // Act
        var saleTransaction = new Sale(
            Guid.NewGuid(),
            new DateOnly(2019, 03, 01),
            new TimeOnly(15, 30, 45),
            amountInCents,
            new Beneficiary("09620676017"),
            new Card("4753****3153"),
            new Store("BAR DO JOÃO", "JOÃO MACEDO"));

        // Assert
        saleTransaction.TransactionValue.Should().Be(expectedValueInReais);
    }

    [Fact]
    public void Transaction_WithZeroAmount_ShouldHaveZeroTransactionValue()
    {
        // Arrange & Act
        var saleTransactionWithZeroAmount = new Sale(
            Guid.NewGuid(),
            new DateOnly(2019, 03, 01),
            new TimeOnly(15, 30, 45),
            0m,
            new Beneficiary("09620676017"),
            new Card("4753****3153"),
            new Store("BAR DO JOÃO", "JOÃO MACEDO"));

        // Assert
        saleTransactionWithZeroAmount.AmountCNAB.Should().Be(0);
        saleTransactionWithZeroAmount.TransactionValue.Should().Be(0);
    }

    [Fact]
    public void Transaction_MultipleInstances_ShouldHaveUniqueIds()
    {
        // Arrange & Act
        var firstSaleTransaction = new Sale(
            Guid.NewGuid(),
            new DateOnly(2019, 03, 01),
            new TimeOnly(15, 30, 45),
            14200m,
            new Beneficiary("09620676017"),
            new Card("4753****3153"),
            new Store("BAR DO JOÃO", "JOÃO MACEDO"));
            
        var secondSaleTransaction = new Sale(
            Guid.NewGuid(),
            new DateOnly(2019, 03, 01),
            new TimeOnly(15, 30, 45),
            14200m,
            new Beneficiary("09620676017"),
            new Card("4753****3153"),
            new Store("BAR DO JOÃO", "JOÃO MACEDO"));
            
        var thirdSaleTransaction = new Sale(
            Guid.NewGuid(),
            new DateOnly(2019, 03, 01),
            new TimeOnly(15, 30, 45),
            14200m,
            new Beneficiary("09620676017"),
            new Card("4753****3153"),
            new Store("BAR DO JOÃO", "JOÃO MACEDO"));

        // Assert
        firstSaleTransaction.Id.Should().NotBe(secondSaleTransaction.Id);
        secondSaleTransaction.Id.Should().NotBe(thirdSaleTransaction.Id);
        firstSaleTransaction.Id.Should().NotBe(thirdSaleTransaction.Id);
    }

    [Fact]
    public void Transaction_Beneficiary_ShouldStoreDocumentCorrectly()
    {
        // Arrange
        const string expectedCPF = "09620676017";
        var beneficiaryWithCPF = new Beneficiary(expectedCPF);

        // Act
        var saleTransaction = new Sale(
            Guid.NewGuid(),
            new DateOnly(2019, 03, 01),
            new TimeOnly(15, 30, 45),
            14200m,
            beneficiaryWithCPF,
            new Card("4753****3153"),
            new Store("BAR DO JOÃO", "JOÃO MACEDO"));

        // Assert
        saleTransaction.Beneficiary.Document.Should().Be(expectedCPF);
    }

    [Fact]
    public void Transaction_Card_ShouldStoreMaskedNumberCorrectly()
    {
        // Arrange
        const string expectedMaskedCardNumber = "4753****3153";
        var cardWithMaskedNumber = new Card(expectedMaskedCardNumber);

        // Act
        var saleTransaction = new Sale(
            Guid.NewGuid(),
            new DateOnly(2019, 03, 01),
            new TimeOnly(15, 30, 45),
            14200m,
            new Beneficiary("09620676017"),
            cardWithMaskedNumber,
            new Store("BAR DO JOÃO", "JOÃO MACEDO"));

        // Assert
        saleTransaction.Card.Number.Should().Be(expectedMaskedCardNumber);
    }

    [Fact]
    public void Transaction_Store_ShouldStoreNameAndOwnerCorrectly()
    {
        // Arrange
        const string expectedStoreName = "BAR DO JOÃO";
        const string expectedStoreOwner = "JOÃO MACEDO";
        var storeWithDetails = new Store(expectedStoreName, expectedStoreOwner);

        // Act
        var saleTransaction = new Sale(
            Guid.NewGuid(),
            new DateOnly(2019, 03, 01),
            new TimeOnly(15, 30, 45),
            14200m,
            new Beneficiary("09620676017"),
            new Card("4753****3153"),
            storeWithDetails);

        // Assert
        saleTransaction.Store.Name.Should().Be(expectedStoreName);
        saleTransaction.Store.Owner.Should().Be(expectedStoreOwner);
    }

    [Theory]
    [InlineData(100, 1)]
    [InlineData(1000, 10)]
    [InlineData(10000, 100)]
    [InlineData(14200, 142)]
    [InlineData(99999, 999.99)]
    [InlineData(1, 0.01)]
    public void Transaction_TransactionValue_ShouldCalculateCorrectlyForVariousAmounts(decimal amountInCents, decimal expectedValueInReais)
    {
        // Arrange & Act
        var saleTransaction = new Sale(
            Guid.NewGuid(),
            new DateOnly(2019, 03, 01),
            new TimeOnly(15, 30, 45),
            amountInCents,
            new Beneficiary("09620676017"),
            new Card("4753****3153"),
            new Store("BAR DO JOÃO", "JOÃO MACEDO"));

        // Assert
        saleTransaction.TransactionValue.Should().Be(expectedValueInReais);
    }

    [Fact]
    public void Transaction_WithCNABFileId_ShouldStoreCNABFileIdCorrectly()
    {
        // Arrange
        var expectedCNABFileId = Guid.NewGuid();

        // Act
        var saleTransaction = new Sale(
            expectedCNABFileId,
            new DateOnly(2019, 03, 01),
            new TimeOnly(15, 30, 45),
            14200m,
            new Beneficiary("09620676017"),
            new Card("4753****3153"),
            new Store("BAR DO JOÃO", "JOÃO MACEDO"));

        // Assert
        saleTransaction.CNABFileId.Should().Be(expectedCNABFileId);
    }

    [Fact]
    public void Transaction_WithCNABFileId_ShouldAlwaysHaveNonNullCNABFileId()
    {
        // Note: After refactoring, CNABFileId is required in constructor
        // This test documents that CNABFileId cannot be null
        
        // Arrange & Act
        var cnabFileId = Guid.NewGuid();
        var saleTransaction = new Sale(
            cnabFileId,
            new DateOnly(2019, 03, 01),
            new TimeOnly(15, 30, 45),
            14200m,
            new Beneficiary("09620676017"),
            new Card("4753****3153"),
            new Store("BAR DO JOÃO", "JOÃO MACEDO"));

        // Assert
        saleTransaction.CNABFileId.Should().Be(cnabFileId);
        saleTransaction.CNABFileId.Should().NotBeNull();
    }

    [Fact]
    public void Transaction_WithLargeAmount_ShouldCalculateTransactionValueCorrectly()
    {
        // Arrange
        const decimal largeAmountInCents = 9999999999m;
        const decimal expectedValueInReais = 99999999.99m;

        // Act
        var saleTransaction = new Sale(
            Guid.NewGuid(),
            new DateOnly(2019, 03, 01),
            new TimeOnly(15, 30, 45),
            largeAmountInCents,
            new Beneficiary("09620676017"),
            new Card("4753****3153"),
            new Store("BAR DO JOÃO", "JOÃO MACEDO"));

        // Assert
        saleTransaction.AmountCNAB.Should().Be(largeAmountInCents);
        saleTransaction.TransactionValue.Should().Be(expectedValueInReais);
    }

    [Fact]
    public void Transaction_DifferentTypes_ShouldCreateCorrectInstances()
    {
        // Arrange - Common parameters for all transactions
        var cnabFileId = Guid.NewGuid();
        var date = new DateOnly(2019, 03, 01);
        var time = new TimeOnly(15, 30, 45);
        var amount = 14200m;
        var beneficiary = new Beneficiary("09620676017");
        var card = new Card("4753****3153");
        var store = new Store("BAR DO JOÃO", "JOÃO MACEDO");
        
        // Act
        var debitTransaction = new Debit(cnabFileId, date, time, amount, beneficiary, card, store);
        var creditTransaction = new Credit(cnabFileId, date, time, amount, beneficiary, card, store);
        var bankSlipTransaction = new BankSlip(cnabFileId, date, time, amount, beneficiary, card, store);
        var fundingTransaction = new Funding(cnabFileId, date, time, amount, beneficiary, card, store);
        var loanReceiptTransaction = new LoanReceipt(cnabFileId, date, time, amount, beneficiary, card, store);
        var tedReceiptTransaction = new TEDReceipt(cnabFileId, date, time, amount, beneficiary, card, store);
        var docReceiptTransaction = new DOCReceipt(cnabFileId, date, time, amount, beneficiary, card, store);
        var rentTransaction = new Rent(cnabFileId, date, time, amount, beneficiary, card, store);

        // Assert
        debitTransaction.Should().BeOfType<Debit>();
        creditTransaction.Should().BeOfType<Credit>();
        bankSlipTransaction.Should().BeOfType<BankSlip>();
        fundingTransaction.Should().BeOfType<Funding>();
        loanReceiptTransaction.Should().BeOfType<LoanReceipt>();
        tedReceiptTransaction.Should().BeOfType<TEDReceipt>();
        docReceiptTransaction.Should().BeOfType<DOCReceipt>();
        rentTransaction.Should().BeOfType<Rent>();
    }

    [Fact]
    public void Transaction_Equals_ShouldCompareById()
    {
        // Arrange
        var transaction1 = new Sale(
            Guid.NewGuid(),
            new DateOnly(2019, 03, 01),
            new TimeOnly(15, 30, 45),
            14200m,
            new Beneficiary("09620676017"),
            new Card("4753****3153"),
            new Store("BAR DO JOÃO", "JOÃO MACEDO"));
            
        var transaction2 = new Sale(
            Guid.NewGuid(),
            new DateOnly(2019, 03, 01),
            new TimeOnly(15, 30, 45),
            14200m,
            new Beneficiary("09620676017"),
            new Card("4753****3153"),
            new Store("BAR DO JOÃO", "JOÃO MACEDO"));
        
        // Act & Assert
        transaction1.Should().NotBe(transaction2);
        transaction1.Id.Should().NotBe(transaction2.Id);
    }

    [Fact]
    public void Transaction_WithRandomData_ShouldCreateValidTransaction()
    {
        // Arrange & Act
        var random = Random.Shared;
        var randomSaleTransaction = new Sale(
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Now.AddDays(-random.Next(365))),
            TimeOnly.FromTimeSpan(TimeSpan.FromSeconds(random.Next(86400))),
            random.Next(1, 1000000),
            new Beneficiary($"{random.Next(100000000):00000000}{random.Next(1000):000}"),
            new Card($"{random.Next(1000, 9999):0000}****{random.Next(1000, 9999):0000}"),
            new Store($"Store_{random.Next(1000)}", $"Owner_{random.Next(1000)}"));

        // Assert
        randomSaleTransaction.Should().NotBeNull();
        randomSaleTransaction.Id.Should().NotBeEmpty();
        randomSaleTransaction.TransactionDate.Should().NotBe(default(DateOnly));
        randomSaleTransaction.TransactionTime.Should().NotBe(default(TimeOnly));
        randomSaleTransaction.AmountCNAB.Should().BeGreaterThanOrEqualTo(0);
        randomSaleTransaction.Beneficiary.Should().NotBeNull();
        randomSaleTransaction.Card.Should().NotBeNull();
        randomSaleTransaction.Store.Should().NotBeNull();
    }
}
