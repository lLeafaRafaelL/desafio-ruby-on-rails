// src/setupProxy.js
// Este arquivo configura um proxy para desenvolvimento local
// Evita problemas de CORS durante o desenvolvimento

const { createProxyMiddleware } = require('http-proxy-middleware');

module.exports = function(app) {
  // Proxy para a API
  app.use(
    '/api',
    createProxyMiddleware({
      target: process.env.REACT_APP_API_URL || 'http://localhost:5000',
      changeOrigin: true,
      secure: false,
      logLevel: 'debug',
      onProxyReq: (proxyReq, req, res) => {
        console.log(`[Proxy] ${req.method} ${req.path} -> ${proxyReq.path}`);
      },
      onError: (err, req, res) => {
        console.error('[Proxy Error]', err);
        res.status(500).json({ 
          error: 'Proxy Error', 
          message: err.message 
        });
      }
    })
  );
};
