using ByCoders.CNAB.Domain.Transactions.Models;
using FluentAssertions;
using Xunit;

namespace ByCoders.CNAB.UnitTests.Domain;

public class BusinessScenariosTests
{
    [Fact]
    public void CalculateBalance_WithMultipleTransactions_ShouldReturnCorrectBalance()
    {
        // Arrange
        var beneficiary = new Beneficiary("12345678901");
        var card = new Card("1234****5678");
        var store = new Store("Test Store", "John Doe");
        var date = new DateOnly(2019, 03, 01);
        var time = new TimeOnly(10, 00, 00);

        var transactions = new List<Transaction>
        {
            new Sale(date, time, 10000, beneficiary, card, store),      // +100.00
            new Debit(date, time, 5000, beneficiary, card, store),      // +50.00 (ENTRADA conforme README.md)
            new Credit(date, time, 20000, beneficiary, card, store),    // +200.00
            new BankSlip(date, time, 3000, beneficiary, card, store),   // -30.00
            new Funding(date, time, 15000, beneficiary, card, store)    // -150.00
        };

        // Act
        var balance = transactions.Sum(t => t.TransactionValue);

        // Assert
        balance.Should().Be(170m); // +100 +50 +200 -30 -150 = +170
    }

    [Fact]
    public void StoreBalance_GroupByStore_ShouldCalculateCorrectly()
    {
        // Arrange
        var beneficiary = new Beneficiary("12345678901");
        var card = new Card("1234****5678");
        var store1 = new Store("Store 1", "Owner 1");
        var store2 = new Store("Store 2", "Owner 2");
        var date = new DateOnly(2019, 03, 01);
        var time = new TimeOnly(10, 00, 00);

        var transactions = new List<Transaction>
        {
            new Sale(date, time, 10000, beneficiary, card, store1),     // Store1: +100
            new Sale(date, time, 5000, beneficiary, card, store1),      // Store1: +50
            new Debit(date, time, 2000, beneficiary, card, store1),     // Store1: +20 (ENTRADA)
            new Sale(date, time, 15000, beneficiary, card, store2),     // Store2: +150
            new BankSlip(date, time, 3000, beneficiary, card, store2)   // Store2: -30
        };

        // Act
        var store1Balance = transactions
            .Where(t => t.Store == store1)
            .Sum(t => t.TransactionValue);

        var store2Balance = transactions
            .Where(t => t.Store == store2)
            .Sum(t => t.TransactionValue);

        // Assert
        store1Balance.Should().Be(170m); // +100 +50 +20 = +170
        store2Balance.Should().Be(120m); // +150 -30 = +120
    }

    [Fact]
    public void DailySummary_GroupByDate_ShouldWork()
    {
        // Arrange
        var beneficiary = new Beneficiary("12345678901");
        var card = new Card("1234****5678");
        var store = new Store("Test Store", "John Doe");
        var date1 = new DateOnly(2019, 03, 01);
        var date2 = new DateOnly(2019, 03, 02);
        var time = new TimeOnly(10, 00, 00);

        var transactions = new List<Transaction>
        {
            new Sale(date1, time, 10000, beneficiary, card, store),
            new Sale(date1, time, 5000, beneficiary, card, store),
            new Sale(date2, time, 15000, beneficiary, card, store)
        };

        // Act
        var summaryByDate = transactions
            .GroupBy(t => t.TransactionDate)
            .Select(g => new
            {
                Date = g.Key,
                Total = g.Sum(t => t.TransactionValue),
                Count = g.Count()
            })
            .ToList();

        // Assert
        summaryByDate.Should().HaveCount(2);
        summaryByDate.First(s => s.Date == date1).Total.Should().Be(150m);
        summaryByDate.First(s => s.Date == date1).Count.Should().Be(2);
        summaryByDate.First(s => s.Date == date2).Total.Should().Be(150m);
        summaryByDate.First(s => s.Date == date2).Count.Should().Be(1);
    }

    [Fact]
    public void Transaction_RealWorldCNABExample_ShouldCalculateCorrectly()
    {
        // Arrange - Exemplo real do CNAB.txt
        // Linha: 3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO
        var beneficiary = new Beneficiary("09620676017");
        var card = new Card("4753****3153");
        var store = new Store("BAR DO JOÃO", "JOÃO MACEDO");
        var date = new DateOnly(2019, 03, 01);
        var time = new TimeOnly(15, 34, 53);
        var amount = 142m; // Amount from CNAB file (already divided by 100 would be 1.42)

        // Act
        var transaction = new Funding(date, time, amount, beneficiary, card, store);

        // Assert
        transaction.TransactionType.Id.Should().Be(3);
        transaction.TransactionDate.Should().Be(date);
        transaction.TransactionTime.Should().Be(time);
        transaction.AmountCNAB.Should().Be(amount);
        transaction.TransactionValue.Should().Be(-1.42m); // Funding is negative (Cash Out)
        transaction.Beneficiary.Document.Should().Be("09620676017");
        transaction.Card.Number.Should().Be("4753****3153");
        transaction.Store.Name.Should().Be("BAR DO JOÃO");
        transaction.Store.Owner.Should().Be("JOÃO MACEDO");
    }

    [Fact]
    public void BeneficiaryTransactions_FilterByBeneficiary_ShouldWork()
    {
        // Arrange
        var beneficiary1 = new Beneficiary("11111111111");
        var beneficiary2 = new Beneficiary("22222222222");
        var card = new Card("1234****5678");
        var store = new Store("Test Store", "John Doe");
        var date = new DateOnly(2019, 03, 01);
        var time = new TimeOnly(10, 00, 00);

        var transactions = new List<Transaction>
        {
            new Sale(date, time, 10000, beneficiary1, card, store),
            new Sale(date, time, 5000, beneficiary1, card, store),
            new Sale(date, time, 15000, beneficiary2, card, store)
        };

        // Act
        var beneficiary1Transactions = transactions
            .Where(t => t.Beneficiary == beneficiary1)
            .ToList();

        // Assert
        beneficiary1Transactions.Should().HaveCount(2);
        beneficiary1Transactions.Sum(t => t.TransactionValue).Should().Be(150m);
    }

    [Fact]
    public void CardTransactions_GroupByCard_ShouldWork()
    {
        // Arrange
        var beneficiary = new Beneficiary("12345678901");
        var card1 = new Card("1111****1111");
        var card2 = new Card("2222****2222");
        var store = new Store("Test Store", "John Doe");
        var date = new DateOnly(2019, 03, 01);
        var time = new TimeOnly(10, 00, 00);

        var transactions = new List<Transaction>
        {
            new Sale(date, time, 10000, beneficiary, card1, store),
            new Sale(date, time, 5000, beneficiary, card1, store),
            new Sale(date, time, 15000, beneficiary, card2, store)
        };

        // Act
        var byCard = transactions
            .GroupBy(t => t.Card)
            .Select(g => new
            {
                Card = g.Key,
                Total = g.Sum(t => t.TransactionValue),
                Count = g.Count()
            })
            .ToList();

        // Assert
        byCard.Should().HaveCount(2);
        byCard.First(c => c.Card == card1).Total.Should().Be(150m);
        byCard.First(c => c.Card == card1).Count.Should().Be(2);
    }

    [Fact]
    public void TransactionsByType_GroupByCashInCashOut_ShouldWork()
    {
        // Arrange
        var beneficiary = new Beneficiary("12345678901");
        var card = new Card("1234****5678");
        var store = new Store("Test Store", "John Doe");
        var date = new DateOnly(2019, 03, 01);
        var time = new TimeOnly(10, 00, 00);

        var transactions = new List<Transaction>
        {
            new Sale(date, time, 10000, beneficiary, card, store),      // Cash In: +100
            new Credit(date, time, 5000, beneficiary, card, store),     // Cash In: +50
            new Debit(date, time, 2000, beneficiary, card, store),      // Cash In: +20 (ENTRADA)
            new BankSlip(date, time, 3000, beneficiary, card, store)    // Cash Out: -30
        };

        // Act
        var cashIn = transactions.Where(t => t.TransactionValue > 0).Sum(t => t.TransactionValue);
        var cashOut = transactions.Where(t => t.TransactionValue < 0).Sum(t => t.TransactionValue);

        // Assert
        cashIn.Should().Be(170m);  // +100 +50 +20 = +170
        cashOut.Should().Be(-30m); // -30
    }

    [Fact]
    public void MonthlyReport_MultipleStores_ShouldCalculateCorrectly()
    {
        // Arrange - Simulating a month of transactions
        var store1 = new Store("BAR DO JOÃO", "JOÃO MACEDO");
        var store2 = new Store("LOJA DO Ó", "MARIA JOSEFINA");
        var store3 = new Store("MERCADO DA AVENIDA", "MARCOS PEREIRA");

        var beneficiary = new Beneficiary("12345678901");
        var card = new Card("1234****5678");
        var date = new DateOnly(2019, 03, 01);
        var time = new TimeOnly(10, 00, 00);

        var transactions = new List<Transaction>
        {
            // Store 1
            new Sale(date, time, 14200, beneficiary, card, store1),
            new Debit(date, time, 15200, beneficiary, card, store1),
            // Store 2
            new Credit(date, time, 13200, beneficiary, card, store2),
            new Sale(date, time, 20000, beneficiary, card, store2),
            // Store 3
            new Funding(date, time, 12200, beneficiary, card, store3),
            new BankSlip(date, time, 11200, beneficiary, card, store3)
        };

        // Act
        var report = transactions
            .GroupBy(t => t.Store)
            .Select(g => new
            {
                Store = g.Key,
                Balance = g.Sum(t => t.TransactionValue),
                TransactionCount = g.Count()
            })
            .OrderByDescending(r => r.Balance)
            .ToList();

        // Assert
        report.Should().HaveCount(3);
        
        // Store 2 should have the highest balance
        report[0].Store.Should().Be(store2);
        report[0].Balance.Should().Be(332m); // 132 + 200
        
        // Store 1 - Debit agora é ENTRADA (+)
        var store1Report = report.First(r => r.Store == store1);
        store1Report.Balance.Should().Be(294m); // +142 +152 = +294
        
        // Store 3 should have negative balance
        var store3Report = report.First(r => r.Store == store3);
        store3Report.Balance.Should().Be(-234m); // -122 - 112
    }
}
