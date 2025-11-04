// src/services/cnabService.ts

import axios from 'axios';
import { TransacaoCNAB, CnabResponse, ConsultaFiltro, ResumoLoja, TIPOS_TRANSACAO } from '../types/cnab.types';

// Usa variável de ambiente ou fallback
// Em produção (Docker), usa '' para path relativo (nginx proxy)
// Em desenvolvimento, usa http://localhost:5000
const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000';
const API_URL = `${API_BASE_URL}/api`;

class CnabService {
  /**
   * Parse do arquivo CNAB conforme especificação do desafio
   * Layout de cada linha (80 caracteres):
   * - Tipo (1 posição)
   * - Data (8 posições - YYYYMMDD)
   * - Valor (10 posições - em centavos)
   * - CPF (11 posições)
   * - Cartão (12 posições)
   * - Hora (6 posições - HHMMSS)
   * - Dono da Loja (14 posições)
   * - Nome da Loja (19 posições)
   */
  parseCnabFile(fileContent: string): TransacaoCNAB[] {
    const transacoes: TransacaoCNAB[] = [];
    const linhas = fileContent.split('\n').filter(linha => linha.trim().length > 0);

    for (const linha of linhas) {
      if (linha.length < 80) {
        console.warn('Linha com tamanho inválido:', linha);
        continue;
      }

      try {
        const transacao: TransacaoCNAB = {
          tipo: parseInt(linha.substring(0, 1)),
          data: this.parseData(linha.substring(1, 9)),
          valor: this.parseValor(linha.substring(9, 19)),
          cpf: linha.substring(19, 30).trim(),
          cartao: linha.substring(30, 42).trim(),
          hora: this.formatarHora(linha.substring(42, 48)),
          donoLoja: linha.substring(48, 62).trim(),
          nomeLoja: linha.substring(62, 81).trim()
        };

        transacoes.push(transacao);
      } catch (error) {
        console.error('Erro ao parsear linha:', linha, error);
      }
    }

    return transacoes;
  }

  /**
   * Parse da data no formato YYYYMMDD
   */
  private parseData(dataStr: string): Date {
    const ano = parseInt(dataStr.substring(0, 4));
    const mes = parseInt(dataStr.substring(4, 6)) - 1; // Mês começa em 0
    const dia = parseInt(dataStr.substring(6, 8));
    return new Date(ano, mes, dia);
  }

  /**
   * Parse do valor (divide por 100 conforme especificação)
   */
  private parseValor(valorStr: string): number {
    const valorCentavos = parseInt(valorStr);
    return valorCentavos / 100;
  }

  /**
   * Formata hora no formato HH:MM:SS
   */
  private formatarHora(horaStr: string): string {
    return `${horaStr.substring(0, 2)}:${horaStr.substring(2, 4)}:${horaStr.substring(4, 6)}`;
  }

  /**
   * Upload do arquivo CNAB
   */
  async uploadCnab(file: File): Promise<any> {
    const formData = new FormData();
    formData.append('file', file); // API espera o campo "file"

    const response = await axios.post(`${API_URL}/Files`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });

    return response.data;
  }

  /**
   * Processa arquivo localmente (para preview)
   */
  processarArquivoLocal(file: File): Promise<TransacaoCNAB[]> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      
      reader.onload = (e: ProgressEvent<FileReader>) => {
        try {
          const conteudo = e.target?.result as string;
          const transacoes = this.parseCnabFile(conteudo);
          resolve(transacoes);
        } catch (error) {
          reject(error);
        }
      };

      reader.onerror = () => {
        reject(new Error('Erro ao ler arquivo'));
      };

      reader.readAsText(file);
    });
  }

  /**
   * Consulta transações CNAB com filtros
   */
  async consultarCnab(filtro: ConsultaFiltro): Promise<CnabResponse> {
    const response = await axios.get<CnabResponse>(
      `${API_URL}/Transactions/store/${encodeURIComponent(filtro.nomeLoja)}`,
      {
        params: {
          fromDate: filtro.dataInicio.toISOString(),
          toDate: filtro.dataFim.toISOString(),
        },
      }
    );

    return response.data;
  }

  /**
   * Calcula resumo por loja
   */
  calcularResumoPorLoja(transacoes: TransacaoCNAB[]): ResumoLoja[] {
    const resumoPorLoja = new Map<string, ResumoLoja>();

    for (const transacao of transacoes) {
      if (!resumoPorLoja.has(transacao.nomeLoja)) {
        resumoPorLoja.set(transacao.nomeLoja, {
          nomeLoja: transacao.nomeLoja,
          totalEntradas: 0,
          totalSaidas: 0,
          saldo: 0,
          quantidadeTransacoes: 0
        });
      }

      const resumo = resumoPorLoja.get(transacao.nomeLoja)!;
      const tipoInfo = TIPOS_TRANSACAO.find(t => t.tipo === transacao.tipo);

      if (tipoInfo) {
        if (tipoInfo.natureza === 'Entrada') {
          resumo.totalEntradas += transacao.valor;
          resumo.saldo += transacao.valor;
        } else {
          resumo.totalSaidas += transacao.valor;
          resumo.saldo -= transacao.valor;
        }
      }

      resumo.quantidadeTransacoes++;
    }

    return Array.from(resumoPorLoja.values());
  }

  /**
   * Obtém descrição do tipo de transação
   */
  getDescricaoTipo(tipo: number): string {
    const tipoInfo = TIPOS_TRANSACAO.find(t => t.tipo === tipo);
    return tipoInfo ? tipoInfo.descricao : 'Desconhecido';
  }

  /**
   * Obtém natureza da transação
   */
  getNaturezaTipo(tipo: number): 'Entrada' | 'Saída' {
    const tipoInfo = TIPOS_TRANSACAO.find(t => t.tipo === tipo);
    return tipoInfo ? tipoInfo.natureza : 'Entrada';
  }

  /**
   * Valida o intervalo de datas (máximo 24 horas)
   */
  validarIntervalo(dataInicio: Date, dataFim: Date): { valido: boolean; mensagem?: string } {
    const diffMs = dataFim.getTime() - dataInicio.getTime();
    const diffHoras = diffMs / (1000 * 60 * 60);

    if (diffHoras < 0) {
      return { valido: false, mensagem: 'Data final deve ser maior que data inicial' };
    }

    if (diffHoras > 24) {
      return { valido: false, mensagem: 'Intervalo máximo permitido é de 24 horas' };
    }

    return { valido: true };
  }

  /**
   * Valida formato do arquivo CNAB
   */
  validarFormatoCnab(fileContent: string): { valido: boolean; erros: string[] } {
    const erros: string[] = [];
    const linhas = fileContent.split('\n').filter(linha => linha.trim().length > 0);

    if (linhas.length === 0) {
      erros.push('Arquivo vazio');
      return { valido: false, erros };
    }

    linhas.forEach((linha, index) => {
      if (linha.length !== 80) {
        erros.push(`Linha ${index + 1}: Tamanho inválido (esperado 80 caracteres, encontrado ${linha.length})`);
      }

      const tipo = parseInt(linha.substring(0, 1));
      if (isNaN(tipo) || tipo < 1 || tipo > 9) {
        erros.push(`Linha ${index + 1}: Tipo de transação inválido (${linha.substring(0, 1)})`);
      }
    });

    return {
      valido: erros.length === 0,
      erros
    };
  }
}

export default new CnabService();
