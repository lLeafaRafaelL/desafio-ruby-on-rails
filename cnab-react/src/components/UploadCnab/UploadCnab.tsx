// src/components/UploadCnab/UploadCnab.tsx

import React, { useState } from 'react';
import cnabService from '../../services/cnabService';
import { TransacaoCNAB } from '../../types/cnab.types';
import './UploadCnab.css';

const UploadCnab: React.FC = () => {
  const [arquivoSelecionado, setArquivoSelecionado] = useState<File | null>(null);
  const [uploading, setUploading] = useState(false);
  const [processando, setProcessando] = useState(false);
  const [mensagemSucesso, setMensagemSucesso] = useState('');
  const [mensagemErro, setMensagemErro] = useState('');
  const [transacoesPreview, setTransacoesPreview] = useState<TransacaoCNAB[]>([]);
  const [mostrarPreview, setMostrarPreview] = useState(false);

  const onFileSelected = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    
    if (file) {
      // Validação básica do arquivo
      if (!file.name.toLowerCase().endsWith('.txt') && !file.name.toLowerCase().endsWith('.cnab')) {
        setMensagemErro('Por favor, selecione um arquivo CNAB válido (.txt ou .cnab)');
        setArquivoSelecionado(null);
        limparPreview();
        return;
      }

      // Validação de tamanho (máximo 10MB)
      if (file.size > 10 * 1024 * 1024) {
        setMensagemErro('Arquivo muito grande. Tamanho máximo: 10MB');
        setArquivoSelecionado(null);
        limparPreview();
        return;
      }

      setArquivoSelecionado(file);
      setMensagemErro('');
      processarPreview(file);
    }
  };

  const processarPreview = async (file: File) => {
    setProcessando(true);
    limparPreview();

    try {
      const transacoes = await cnabService.processarArquivoLocal(file);
      setTransacoesPreview(transacoes);
      setMostrarPreview(true);

      if (transacoes.length === 0) {
        setMensagemErro('Arquivo não contém transações válidas');
      }
    } catch (error) {
      setMensagemErro('Erro ao processar arquivo: formato inválido');
      limparPreview();
    } finally {
      setProcessando(false);
    }
  };

  const limparPreview = () => {
    setTransacoesPreview([]);
    setMostrarPreview(false);
  };

  const limparFormulario = () => {
    setArquivoSelecionado(null);
    setMensagemSucesso('');
    setMensagemErro('');
    limparPreview();
    
    const fileInput = document.getElementById('fileInput') as HTMLInputElement;
    if (fileInput) {
      fileInput.value = '';
    }
  };

  const uploadArquivo = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!arquivoSelecionado) {
      setMensagemErro('Por favor, selecione um arquivo');
      return;
    }

    if (transacoesPreview.length === 0) {
      setMensagemErro('Arquivo não contém transações válidas');
      return;
    }

    setUploading(true);
    setMensagemErro('');
    setMensagemSucesso('');

    try {
      await cnabService.uploadCnab(arquivoSelecionado);
      setMensagemSucesso(`Arquivo enviado com sucesso! ${transacoesPreview.length} transação(ões) processada(s).`);
      
      setTimeout(() => {
        limparFormulario();
      }, 3000);
    } catch (error: any) {
      setMensagemErro(error.response?.data?.message || 'Erro ao enviar arquivo. Tente novamente.');
    } finally {
      setUploading(false);
    }
  };

  const formatarData = (data: Date): string => {
    return new Date(data).toLocaleDateString('pt-BR');
  };

  const formatarValor = (valor: number): string => {
    return valor.toLocaleString('pt-BR', { 
      style: 'currency', 
      currency: 'BRL' 
    });
  };

  const getTamanhoArquivo = (): string => {
    if (!arquivoSelecionado) return '';
    
    const bytes = arquivoSelecionado.size;
    if (bytes < 1024) return bytes + ' bytes';
    if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(2) + ' KB';
    return (bytes / (1024 * 1024)).toFixed(2) + ' MB';
  };

  const calcularValorTotal = (): number => {
    return transacoesPreview.reduce((total, t) => {
      const natureza = cnabService.getNaturezaTipo(t.tipo);
      return natureza === 'Entrada' ? total + t.valor : total - t.valor;
    }, 0);
  };

  const getResumoPorLoja = (): Map<string, number> => {
    const resumo = new Map<string, number>();
    
    for (const transacao of transacoesPreview) {
      const atual = resumo.get(transacao.nomeLoja) || 0;
      resumo.set(transacao.nomeLoja, atual + 1);
    }
    
    return resumo;
  };

  const valorTotal = calcularValorTotal();
  const resumoPorLoja = getResumoPorLoja();

  return (
    <div className="upload-container">
      <div className="card">
        <div className="card-header">
          <h2>
            <span className="icon">📤</span>
            Upload de Arquivo CNAB
          </h2>
          <p className="subtitle">Importe transações financeiras de várias lojas</p>
        </div>

        <div className="card-body">
          {/* Alertas */}
          {mensagemSucesso && (
            <div className="alert alert-success">
              <span className="icon">✓</span>
              {mensagemSucesso}
            </div>
          )}

          {mensagemErro && (
            <div className="alert alert-error">
              <span className="icon">⚠️</span>
              {mensagemErro}
            </div>
          )}

          {/* Formulário */}
          <form onSubmit={uploadArquivo}>
            <div className="form-group">
              <label htmlFor="fileInput">Arquivo CNAB *</label>
              <div className="file-input-wrapper">
                <input 
                  type="file" 
                  id="fileInput"
                  onChange={onFileSelected}
                  accept=".txt,.cnab"
                  className="file-input"
                  disabled={uploading || processando}
                />
                <button 
                  type="button" 
                  className="btn-select-file"
                  disabled={uploading || processando}
                  onClick={() => document.getElementById('fileInput')?.click()}
                >
                  {processando ? 'Processando...' : 'Selecionar Arquivo'}
                </button>
                <span className={`file-name ${!arquivoSelecionado ? 'placeholder' : ''}`}>
                  {arquivoSelecionado 
                    ? `${arquivoSelecionado.name} (${getTamanhoArquivo()})`
                    : 'Nenhum arquivo selecionado'
                  }
                </span>
              </div>
              <small className="help-text">
                Formato CNAB (80 caracteres por linha). Formatos aceitos: .txt, .cnab (Máx: 10MB)
              </small>
            </div>

            {/* Preview das Transações */}
            {mostrarPreview && transacoesPreview.length > 0 && (
              <div className="preview-section">
                <div className="preview-header">
                  <h3>Preview das Transações</h3>
                  <div className="preview-stats">
                    <span className="stat">
                      <strong>{transacoesPreview.length}</strong> transação(ões)
                    </span>
                    <span className={`stat ${valorTotal >= 0 ? 'positive' : 'negative'}`}>
                      Saldo: <strong>{formatarValor(valorTotal)}</strong>
                    </span>
                  </div>
                </div>

                {/* Resumo por Loja */}
                <div className="resumo-lojas">
                  <h4>Lojas no arquivo:</h4>
                  <div className="lojas-grid">
                    {Array.from(resumoPorLoja.entries()).map(([loja, count]) => (
                      <div key={loja} className="loja-card">
                        <span className="loja-nome">{loja}</span>
                        <span className="loja-count">{count} transação(ões)</span>
                      </div>
                    ))}
                  </div>
                </div>

                {/* Tabela de Transações (primeiras 10) */}
                <div className="transacoes-preview">
                  <table className="preview-table">
                    <thead>
                      <tr>
                        <th>Tipo</th>
                        <th>Data</th>
                        <th>Valor</th>
                        <th>Loja</th>
                        <th>Natureza</th>
                      </tr>
                    </thead>
                    <tbody>
                      {transacoesPreview.slice(0, 10).map((transacao, index) => {
                        const natureza = cnabService.getNaturezaTipo(transacao.tipo);
                        return (
                          <tr key={index}>
                            <td>
                              <span className="tipo-badge">{transacao.tipo}</span>
                              {cnabService.getDescricaoTipo(transacao.tipo)}
                            </td>
                            <td>{formatarData(transacao.data)}</td>
                            <td className="valor">{formatarValor(transacao.valor)}</td>
                            <td>{transacao.nomeLoja}</td>
                            <td>
                              <span className={`natureza-badge ${natureza.toLowerCase()}`}>
                                {natureza}
                              </span>
                            </td>
                          </tr>
                        );
                      })}
                    </tbody>
                  </table>
                  {transacoesPreview.length > 10 && (
                    <p className="mais-registros">
                      + {transacoesPreview.length - 10} transação(ões) adicional(is)
                    </p>
                  )}
                </div>
              </div>
            )}

            {/* Botões de Ação */}
            <div className="form-actions">
              <button 
                type="submit" 
                className="btn btn-primary"
                disabled={!arquivoSelecionado || uploading || processando || transacoesPreview.length === 0}
              >
                {uploading ? (
                  <>
                    <span className="icon spinner">⏳</span>
                    Enviando...
                  </>
                ) : (
                  <>
                    <span className="icon">📤</span>
                    Enviar para o Servidor
                  </>
                )}
              </button>

              <button 
                type="button" 
                className="btn btn-secondary"
                onClick={limparFormulario}
                disabled={uploading || processando}
              >
                <span className="icon">🔄</span>
                Limpar
              </button>
            </div>
          </form>

          {/* Informações Adicionais */}
          <div className="info-box">
            <h4>📋 Formato do Arquivo CNAB:</h4>
            <div className="layout-info">
              <p>Cada linha deve conter <strong>exatamente 80 caracteres</strong> com a seguinte estrutura:</p>
              <ul>
                <li><strong>Tipo</strong> (1 caractere): Tipo da transação (1-9)</li>
                <li><strong>Data</strong> (8 caracteres): YYYYMMDD</li>
                <li><strong>Valor</strong> (10 caracteres): Em centavos (será dividido por 100)</li>
                <li><strong>CPF</strong> (11 caracteres): CPF do cliente</li>
                <li><strong>Cartão</strong> (12 caracteres): Número do cartão</li>
                <li><strong>Hora</strong> (6 caracteres): HHMMSS</li>
                <li><strong>Dono</strong> (14 caracteres): Nome do dono da loja</li>
                <li><strong>Loja</strong> (19 caracteres): Nome da loja</li>
              </ul>
            </div>

            <h4>💡 Tipos de Transação:</h4>
            <div className="tipos-grid">
              <div className="tipo-item entrada">1 - Débito</div>
              <div className="tipo-item saida">2 - Boleto</div>
              <div className="tipo-item saida">3 - Financiamento</div>
              <div className="tipo-item entrada">4 - Crédito</div>
              <div className="tipo-item entrada">5 - Recebimento Empréstimo</div>
              <div className="tipo-item entrada">6 - Vendas</div>
              <div className="tipo-item entrada">7 - Recebimento TED</div>
              <div className="tipo-item entrada">8 - Recebimento DOC</div>
              <div className="tipo-item saida">9 - Aluguel</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default UploadCnab;
