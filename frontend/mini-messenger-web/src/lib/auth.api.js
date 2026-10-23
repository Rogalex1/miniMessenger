import http from './http'

export const register = (data) => http.post('/api/auth/register', data)
export const login = (data) => http.post('/api/auth/login', data)