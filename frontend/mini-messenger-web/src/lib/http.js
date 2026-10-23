import axios from 'axios'

const http = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL
})

// Ajoute automatiquement le jeton à chaque requête
http.interceptors.request.use((config) => {
  if (typeof window !== 'undefined') {
    const token = localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
  }
  return config
})

export default http