# Testes Unitários - ByCoders CNAB

Este projeto contém os testes unitários para a camada **AppService** do sistema de importação CNAB.

## 🧪 Tecnologias de Teste

- **xUnit** - Framework de testes
- **FluentAssertions** - Assertions fluentes e expressivas
- **NSubstitute** - Biblioteca de mocking
- **NBuilder** - Construção de objetos para testes

## 📁 Estrutura dos Testes

```
AppService/
├── CNABLineParserTests.cs          # Testes do parser de linhas CNAB
├── TransactionFactoryTests.cs      # Testes da factory de transações
└── ImportCNABRequestHandlerTests.cs # Testes do handler de importação
```

## 🎯 Cobertura de Testes

### CNABLineParserTests (19 testes)
✅ Parsing de linhas válidas  
✅ Validação de formato  
✅ Todos os tipos de transação (1-9)  
✅ Tratamento de erros (linha vazia, curta, tipo inválido, etc.)  
✅ Edge cases (valores zero, grandes, espaços, etc.)

### TransactionFactoryTests (20 testes)
✅ Criação de todas as subclasses (Debit, Sale, Credit, etc.)  
✅ Mapeamento correto de propriedades  
✅ Validações (CPF, Cartão, Loja, Valor)  
✅ Tratamento de dados nulos ou inválidos  
✅ Independência entre instâncias

### ImportCNABRequestHandlerTests (14 testes)
✅ Importação de arquivo válido  
✅ Processamento paralelo  
✅ Arquivo vazio  
✅ Múltiplas transações  
✅ Todos os tipos de transação  
✅ Arquivo real (21 linhas do CNAB.txt)  
✅ Performance com 100 linhas  
✅ Cancellation token  
✅ Diferentes lojas

**Total: 53 testes unitários**

## ▶️ Como Executar

### Visual Studio
1. Abra o Test Explorer (View → Test Explorer)
2. Click em "Run All" para executar todos os testes

### Linha de Comando
```bash
# Executar todos os testes
dotnet test

# Executar com detalhes
dotnet test --logger "console;verbosity=detailed"

# Executar testes específicos
dotnet test --filter "FullyQualifiedName~CNABLineParserTests"

# Gerar relatório de cobertura
dotnet test /p:CollectCoverage=true
```

### Rider
1. Clique com botão direito no projeto de testes
2. Selecione "Run Unit Tests"

## 📊 Cenários Testados

### Casos de Sucesso
- ✅ Parsing correto de todas as posições do CNAB
- ✅ Criação de 9 tipos diferentes de transações
- ✅ Importação de arquivos com múltiplas linhas
- ✅ Processamento paralelo eficiente

### Casos de Erro
- ❌ Linha vazia ou nula
- ❌ Linha com menos de 81 caracteres
- ❌ Tipo de transação inválido (0, 10, letras)
- ❌ Data inválida
- ❌ Valor negativo
- ❌ CPF, Cartão ou Loja vazios

### Edge Cases
- ⚠️ Valor zero
- ⚠️ Valores muito grandes (9999999999)
- ⚠️ Linhas com espaços extras
- ⚠️ Arquivo com linhas em branco entre dados
- ⚠️ Arquivo com 100+ transações

## 🏗️ Padrões de Teste

### Arrange-Act-Assert (AAA)
Todos os testes seguem o padrão AAA para clareza:
```csharp
[Fact]
public void TestMethod()
{
    // Arrange - Preparar dados
    var data = CreateTestData();
    
    // Act - Executar ação
    var result = _sut.Method(data);
    
    // Assert - Verificar resultado
    result.Should().Be(expected);
}
```

### System Under Test (SUT)
Cada classe de teste define `_sut` para o objeto sendo testado:
```csharp
private readonly CNABLineParser _sut;
```

### Teoria vs Fato
- `[Fact]` - Teste único
- `[Theory]` - Teste parametrizado com múltiplos valores

## 🔍 Exemplos de Uso

### FluentAssertions
```csharp
result.Should().NotBeNull();
result.TransactionType.Should().Be(TransactionTypes.Sales);
result.Amount.Should().Be(14200);
```

### NSubstitute (Mock)
```csharp
var formFile = Substitute.For<IFormFile>();
formFile.OpenReadStream().Returns(stream);
```

### Theory com InlineData
```csharp
[Theory]
[InlineData(null)]
[InlineData("")]
[InlineData("   ")]
public void Test_InvalidInput(string input)
{
    // Test implementation
}
```

## 🚀 Próximos Passos

- [ ] Testes de integração
- [ ] Testes da camada Domain
- [ ] Testes da camada Infrastructure
- [ ] Geração de relatório de cobertura
- [ ] Integração com CI/CD

## 📝 Notas

- Todos os testes são independentes e podem ser executados em qualquer ordem
- Não há dependência de banco de dados ou recursos externos
- Mocks são usados para isolar componentes
- Performance é testada com arquivos grandes (100 linhas)
