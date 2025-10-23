# ByCoders.CNAB.Core

## 📋 Visão Geral

Camada **Core** do sistema CNAB contendo abstrações fundamentais e patterns utilizados em toda a aplicação.

## 🏗️ Estrutura

```
ByCoders.CNAB.Core/
├── Handlers/
│   └── IRequestHandler.cs      # Interface CQRS para handlers
├── Results/
│   └── Result.cs               # Result Pattern para controle de fluxo
└── README.md
```

## 📦 Componentes

### 1. **IRequestHandler** (Handlers/)

Interface baseada no padrão **CQRS/Mediator** para handlers de requisições.

```csharp
public interface IRequestHandler<TRequest, TResponse>
    where TRequest : RequestDto
    where TResponse : ResponseDto
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
```

#### Base Classes
- `RequestDto`: Classe base para requisições
- `ResponseDto`: Classe base para respostas

#### Uso
```csharp
// Handler implementa a interface
public class ImportCNABRequestHandler : IRequestHandler<ImportCNABRequest, ImportCNABResponse>
{
    public async Task<ImportCNABResponse> Handle(
        ImportCNABRequest request, 
        CancellationToken cancellationToken)
    {
        // Implementação
    }
}

// Request herda de RequestDto
public record ImportCNABRequest : RequestDto
{
    public IFormFile CNABFile { get; set; }
}

// Response herda de ResponseDto
public record ImportCNABResponse : ResponseDto
{
    public bool Success { get; set; }
    public int TransactionsImported { get; set; }
}
```

### 2. **Result Pattern** (Results/)

Pattern para representar o resultado de operações sem usar exceptions para controle de fluxo.

```csharp
// Com valor
public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure { get; }
    public T? Value { get; }
    public string Error { get; }
    
    public static Result<T> Success(T value);
    public static Result<T> Failure(string error);
}

// Sem valor
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure { get; }
    public string Error { get; }
    
    public static Result Success();
    public static Result Failure(string error);
}
```

#### Uso
```csharp
// Retornar sucesso
public Result<Transaction> Create(Data data)
{
    var transaction = new Transaction(data);
    return Result<Transaction>.Success(transaction);
}

// Retornar falha
public Result<Transaction> Create(Data data)
{
    if (data == null)
        return Result<Transaction>.Failure("Data cannot be null");
        
    // ...
}

// Verificar resultado
var result = factory.Create(data);
if (result.IsFailure)
{
    Console.WriteLine(result.Error);
    return;
}

var transaction = result.Value;
```

## 🎯 Benefícios do Core

### 1. **Desacoplamento**
- Abstrações compartilhadas entre camadas
- Domain não depende de infraestrutura
- AppService depende apenas de abstrações

### 2. **Consistência**
- Todos os handlers seguem o mesmo padrão
- Result Pattern usado consistentemente
- Contratos bem definidos

### 3. **Testabilidade**
- Interfaces facilitam mocking
- Result Pattern facilita testes
- Sem dependências externas

### 4. **Manutenibilidade**
- Mudanças centralizadas
- Fácil adicionar novos handlers
- Pattern conhecido (CQRS)

## 📂 Dependências

```
Core (sem dependências)
  ↑
  ├── AppService
  ├── Domain
  └── Infrastructure
```

**Nenhuma dependência externa** - Core é a camada mais interna.

## 🚀 Como Usar em Outros Projetos

### 1. Adicionar referência ao Core
```xml
<ProjectReference Include="..\ByCoders.CNAB.Core\ByCoders.CNAB.Core.csproj" />
```

### 2. Importar namespaces
```csharp
using ByCoders.CNAB.Core;              // IRequestHandler, RequestDto, ResponseDto
using ByCoders.CNAB.Core.Results;     // Result, Result<T>
```

### 3. Implementar handlers
```csharp
public class MeuHandler : IRequestHandler<MeuRequest, MeuResponse>
{
    public async Task<MeuResponse> Handle(MeuRequest request, CancellationToken ct)
    {
        // Implementação
    }
}
```

### 4. Usar Result Pattern
```csharp
public Result<Data> ProcessData(string input)
{
    if (string.IsNullOrEmpty(input))
        return Result<Data>.Failure("Input cannot be empty");
        
    try
    {
        var data = Parse(input);
        return Result<Data>.Success(data);
    }
    catch (Exception ex)
    {
        return Result<Data>.Failure($"Error: {ex.Message}");
    }
}
```

## 📊 Arquitetura

```
┌─────────────────────────────────────────┐
│          Presentation Layer             │
│         (API, Controllers)              │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│          Application Layer               │
│    (Handlers, Services, UseCases)       │
│    Usa: IRequestHandler, Result         │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│            Domain Layer                  │
│      (Entities, Value Objects)          │
│    Usa: Result (opcional)               │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│        Infrastructure Layer              │
│    (Repositories, External Services)    │
│    Usa: Result (opcional)               │
└─────────────────────────────────────────┘

              ┌─────────┐
              │  CORE   │ ← Usado por todas as camadas
              └─────────┘
```

## ✨ Exemplos Práticos

### Exemplo Completo: Importação CNAB

```csharp
// 1. Request
public record ImportCNABRequest : RequestDto
{
    public IFormFile CNABFile { get; set; }
}

// 2. Response
public record ImportCNABResponse : ResponseDto
{
    public bool Success { get; set; }
    public int TransactionsImported { get; set; }
    public List<string> Errors { get; set; }
}

// 3. Handler
public class ImportCNABRequestHandler : IRequestHandler<ImportCNABRequest, ImportCNABResponse>
{
    public async Task<ImportCNABResponse> Handle(
        ImportCNABRequest request, 
        CancellationToken cancellationToken)
    {
        var transactions = new List<Transaction>();
        var errors = new List<string>();
        
        foreach (var line in lines)
        {
            // Usa Result Pattern
            var parseResult = parser.Parse(line);
            if (parseResult.IsFailure)
            {
                errors.Add($"Line {n}: {parseResult.Error}");
                continue;
            }
            
            var createResult = factory.Create(parseResult.Value);
            if (createResult.IsFailure)
            {
                errors.Add($"Line {n}: {createResult.Error}");
                continue;
            }
            
            transactions.Add(createResult.Value);
        }
        
        return new ImportCNABResponse
        {
            Success = errors.Count == 0,
            TransactionsImported = transactions.Count,
            Errors = errors
        };
    }
}
```

## 🔗 Links Úteis

- **CQRS Pattern**: Separação de leitura e escrita
- **Mediator Pattern**: Desacoplamento de handlers
- **Result Pattern**: Controle de fluxo explícito
- **Railway Oriented Programming**: Encadeamento de operações

## 📝 Notas

- Core não deve ter dependências externas
- Mantenha abstrações simples e focadas
- Use Result Pattern para operações que podem falhar
- IRequestHandler para todos os use cases
