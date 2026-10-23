import { create } from 'zustand';
import { getUsers } from './users.api';
import { getConversation, sendMessage as apiSendMessage } from './messages.api';
import { getApiErrorMessage } from './utils';

export const useChatStore = create((set, get) => ({
  users: [],
  selectedUser: null,
  messages: [],
  isLoadingUsers: false,
  isLoadingMessages: false,
  isSending: false,
  error: null,

  /**
   * Charge la liste des autres utilisateurs
   */
  fetchUsers: async () => {
    set({ isLoadingUsers: true, error: null });
    try {
      const response = await getUsers();
      set({ users: response.data || [], isLoadingUsers: false });
    } catch (err) {
      const msg = getApiErrorMessage(err, 'Impossible de charger les utilisateurs.');
      set({ error: msg, isLoadingUsers: false });
    }
  },

  /**
   * Sélectionne un contact et charge la conversation
   * @param {{ id: number, username: string } | null} user 
   */
  selectUser: (user) => {
    const current = get().selectedUser;
    if (current?.id === user?.id) return;

    set({ selectedUser: user, messages: [], error: null });
    if (user && user.id) {
      get().fetchMessages(user.id, false);
    }
  },

  /**
   * Récupère les messages d'une conversation
   * @param {number} userId 
   * @param {boolean} isPolling true s'il s'agit d'un rafraîchissement d'arrière-plan sans spinner
   */
  fetchMessages: async (userId, isPolling = false) => {
    if (!userId) return;

    if (!isPolling) {
      set({ isLoadingMessages: true, error: null });
    }

    try {
      const response = await getConversation(userId);
      const newMessages = response.data || [];

      // Vérifie que l'utilisateur sélectionné n'a pas changé pendant la requête
      if (get().selectedUser?.id === userId) {
        set({
          messages: newMessages,
          isLoadingMessages: false,
        });
      }
    } catch (err) {
      if (!isPolling) {
        const msg = getApiErrorMessage(err, 'Impossible de charger la conversation.');
        set({ error: msg, isLoadingMessages: false });
      }
    }
  },

  /**
   * Envoie un message au contact sélectionné
   * @param {string} content 
   */
  sendMessage: async (content) => {
    const targetUser = get().selectedUser;
    if (!targetUser || !content.trim()) return false;

    set({ isSending: true, error: null });
    try {
      const response = await apiSendMessage({
        receiverId: targetUser.id,
        content: content.trim(),
      });

      const sentMsg = response.data;
      // On s'assure de ne pas ajouter de doublon si le rafraîchissement l'a déjà récupéré
      set((state) => {
        const exists = state.messages.some((m) => m.id === sentMsg.id);
        return {
          messages: exists ? state.messages : [...state.messages, sentMsg],
          isSending: false,
        };
      });

      return true;
    } catch (err) {
      const msg = getApiErrorMessage(err, "Échec de l'envoi du message.");
      set({ error: msg, isSending: false });
      throw new Error(msg);
    }
  },

  /**
   * Réinitialise l'état du store
   */
  reset: () => {
    set({
      users: [],
      selectedUser: null,
      messages: [],
      isLoadingUsers: false,
      isLoadingMessages: false,
      isSending: false,
      error: null,
    });
  },
}));
