import api from './http';

/**
 * Récupère l'historique de conversation avec un utilisateur
 * @param {number} userId 
 * @returns {Promise<import('axios').AxiosResponse<Array<{ id: number, content: string, sentAt: string, senderId: number, receiverId: number }>>>}
 */
export async function getConversation(userId) {
  return await api.get(`/api/messages/${userId}`);
}

/**
 * Envoie un message à un destinataire
 * @param {{ receiverId: number, content: string }} data 
 * @returns {Promise<import('axios').AxiosResponse<{ id: number, content: string, sentAt: string, senderId: number, receiverId: number }>>}
 */
export async function sendMessage(data) {
  return await api.post('/api/messages', data);
}
