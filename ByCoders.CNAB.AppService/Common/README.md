# Result Pattern

## 📋 Visão Geral

Implementação simples do **Result Pattern** para evitar o uso de exceptions para controle de fluxo e permitir o processamento contínuo de erros.

## 🏗️ Estrutura

### Result<T>
Representa o resultado de uma operação que retorna um valor:

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure { get; }
    public T? Value { get; }
    public string Error { get; }
}
```

### Result
Representa o resultado de uma operação sem valor de retorno:

```csharp
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure { get; }
    public string Error { get; }
}
```

## 💡 Como Usar

### Criando Resultados

#### Sucesso com valor
```csharp
var result = Result<Transaction>.Success(transaction);
```

#### Sucesso sem valor
```csharp
var result = Result.Success();
```

#### Falha
```csharp
var result = Result<Transaction>.Failure("Transaction not found");
```

### Verificando Resultados

```csharp
var parseResult = parser.Parse(line);

if (parseResult.IsFailure)
{
    // Lidar com erro
    Console.WriteLine(parseResult.Error);
    return;
}

// Usar o valor
var data = parseResult.Value;
```

### Propagando Erros no Handler

```csharp
var parseResult = parser.Parse(line);
if (parseResult.IsFailure)
{
    errors.Add($"Line {lineNumber}: {parseResult.Error}");
    continue; // Continua processando próxima linha
}

var createResult = factory.Create(parseResult.Value);
if (createResult.IsFailure)
{
    errors.Add($"Line {lineNumber}: {createResult.Error}");
    continue; // Continua processando próxima linha
}

transactions.Add(createResult.Value);
```

## 📂 Implementação no Projeto

```
Common/
└── Result.cs               # Classes Result<T> e Result

Transactions/CNAB/Import/
├── CNABLineParser.cs       # Retorna Result<CNABLineDataDto>
├── TransactionFactory.cs   # Retorna Result<Transaction>
└── ImportCNABRequestHandler.cs # Coleta erros e continua processamento
```

## 🔄 Antes vs Depois

### ❌ Antes (Com Exceptions)
```csharp
public CNABLineDataDto Parse(string line)
{
    if (string.IsNullOrWhiteSpace(line))
        throw new ArgumentException("CNAB line cannot be empty");
    
    // Parse...
}

// Uso - Para no primeiro erro
try
{
    var data = parser.Parse(line);
    // processar
}
catch (ArgumentException ex)
{
    // lidar com erro - processamento para
}
```

### ✅ Depois (Com Result Pattern)
```csharp
public Result<CNABLineDataDto> Parse(string line)
{
    if (string.IsNullOrWhiteSpace(line))
        return Result<CNABLineDataDto>.Failure("CNAB line cannot be empty");
    
    try
    {
        // Parse...
        return Result<CNABLineDataDto>.Success(data);
    }
    catch (Exception ex)
    {
        return Result<CNABLineDataDto>.Failure($"Error parsing: {ex.Message}");
    }
}

// Uso - Coleta TODOS os erros
var result = parser.Parse(line);
if (result.IsFailure)
{
    errors.Add($"Line {n}: {result.Error}");
    continue; // Continua processando outras linhas
}

var data = result.Value;
```

## 🎯 Benefícios

### 1. Sem Exceptions para Controle de Fluxo
- ✅ Exceptions são caras (stack trace)
- ✅ Result Pattern usa retorno normal
- ✅ Melhor performance

### 2. Processamento Contínuo
```csharp
// Processa 100 linhas, coleta TODOS os erros
foreach (var line in lines)
{
    var result = parser.Parse(line);
    if (result.IsFailure)
    {
        errors.Add($"Line {n}: {result.Error}");
        continue; // Continua processando
    }
    
    transactions.Add(result.Value);
}

// No final: X transações importadas, Y erros coletados
```

### 3. Código Explícito
```csharp
// Fica claro que o método pode falhar
Result<Transaction> result = factory.Create(data);

// Compilador força tratamento
if (result.IsSuccess)
{
    // usar result.Value
}
```

### 4. Railway Oriented Programming
```csharp
// Encadeamento de operações
var result = ParseLine(line);
if (result.IsFailure) return result;

var validated = ValidateData(result.Value);
if (validated.IsFailure) return validated;

// etc...
```

## 📊 Comparação

| Aspecto | Exceptions | Result Pattern |
|---------|-----------|----------------|
| Performance | ❌ Lenta | ✅ Rápida |
| Controle de Fluxo | ❌ Não linear | ✅ Linear |
| Explícito | ❌ Implícito | ✅ Explícito |
| Múltiplos Erros | ❌ Para no primeiro | ✅ Coleta todos |
| Stack Trace | ✅ Sim | ⚠️ Via try-catch interno |

## 🚀 Uso no Projeto

### CNABLineParser
- Valida formato de linha
- Retorna `Result<CNABLineDataDto>`
- Try-catch interno captura exceptions de parsing

### TransactionFactory
- Valida dados da transação
- Retorna `Result<Transaction>`
- Try-catch interno captura exceptions de criação

### ImportCNABRequestHandler
- Processa arquivo completo
- Coleta TODOS os erros com número da linha
- Continua processando linhas válidas
- Retorna: X importadas, Y erros

## ✨ Exemplo Completo

```csharp
// Parse
var parseResult = parser.Parse(line);
if (parseResult.IsFailure)
{
    errors.Add($"Line {n}: {parseResult.Error}");
    continue;
}

// Create
var createResult = factory.Create(parseResult.Value);
if (createResult.IsFailure)
{
    errors.Add($"Line {n}: {createResult.Error}");
    continue;
}

// Success - usar o valor
transactions.Add(createResult.Value);
```

Resultado final:
```
Success: false (tem erros)
TransactionsImported: 18
Errors:
  - Line 2: CNAB line must be at least 81 characters. Got 20
  - Line 5: Invalid transaction type: 0
  - Line 9: CPF cannot be empty
```
