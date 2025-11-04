// src/components/ConsultaCnab/ConsultaCnab.tsx

import React, { useState, useEffect } from 'react';
import cnabService from '../../services/cnabService';
import { TransacaoCNAB, ConsultaFiltro } from '../../types/cnab.types';
import './ConsultaCnab.css';

const ConsultaCnab: React.FC = () => {
  const [nomeLoja, setNomeLoja] = useState('');
  const [dataInicio, setDataInicio] = useState('');
  const [dataFim, setDataFim] = useState('');
  const [resultados, setResultados] = useState<TransacaoCNAB[]>([]);
  const [carregando, setCarregando] = useState(false);
  const [mensagemErro, setMensagemErro] = useState('');
  const [consultaRealizada, setConsultaRealizada] = useState(false);

  useEffect(() => {
    // Inicializar datas com valores padrão
    const agora = new Date();
    const dozeHorasAtras = new Date(agora.getTime() - (12 * 60 * 60 * 1000));
    
    setDataFim(formatarDataParaInput(agora));
    setDataInicio(formatarDataParaInput(dozeHorasAtras));
  }, []);

  const formatarDataParaInput = (data: Date): string => {
    const year = data.getFullYear();
    const month = String(data.getMonth() + 1).padStart(2, '0');
    const day = String(data.getDate()).padStart(2, '0');
    const hours = String(data.getHours()).padStart(2, '0');
    const minutes = String(data.getMinutes()).padStart(2, '0');
    
    return `${year}-${month}-${day}T${hours}:${minutes}`;
  };

  const calcularDiferencaHoras = (): number => {
    if (!dataInicio || !dataFim) return 0;
    
    const inicio = new Date(dataInicio);
    const fim = new Date(dataFim);
    const diffMs = fim.getTime() - inicio.getTime();
    return diffMs / (1000 * 60 * 60);
  };

  const getDiferencaHorasFormatada = (): string => {
    const horas = calcularDiferencaHoras();
    if (horas < 0) return 'Intervalo inválido';
    if (horas > 24) return `${horas.toFixed(1)}h (excede 24h)`;
    return `${horas.toFixed(1)}h`;
  };

  const isIntervaloValido = (): boolean => {
    const horas = calcularDiferencaHoras();
    return horas >= 0 && horas <= 24;
  };

  const consultar = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!nomeLoja.trim()) {
      setMensagemErro('Por favor, informe o nome da loja');
      return;
    }

    if (!dataInicio || !dataFim) {
      setMensagemErro('Por favor, informe as datas de início e fim');
      return;
    }

    const inicio = new Date(dataInicio);
    const fim = new Date(dataFim);

    const validacao = cnabService.validarIntervalo(inicio, fim);
    if (!validacao.valido) {
      setMensagemErro(validacao.mensagem || 'Intervalo de datas inválido');
      return;
    }

    setCarregando(true);
    setMensagemErro('');
    setConsultaRealizada(false);

    const filtro: ConsultaFiltro = {
      nomeLoja,
      dataInicio: inicio,
      dataFim: fim
    };

    try {
      const response = await cnabService.consultarCnab(filtro);
      setResultados(response.transacoes);
      setConsultaRealizada(true);
    } catch (error: any) {
      setMensagemErro(error.response?.data?.message || 'Erro ao consultar arquivos. Tente novamente.');
      setResultados([]);
    } finally {
      setCarregando(false);
    }
  };

  const limpar = () => {
    setNomeLoja('');
    const agora = new Date();
    const dozeHorasAtras = new Date(agora.getTime() - (12 * 60 * 60 * 1000));
    setDataFim(formatarDataParaInput(agora));
    setDataInicio(formatarDataParaInput(dozeHorasAtras));
    setResultados([]);
    setMensagemErro('');
    setConsultaRealizada(false);
  };

  const formatarData = (data: Date): string => {
    return new Date(data).toLocaleString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  const formatarValor = (valor: number): string => {
    return valor.toLocaleString('pt-BR', { 
      style: 'currency', 
      currency: 'BRL' 
    });
  };

  return (
    <div className="consulta-container">
      <div className="card">
        <div className="card-header">
          <h2>
            <span className="icon">🔍</span>
            Consulta de Arquivos CNAB
          </h2>
        </div>

        <div className="card-body">
          {mensagemErro && (
            <div className="alert alert-error">
              <span className="icon">⚠️</span>
              {mensagemErro}
            </div>
          )}

          <form onSubmit={consultar}>
            <div className="form-group">
              <label htmlFor="nomeLoja">Nome da Loja *</label>
              <input 
                type="text" 
                id="nomeLoja"
                value={nomeLoja}
                onChange={(e) => setNomeLoja(e.target.value)}
                className="form-control"
                placeholder="Ex: Loja Centro"
                required
                disabled={carregando}
              />
            </div>

            <div className="date-range-group">
              <div className="form-group">
                <label htmlFor="dataInicio">Data/Hora Início *</label>
                <input 
                  type="datetime-local" 
                  id="dataInicio"
                  value={dataInicio}
                  onChange={(e) => setDataInicio(e.target.value)}
                  className="form-control"
                  required
                  disabled={carregando}
                />
              </div>

              <div className="form-group">
                <label htmlFor="dataFim">Data/Hora Fim *</label>
                <input 
                  type="datetime-local" 
                  id="dataFim"
                  value={dataFim}
                  onChange={(e) => setDataFim(e.target.value)}
                  className="form-control"
                  required
                  disabled={carregando}
                />
              </div>
            </div>

            <div className={`interval-indicator ${isIntervaloValido() ? 'valid' : 'invalid'}`}>
              <span className="icon">{isIntervaloValido() ? '✓' : '⚠️'}</span>
              <span className="text">
                Intervalo: {getDiferencaHorasFormatada()}
                {!isIntervaloValido() && <small>(máximo permitido: 24 horas)</small>}
              </span>
            </div>

            <div className="form-actions">
              <button 
                type="submit" 
                className="btn btn-primary"
                disabled={!isIntervaloValido() || carregando}
              >
                {carregando ? (
                  <>
                    <span className="icon spinner">⏳</span>
                    Consultando...
                  </>
                ) : (
                  <>
                    <span className="icon">🔍</span>
                    Consultar
                  </>
                )}
              </button>

              <button 
                type="button" 
                className="btn btn-secondary"
                onClick={limpar}
                disabled={carregando}
              >
                <span className="icon">🔄</span>
                Limpar
              </button>
            </div>
          </form>

          {consultaRealizada && (
            <div className="resultados-section">
              <div className="resultados-header">
                <h3>Resultados da Consulta</h3>
                <span className="badge">{resultados.length} arquivo(s) encontrado(s)</span>
              </div>

              {resultados.length === 0 ? (
                <div className="empty-state">
                  <span className="icon-empty">📂</span>
                  <p>Nenhum arquivo encontrado para os filtros informados</p>
                  <small>Tente ajustar os critérios de busca</small>
                </div>
              ) : (
                <div className="table-container">
                  <table className="results-table">
                    <thead>
                      <tr>
                        <th>Tipo</th>
                        <th>Data</th>
                        <th>Valor</th>
                        <th>Loja</th>
                        <th>Dono</th>
                        <th>Natureza</th>
                      </tr>
                    </thead>
                    <tbody>
                      {resultados.map((arquivo, index) => {
                        const natureza = cnabService.getNaturezaTipo(arquivo.tipo);
                        return (
                          <tr key={index}>
                            <td>
                              <span className="tipo-badge">{arquivo.tipo}</span>
                              {cnabService.getDescricaoTipo(arquivo.tipo)}
                            </td>
                            <td>{formatarData(arquivo.data)}</td>
                            <td className="valor">{formatarValor(arquivo.valor)}</td>
                            <td>{arquivo.nomeLoja}</td>
                            <td>{arquivo.donoLoja}</td>
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
                </div>
              )}
            </div>
          )}

          <div className="info-box">
            <h4>Instruções:</h4>
            <ul>
              <li>Informe o nome da loja que deseja consultar</li>
              <li>Selecione a data/hora de início e fim</li>
              <li>O intervalo máximo permitido é de 24 horas</li>
              <li>Clique em "Consultar" para buscar os arquivos</li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ConsultaCnab;
