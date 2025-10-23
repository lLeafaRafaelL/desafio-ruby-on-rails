using ByCoders.CNAB.AppService.Transactions.CNAB.Import;
using ByCoders.CNAB.Domain.Transactions.Models;
using FluentAssertions;
using FizzWare.NBuilder;
using Xunit;

namespace ByCoders.CNAB.UnitTests.AppService;

public class TransactionFactoryTests
{
    private readonly TransactionFactory _sut;

    public TransactionFactoryTests()
    {
        _sut = new TransactionFactory();
    }

    [Fact]
    public void Create_DebitTransaction_ShouldReturnDebitInstance()
    {
        // Arrange
        var data = CreateValidCNABLineDataDto(TransactionTypes.Debit);

        // Act
        var result = _sut.Create(data);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Debit>();
        result.TransactionType.Id.Should().Be((int)TransactionTypes.Debit);
    }

    [Fact]
    public void Create_BankSlipTransaction_ShouldReturnBankSlipInstance()
    {
        // Arrange
        var data = CreateValidCNABLineDataDto(TransactionTypes.BankSlip);

        // Act
        var result = _sut.Create(data);

        // Assert
        result.Should().BeOfType<BankSlip>();
        result.TransactionType.Id.Should().Be((int)TransactionTypes.BankSlip);
    }

    [Fact]
    public void Create_FundingTransaction_ShouldReturnFundingInstance()
    {
        // Arrange
        var data = CreateValidCNABLineDataDto(TransactionTypes.Funding);

        // Act
        var result = _sut.Create(data);

        // Assert
        result.Should().BeOfType<Funding>();
        result.TransactionType.Id.Should().Be((int)TransactionTypes.Funding);
    }

    [Fact]
    public void Create_CreditTransaction_ShouldReturnCreditInstance()
    {
        // Arrange
        var data = CreateValidCNABLineDataDto(TransactionTypes.Credit);

        // Act
        var result = _sut.Create(data);

        // Assert
        result.Should().BeOfType<Credit>();
        result.TransactionType.Id.Should().Be((int)TransactionTypes.Credit);
    }

    [Fact]
    public void Create_LoanReceiptTransaction_ShouldReturnLoanReceiptInstance()
    {
        // Arrange
        var data = CreateValidCNABLineDataDto(TransactionTypes.LoanReceipt);

        // Act
        var result = _sut.Create(data);

        // Assert
        result.Should().BeOfType<LoanReceipt>();
        result.TransactionType.Id.Should().Be((int)TransactionTypes.LoanReceipt);
    }

    [Fact]
    public void Create_SalesTransaction_ShouldReturnSaleInstance()
    {
        // Arrange
        var data = CreateValidCNABLineDataDto(TransactionTypes.Sales);

        // Act
        var result = _sut.Create(data);

        // Assert
        result.Should().BeOfType<Sale>();
        result.TransactionType.Id.Should().Be((int)TransactionTypes.Sales);
    }

    [Fact]
    public void Create_TEDReceiptTransaction_ShouldReturnTEDReceiptInstance()
    {
        // Arrange
        var data = CreateValidCNABLineDataDto(TransactionTypes.TEDReceipt);

        // Act
        var result = _sut.Create(data);

        // Assert
        result.Should().BeOfType<TEDReceipt>();
        result.TransactionType.Id.Should().Be((int)TransactionTypes.TEDReceipt);
    }

    [Fact]
    public void Create_DOCReceiptTransaction_ShouldReturnDOCReceiptInstance()
    {
        // Arrange
        var data = CreateValidCNABLineDataDto(TransactionTypes.DOCReceipt);

        // Act
        var result = _sut.Create(data);

        // Assert
        result.Should().BeOfType<DOCReceipt>();
        result.TransactionType.Id.Should().Be((int)TransactionTypes.DOCReceipt);
    }

    [Fact]
    public void Create_RentTransaction_ShouldReturnRentInstance()
    {
        // Arrange
        var data = CreateValidCNABLineDataDto(TransactionTypes.Rent);

        // Act
        var result = _sut.Create(data);

        // Assert
        result.Should().BeOfType<Rent>();
        result.TransactionType.Id.Should().Be((int)TransactionTypes.Rent);
    }

    [Fact]
    public void Create_ValidData_ShouldMapAllProperties()
    {
        // Arrange
        var expectedDate = new DateOnly(2019, 03, 01);
        var expectedTime = new TimeOnly(15, 34, 53);
        var expectedAmount = 14200m;
        var expectedCpf = "09620676017";
        var expectedCard = "4753****3153";
        var expectedOwner = "JOÃO MACEDO";
        var expectedStore = "BAR DO JOÃO";

        var data = new CNABLineDataDto(
            TransactionTypes.Sales,
            expectedDate,
            expectedAmount,
            expectedCpf,
            expectedCard,
            expectedTime,
            expectedOwner,
            expectedStore
        );

        // Act
        var result = _sut.Create(data);

        // Assert
        result.TransactionDate.Should().Be(expectedDate);
        result.TransactionTime.Should().Be(expectedTime);
        result.AmountCNAB.Should().Be(expectedAmount);
        result.Beneficiary.Document.Should().Be(expectedCpf);
        result.Card.Number.Should().Be(expectedCard);
        result.Store.Owner.Should().Be(expectedOwner);
        result.Store.Name.Should().Be(expectedStore);
    }

    [Fact]
    public void Create_NullData_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => _sut.Create(null);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_InvalidCPF_ShouldThrowArgumentException(string cpf)
    {
        // Arrange
        var data = new CNABLineDataDto(
            TransactionTypes.Sales,
            new DateOnly(2019, 03, 01),
            1000,
            cpf,
            "1234567890",
            new TimeOnly(10, 30, 00),
            "Owner",
            "Store"
        );

        // Act
        var act = () => _sut.Create(data);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("CPF cannot be empty");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_InvalidCardNumber_ShouldThrowArgumentException(string cardNumber)
    {
        // Arrange
        var data = new CNABLineDataDto(
            TransactionTypes.Sales,
            new DateOnly(2019, 03, 01),
            1000,
            "12345678901",
            cardNumber,
            new TimeOnly(10, 30, 00),
            "Owner",
            "Store"
        );

        // Act
        var act = () => _sut.Create(data);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Card number cannot be empty");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_InvalidStoreName_ShouldThrowArgumentException(string storeName)
    {
        // Arrange
        var data = new CNABLineDataDto(
            TransactionTypes.Sales,
            new DateOnly(2019, 03, 01),
            1000,
            "12345678901",
            "1234567890",
            new TimeOnly(10, 30, 00),
            "Owner",
            storeName
        );

        // Act
        var act = () => _sut.Create(data);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Store name cannot be empty");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_InvalidStoreOwner_ShouldThrowArgumentException(string storeOwner)
    {
        // Arrange
        var data = new CNABLineDataDto(
            TransactionTypes.Sales,
            new DateOnly(2019, 03, 01),
            1000,
            "12345678901",
            "1234567890",
            new TimeOnly(10, 30, 00),
            storeOwner,
            "Store"
        );

        // Act
        var act = () => _sut.Create(data);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Store owner cannot be empty");
    }

    [Fact]
    public void Create_NegativeAmount_ShouldThrowArgumentException()
    {
        // Arrange
        var data = new CNABLineDataDto(
            TransactionTypes.Sales,
            new DateOnly(2019, 03, 01),
            -1000,
            "12345678901",
            "1234567890",
            new TimeOnly(10, 30, 00),
            "Owner",
            "Store"
        );

        // Act
        var act = () => _sut.Create(data);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Amount cannot be negative");
    }

    [Fact]
    public void Create_ZeroAmount_ShouldCreateTransaction()
    {
        // Arrange
        var data = CreateValidCNABLineDataDto(TransactionTypes.Sales, amount: 0);

        // Act
        var result = _sut.Create(data);

        // Assert
        result.Should().NotBeNull();
        result.AmountCNAB.Should().Be(0);
    }

    [Fact]
    public void Create_MultipleTransactions_ShouldCreateIndependentInstances()
    {
        // Arrange
        var data1 = CreateValidCNABLineDataDto(TransactionTypes.Sales);
        var data2 = CreateValidCNABLineDataDto(TransactionTypes.Debit);

        // Act
        var result1 = _sut.Create(data1);
        var result2 = _sut.Create(data2);

        // Assert
        result1.Should().NotBeSameAs(result2);
        result1.Id.Should().NotBe(result2.Id);
    }

    // Helper method
    private CNABLineDataDto CreateValidCNABLineDataDto(
        TransactionTypes type,
        decimal amount = 14200)
    {
        return new CNABLineDataDto(
            type,
            new DateOnly(2019, 03, 01),
            amount,
            "09620676017",
            "4753****3153",
            new TimeOnly(15, 34, 53),
            "JOÃO MACEDO",
            "BAR DO JOÃO"
        );
    }
}
