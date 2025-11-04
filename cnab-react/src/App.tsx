// src/App.tsx

import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link, Navigate } from 'react-router-dom';
import UploadCnab from './components/UploadCnab/UploadCnab';
import ConsultaCnab from './components/ConsultaCnab/ConsultaCnab';
import './App.css';

const App: React.FC = () => {
  return (
    <Router>
      <div className="app-container">
        {/* Navbar */}
        <nav className="navbar">
          <div className="nav-content">
            <div className="nav-brand">
              <span className="logo">💼</span>
              <h1>Sistema CNAB</h1>
            </div>
            
            <ul className="nav-menu">
              <li>
                <Link to="/upload" className="nav-link">
                  <span className="nav-icon">📤</span>
                  Upload
                </Link>
              </li>
              <li>
                <Link to="/consulta" className="nav-link">
                  <span className="nav-icon">🔍</span>
                  Consulta
                </Link>
              </li>
            </ul>
          </div>
        </nav>

        {/* Conteúdo Principal */}
        <main className="main-content">
          <Routes>
            <Route path="/" element={<Navigate to="/upload" replace />} />
            <Route path="/upload" element={<UploadCnab />} />
            <Route path="/consulta" element={<ConsultaCnab />} />
          </Routes>
        </main>

        {/* Footer */}
        <footer className="footer">
          <p>&copy; 2024 Sistema CNAB - Gerenciamento de Arquivos</p>
        </footer>
      </div>
    </Router>
  );
};

export default App;
