# Sistema de Gerenciamento CNAB - React + TypeScript

Sistema completo para upload, parsing e consulta de arquivos CNAB desenvolvido em React 18 com TypeScript.

## 🎯 Sobre o Projeto

Implementação completa do desafio **ByCodersTec** de processamento de arquivos CNAB usando React, TypeScript, React Router e Axios.

### Tecnologias Utilizadas
- ⚛️ React 18.2
- 📘 TypeScript 4.9
- 🛣️ React Router DOM 6
- 📡 Axios para HTTP
- 🎨 CSS3 puro (sem frameworks)
- 🪝 React Hooks (useState, useEffect)

---

## 📋 Especificação do Arquivo CNAB

### Formato do Arquivo
Cada linha contém **exatamente 80 caracteres**:

| Campo | Posições | Tamanho | Descrição |
|-------|----------|---------|-----------|
| Tipo | 1 | 1 | Tipo da transação (1-9) |
| Data | 2-9 | 8 | YYYYMMDD |
| Valor | 10-19 | 10 | Centavos (÷100) |
| CPF | 20-30 | 11 | CPF do cliente |
| Cartão | 31-42 | 12 | Número do cartão |
| Hora | 43-48 | 6 | HHMMSS |
| Dono | 49-62 | 14 | Nome do dono |
| Loja | 63-81 | 19 | Nome da loja |

### Tipos de Transação

| Tipo | Descrição | Natureza | Sinal |
|------|-----------|----------|-------|
| 1 | Débito | Entrada | + |
| 2 | Boleto | Saída | - |
| 3 | Financiamento | Saída | - |
| 4 | Crédito | Entrada | + |
| 5 | Receb. Empréstimo | Entrada | + |
| 6 | Vendas | Entrada | + |
| 7 | Receb. TED | Entrada | + |
| 8 | Receb. DOC | Entrada | + |
| 9 | Aluguel | Saída | - |

---

## 🚀 Instalação e Execução

### 1. Pré-requisitos
```bash
Node.js 16+
npm 8+ ou yarn 1.22+
```

### 2. Criar Projeto (Método 1 - Criar do Zero)
```bash
# Criar novo projeto React com TypeScript
npx create-react-app cnab-system --template typescript
cd cnab-system

# Instalar dependências adicionais
npm install react-router-dom axios
npm install --save-dev @types/react-router-dom

# Copiar todos os arquivos de src/ do projeto
```

### 3. Usar Projeto Pronto (Método 2 - Recomendado)
```bash
# Extrair o ZIP do projeto
cd cnab-react

# Instalar dependências
npm install

# Executar
npm start
```

A aplicação abrirá automaticamente em `http://localhost:3000`

---

## 📂 Estrutura do Projeto

```
cnab-react/
├── public/
│   └── index.html
├── src/
│   ├── components/
│   │   ├── UploadCnab/
│   │   │   ├── UploadCnab.tsx       # Componente de Upload
│   │   │   └── UploadCnab.css       # Estilos do Upload
│   │   └── ConsultaCnab/
│   │       ├── ConsultaCnab.tsx     # Componente de Consulta
│   │       └── ConsultaCnab.css     # Estilos da Consulta
│   ├── services/
│   │   └── cnabService.ts           # Parser e API calls
│   ├── types/
│   │   └── cnab.types.ts            # Interfaces TypeScript
│   ├── App.tsx                      # Componente principal
│   ├── App.css                      # Estilos globais
│   ├── index.tsx                    # Entry point
│   └── index.css                    # Reset CSS
├── exemplo-cnab.txt                 # Arquivo de teste
├── package.json                     # Dependências
├── tsconfig.json                    # Config TypeScript
└── README.md                        # Este arquivo
```

---

## ⚙️ Configuração da API

Antes de usar, configure a URL da sua API em `src/services/cnabService.ts`:

```typescript
const API_URL = 'http://localhost:8080/api/cnab'; // Altere aqui
```

---

## 🎯 Funcionalidades Implementadas

### ✅ Upload de Arquivo (`/upload`)
- Upload de arquivo CNAB (.txt ou .cnab)
- **Parser automático em tempo real**
- **Preview das transações** antes de enviar
- Validação de formato (80 caracteres)
- **Resumo por loja automático**
- **Cálculo de saldo** (entradas - saídas)
- Validação de tipos (1-9)
- Feedback visual completo

### ✅ Consulta de Transações (`/consulta`)
- Busca por nome da loja
- Filtro por data/hora (início e fim)
- **Validação de intervalo (máx 24h)**
- Indicador visual de intervalo
- Tabela de resultados responsiva
- Formatação de valores em R$
- Identificação de entrada/saída

### ✅ Parser CNAB Completo
```typescript
// Extração precisa dos campos
tipo: parseInt(linha.substring(0, 1))
data: parseData(linha.substring(1, 9))
valor: parseInt(linha.substring(9, 19)) / 100  // Normalização
cpf: linha.substring(19, 30).trim()
// ... demais campos
```

---

## 🔗 Endpoints da API

### 1. Upload
```http
POST /api/cnab/upload
Content-Type: multipart/form-data

FormData:
- arquivo: File

Response:
{
  "success": true,
  "message": "Arquivo processado",
  "transacoes": 10
}
```

### 2. Consulta
```http
GET /api/cnab/consultar?nomeLoja={nome}&dataInicio={iso}&dataFim={iso}

Response:
{
  "transacoes": [...],
  "resumoPorLoja": [...],
  "total": 10
}
```

---

## 🧪 Testando

### Arquivo de Teste Incluído
Use `exemplo-cnab.txt` para testar:

```
3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       
5201903010000013200556418150633123****7687145607MARIA JOSEFINALOJA DO Ã           
```

### Passo a Passo
1. Acesse `http://localhost:3000`
2. Clique em "Selecionar Arquivo"
3. Escolha `exemplo-cnab.txt`
4. Veja o preview automático
5. Confira o resumo por loja
6. Clique em "Enviar para o Servidor"

---

## 💡 Conceitos React Utilizados

### Hooks
```typescript
// useState para estado local
const [arquivo, setArquivo] = useState<File | null>(null);

// useEffect para inicialização
useEffect(() => {
  // Código executado na montagem
}, []);
```

### Props e TypeScript
```typescript
interface TransacaoCNAB {
  tipo: number;
  data: Date;
  valor: number;
  // ...
}
```

### React Router
```typescript
<Routes>
  <Route path="/" element={<Navigate to="/upload" />} />
  <Route path="/upload" element={<UploadCnab />} />
  <Route path="/consulta" element={<ConsultaCnab />} />
</Routes>
```

### Async/Await com Axios
```typescript
const response = await axios.post(url, formData);
```

---

## 🎨 Componentes Principais

### UploadCnab Component
```typescript
const UploadCnab: React.FC = () => {
  const [arquivo, setArquivo] = useState<File | null>(null);
  const [transacoes, setTransacoes] = useState<TransacaoCNAB[]>([]);
  
  const processarPreview = async (file: File) => {
    const resultado = await cnabService.processarArquivoLocal(file);
    setTransacoes(resultado);
  };
  
  return (
    // JSX do componente
  );
};
```

### ConsultaCnab Component
```typescript
const ConsultaCnab: React.FC = () => {
  const [nomeLoja, setNomeLoja] = useState('');
  const [dataInicio, setDataInicio] = useState('');
  
  const consultar = async (e: React.FormEvent) => {
    e.preventDefault();
    const response = await cnabService.consultarCnab(filtro);
    setResultados(response.transacoes);
  };
  
  return (
    // JSX do componente
  );
};
```

---

## 📊 Comparação Angular vs React

| Aspecto | Angular | React |
|---------|---------|-------|
| Linguagem | TypeScript (obrigatório) | JS ou TS (opcional) |
| Arquitetura | Framework completo | Biblioteca (+ libs) |
| Rotas | Angular Router | React Router |
| HTTP | HttpClient | Axios/Fetch |
| Forms | FormsModule | Hooks + state |
| Estado | Services | useState/Context |
| Styling | CSS Modules | CSS/Styled |

---

## 🔥 Diferenciais do React

### 1. Hooks Modernos
```typescript
// Mais conciso que Angular
const [estado, setEstado] = useState(valorInicial);
```

### 2. JSX Intuitivo
```typescript
// Lógica e UI juntos
return (
  <div>
    {loading ? <Spinner /> : <Content />}
  </div>
);
```

### 3. Componentes Funcionais
```typescript
// Sem classes, apenas functions
const Componente: React.FC = () => {
  // lógica
  return <div>UI</div>;
};
```

---

## 🛠️ Scripts Disponíveis

```bash
# Desenvolvimento
npm start              # Inicia em http://localhost:3000

# Build para produção
npm run build          # Cria pasta build/

# Testes
npm test               # Executa testes

# Ejetar configuração (cuidado!)
npm run eject          # Expõe webpack config
```

---

## 📦 Dependências Principais

```json
{
  "react": "^18.2.0",
  "react-dom": "^18.2.0",
  "react-router-dom": "^6.20.0",
  "axios": "^1.6.0",
  "typescript": "^4.9.5"
}
```

---

## 🚨 Troubleshooting

### Erro: "Module not found"
```bash
npm install
```

### Erro: "Port 3000 already in use"
```bash
# No terminal, mate o processo na porta 3000
# Linux/Mac:
lsof -ti:3000 | xargs kill -9

# Windows:
netstat -ano | findstr :3000
taskkill /PID <PID> /F
```

### Erro de TypeScript
```bash
# Reinstalar types
npm install --save-dev @types/react @types/react-dom
```

---

## 🎓 Aprendizados

### React vs Angular - Quando Usar?

**Use React quando:**
- ✅ Quer mais flexibilidade
- ✅ Prefere escolher suas libs
- ✅ Gosta de JSX
- ✅ Quer comunidade maior

**Use Angular quando:**
- ✅ Quer solução completa "out of the box"
- ✅ Prefere opiniões fortes (conventions)
- ✅ Trabalha em grandes empresas
- ✅ Quer TypeScript obrigatório

---

## 📚 Próximos Passos

1. **Estado Global**: Implementar Redux ou Context API
2. **Testes**: Adicionar testes com Jest e Testing Library
3. **Otimização**: Implementar lazy loading de rotas
4. **PWA**: Transformar em Progressive Web App
5. **Styled Components**: Migrar CSS para styled-components
6. **React Query**: Gerenciar cache de API calls

---

## ✅ Checklist do Aluno

- [ ] Entendi a estrutura de componentes React
- [ ] Sei usar useState e useEffect
- [ ] Entendi o fluxo de dados (props/state)
- [ ] Sei fazer requisições HTTP com Axios
- [ ] Entendi React Router
- [ ] Sei parsear arquivos CNAB
- [ ] Testei a aplicação localmente
- [ ] Comparei com a versão Angular

---

## 🎉 Parabéns!

Você agora domina:
✅ React com TypeScript
✅ Hooks (useState, useEffect)
✅ React Router v6
✅ Axios para HTTP
✅ FileReader API
✅ Parsing de arquivos CNAB
✅ Componentização
✅ Gerenciamento de estado

**Pronto para o mercado React! 💪**

---

**Dúvidas? Pergunte ao professor! 🎓**
