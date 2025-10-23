# Testes Unitários - Domain Layer

## 📋 Visão Geral

Testes unitários completos para a camada de **Domain**, cobrindo entidades, value objects, e regras de negócio das transações CNAB.

## 🧪 Estrutura dos Testes

```
Domain/
├── TransactionTests.cs              # Testes da classe base Transaction
├── TransactionTypesTests.cs         # Testes de todos os 9 tipos de transação
├── TransactionTypeTests.cs          # Testes da classe TransactionType
├── BusinessScenariosTests.cs        # Cenários de negócio e casos de uso
└── README.md
```

## 📊 Cobertura de Testes

### TransactionTests (13 testes)
✅ Geração automática de ID  
✅ Timestamp de criação (CreatedOn)  
✅ Mapeamento de propriedades  
✅ Cálculo de TransactionValue (AmountCNAB / 100)  
✅ Value Objects (Beneficiary, Card, Store)  
✅ Unicidade de IDs  
✅ Edge cases (zero, grandes valores)

### TransactionTypesTests (28 testes)
✅ Todos os 9 tipos de transação  
✅ Valores positivos (Cash In): Credit, Sale, LoanReceipt, TEDReceipt, DOCReceipt  
✅ Valores negativos (Cash Out): Debit, BankSlip, Funding, Rent  
✅ Mapeamento correto de TransactionTypes → Classes  
✅ Igualdade de Value Objects  
✅ Regras de negócio (sinais corretos)  
✅ Precisão decimal

### TransactionTypeTests (7 testes)
✅ Criação com TransactionTypes enum  
✅ Propriedades: Id, Description, Nature  
✅ Mapeamento enum → int (1-9)  
✅ Igualdade de TransactionType  
✅ TransactionNature enum

### BusinessScenariosTests (9 testes)
✅ Cálculo de saldo com múltiplas transações  
✅ Agrupamento por loja (Store)  
✅ Relatórios diários  
✅ Exemplo real do CNAB.txt  
✅ Filtros por beneficiário  
✅ Agrupamento por cartão  
✅ Separação Cash In / Cash Out  
✅ Relatórios mensais consolidados

**Total: 57 testes unitários**

## 🎯 Tipos de Transação Testados

### Cash In (Entrada de Dinheiro) ➕
| Tipo | ID | Classe | Sinal |
|------|-----|--------|-------|
| Débito | 1 | `Debit` | - |
| Crédito | 4 | `Credit` | + |
| Venda | 6 | `Sale` | + |
| Recebimento Empréstimo | 5 | `LoanReceipt` | + |
| Recebimento TED | 7 | `TEDReceipt` | + |
| Recebimento DOC | 8 | `DOCReceipt` | + |

### Cash Out (Saída de Dinheiro) ➖
| Tipo | ID | Classe | Sinal |
|------|-----|--------|-------|
| Boleto | 2 | `BankSlip` | - |
| Financiamento | 3 | `Funding` | - |
| Aluguel | 9 | `Rent` | - |

**Nota:** Débito (tipo 1) é negativo pois representa saída de dinheiro da conta.

## 💡 Conceitos Testados

### 1. Entidades
```csharp
Transaction (classe abstrata)
├── Properties: Id, CreatedOn, TransactionDate, TransactionTime, AmountCNAB
├── Method: TransactionValue (virtual)
└── Subclasses: Sale, Debit, Credit, BankSlip, etc.
```

### 2. Value Objects
```csharp
Beneficiary(string Document)    // CPF do beneficiário
Card(string Number)              // Número do cartão
Store(string Name, string Owner) // Dados da loja
```

### 3. Regras de Negócio

#### Cálculo de Valor
```csharp
// Valor base (positivo)
TransactionValue = AmountCNAB / 100

// Transações de saída (negativo)
TransactionValue = -AmountCNAB / 100
```

#### Tipos com Override Negativo
- `Debit`: Saída de dinheiro
- `BankSlip`: Pagamento de boleto
- `Funding`: Financiamento
- `Rent`: Pagamento de aluguel

## 🔍 Exemplos de Testes

### Teste Básico
```csharp
[Fact]
public void Sale_ShouldHavePositiveValue()
{
    // Arrange
    var amount = 10000m; // R$ 100.00 no CNAB
    
    // Act
    var transaction = new Sale(date, time, amount, beneficiary, card, store);
    
    // Assert
    transaction.TransactionValue.Should().Be(100m);
}
```

### Teste de Negativo
```csharp
[Fact]
public void Debit_ShouldHaveNegativeValue()
{
    // Arrange
    var amount = 10000m;
    
    // Act
    var transaction = new Debit(date, time, amount, beneficiary, card, store);
    
    // Assert
    transaction.TransactionValue.Should().Be(-100m); // Negativo!
}
```

### Teste de Cenário
```csharp
[Fact]
public void CalculateBalance_WithMultipleTransactions_ShouldReturnCorrectBalance()
{
    // Arrange
    var transactions = new List<Transaction>
    {
        new Sale(date, time, 10000, ...),      // +100.00
        new Debit(date, time, 5000, ...),      // -50.00
        new Credit(date, time, 20000, ...)     // +200.00
    };
    
    // Act
    var balance = transactions.Sum(t => t.TransactionValue);
    
    // Assert
    balance.Should().Be(250m);
}
```

### Teste com Exemplo Real
```csharp
[Fact]
public void Transaction_RealWorldCNABExample_ShouldCalculateCorrectly()
{
    // Linha CNAB: 3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO
    var beneficiary = new Beneficiary("09620676017");
    var card = new Card("4753****3153");
    var store = new Store("BAR DO JOÃO", "JOÃO MACEDO");
    var date = new DateOnly(2019, 03, 01);
    var time = new TimeOnly(15, 34, 53);
    var amount = 142m;
    
    var transaction = new Funding(date, time, amount, beneficiary, card, store);
    
    transaction.TransactionValue.Should().Be(-1.42m); // Funding é negativo
}
```

## 📈 Cenários de Negócio Testados

### 1. Saldo por Loja
```csharp
var store1Balance = transactions
    .Where(t => t.Store == store1)
    .Sum(t => t.TransactionValue);
```

### 2. Resumo Diário
```csharp
var dailySummary = transactions
    .GroupBy(t => t.TransactionDate)
    .Select(g => new {
        Date = g.Key,
        Total = g.Sum(t => t.TransactionValue)
    });
```

### 3. Cash In vs Cash Out
```csharp
var cashIn = transactions.Where(t => t.TransactionValue > 0).Sum(t => t.TransactionValue);
var cashOut = transactions.Where(t => t.TransactionValue < 0).Sum(t => t.TransactionValue);
```

### 4. Relatório por Cartão
```csharp
var byCard = transactions
    .GroupBy(t => t.Card)
    .Select(g => new {
        Card = g.Key,
        Total = g.Sum(t => t.TransactionValue)
    });
```

## ▶️ Como Executar

### Todos os testes do Domain
```bash
dotnet test --filter "FullyQualifiedName~Domain"
```

### Teste específico
```bash
dotnet test --filter "FullyQualifiedName~TransactionTests"
```

### Com detalhes
```bash
dotnet test --filter "FullyQualifiedName~Domain" --logger "console;verbosity=detailed"
```

## 🎨 Padrões Utilizados

### AAA Pattern
```csharp
// Arrange - Preparar
var transaction = new Sale(...);

// Act - Executar
var value = transaction.TransactionValue;

// Assert - Verificar
value.Should().Be(100m);
```

### Theory com InlineData
```csharp
[Theory]
[InlineData(100, 1)]
[InlineData(1000, 10)]
public void Test(decimal input, decimal expected)
{
    // Test implementation
}
```

### FluentAssertions
```csharp
transaction.Should().NotBeNull();
transaction.Id.Should().NotBeEmpty();
transaction.TransactionValue.Should().Be(100m);
transaction.Store.Should().Be(store);
```

## 🔄 Value Objects

Os Value Objects são records imutáveis e possuem igualdade por valor:

```csharp
var beneficiary1 = new Beneficiary("12345678901");
var beneficiary2 = new Beneficiary("12345678901");

beneficiary1.Should().Be(beneficiary2); // ✅ São iguais
beneficiary1.GetHashCode().Should().Be(beneficiary2.GetHashCode()); // ✅ Mesmo hash
```

## 📊 Cobertura

| Classe | Cobertura | Testes |
|--------|-----------|--------|
| Transaction (base) | 100% | 13 |
| Sale | 100% | Incluído em TypesTests |
| Debit | 100% | Incluído em TypesTests |
| Credit | 100% | Incluído em TypesTests |
| BankSlip | 100% | Incluído em TypesTests |
| Funding | 100% | Incluído em TypesTests |
| Rent | 100% | Incluído em TypesTests |
| LoanReceipt | 100% | Incluído em TypesTests |
| TEDReceipt | 100% | Incluído em TypesTests |
| DOCReceipt | 100% | Incluído em TypesTests |
| TransactionType | 100% | 7 |
| Beneficiary | 100% | Incluído em TransactionTests |
| Card | 100% | Incluído em TransactionTests |
| Store | 100% | Incluído em TransactionTests |

## 🚀 Próximos Passos

- [ ] Testes de integração com Repository
- [ ] Testes de performance com grandes volumes
- [ ] Testes de validação de domínio
- [ ] Benchmarks de cálculos

## 📝 Notas Importantes

1. **AmountCNAB vs TransactionValue**
   - `AmountCNAB`: Valor raw do arquivo (ex: 14200)
   - `TransactionValue`: Valor calculado (ex: 142.00 ou -142.00)

2. **Sinais Negativos**
   - Transações de **saída** têm `TransactionValue` negativo
   - Override em: Debit, BankSlip, Funding, Rent

3. **Value Objects**
   - São records (igualdade por valor)
   - Imutáveis
   - Sem lógica de negócio

4. **Entidades**
   - Transaction tem identidade (Id)
   - CreatedOn gerado automaticamente
   - Subclasses definem comportamento específico
