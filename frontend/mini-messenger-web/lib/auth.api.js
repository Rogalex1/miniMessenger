import api from './http';

/**
 * Inscription d'un nouvel utilisateur
 * @param {{ username: string, password: string }} data 
 * @returns {Promise<import('axios').AxiosResponse<{ userId: number, username: string, token: string }>>}
 */
export async function register(data) {
  return await api.post('/api/auth/register', data);
}

/**
 * Connexion d'un utilisateur existant
 * @param {{ username: string, password: string }} data 
 * @returns {Promise<import('axios').AxiosResponse<{ userId: number, username: string, token: string }>>}
 */
export async function login(data) {
  return await api.post('/api/auth/login', data);
}
