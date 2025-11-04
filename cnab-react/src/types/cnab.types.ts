// src/types/cnab.types.ts

export interface TipoTransacao {
  tipo: number;
  descricao: string;
  natureza: 'Entrada' | 'Saída';
  sinal: '+' | '-';
}

export interface TransacaoCNAB {
  tipo: number;
  data: Date;
  valor: number; // Valor já normalizado (dividido por 100)
  cpf: string;
  cartao: string;
  hora: string;
  donoLoja: string;
  nomeLoja: string;
}

export interface ResumoLoja {
  nomeLoja: string;
  totalEntradas: number;
  totalSaidas: number;
  saldo: number;
  quantidadeTransacoes: number;
}

export interface ConsultaFiltro {
  nomeLoja: string;
  dataInicio: Date;
  dataFim: Date;
}

export interface CnabResponse {
  transacoes: TransacaoCNAB[];
  resumoPorLoja: ResumoLoja[];
  total: number;
}

// Tabela de tipos de transação conforme especificação do desafio
export const TIPOS_TRANSACAO: TipoTransacao[] = [
  { tipo: 1, descricao: 'Débito', natureza: 'Entrada', sinal: '+' },
  { tipo: 2, descricao: 'Boleto', natureza: 'Saída', sinal: '-' },
  { tipo: 3, descricao: 'Financiamento', natureza: 'Saída', sinal: '-' },
  { tipo: 4, descricao: 'Crédito', natureza: 'Entrada', sinal: '+' },
  { tipo: 5, descricao: 'Recebimento Empréstimo', natureza: 'Entrada', sinal: '+' },
  { tipo: 6, descricao: 'Vendas', natureza: 'Entrada', sinal: '+' },
  { tipo: 7, descricao: 'Recebimento TED', natureza: 'Entrada', sinal: '+' },
  { tipo: 8, descricao: 'Recebimento DOC', natureza: 'Entrada', sinal: '+' },
  { tipo: 9, descricao: 'Aluguel', natureza: 'Saída', sinal: '-' }
];
