import { create } from 'zustand';

export const useAuthStore = create((set, get) => ({
  user: null,
  token: null,
  ready: false,

  /**
   * Initialise la session depuis le localStorage
   */
  init: () => {
    if (typeof window === 'undefined') return;

    try {
      const token = localStorage.getItem('token');
      const userJson = localStorage.getItem('user');
      const user = userJson ? JSON.parse(userJson) : null;

      if (token && user) {
        set({ user, token, ready: true });
      } else {
        set({ user: null, token: null, ready: true });
      }
    } catch {
      set({ user: null, token: null, ready: true });
    }
  },

  /**
   * Enregistre la session après login ou register
   * Supporte userId ou id
   */
  setAuth: (data) => {
    if (!data) return;
    const userId = data.userId ?? data.id;
    const username = data.username;
    const token = data.token;
    const userData = { userId, username };

    if (typeof window !== 'undefined') {
      if (token) localStorage.setItem('token', token);
      localStorage.setItem('user', JSON.stringify(userData));
    }
    set({ user: userData, token, ready: true });
  },

  /**
   * Efface la session et déconnecte l'utilisateur
   */
  logout: () => {
    if (typeof window !== 'undefined') {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      window.location.href = '/login';
    }
    set({ user: null, token: null, ready: true });
  },
}));
