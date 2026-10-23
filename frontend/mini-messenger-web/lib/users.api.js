import api from './http';

/**
 * Récupère la liste des autres utilisateurs
 * @returns {Promise<import('axios').AxiosResponse<Array<{ id: number, username: string }>>>}
 */
export async function getUsers() {
  return await api.get('/api/users');
}
