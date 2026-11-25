import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'node:path'

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: { '@': path.resolve(import.meta.dirname, './src') },
  },
  server: {
    port: 5301,
    strictPort: true,
    proxy: {
      '/api': { target: 'http://localhost:5300', changeOrigin: true },
    },
  },
})
