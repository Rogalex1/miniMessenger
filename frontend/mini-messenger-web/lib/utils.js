/**
 * Parse une date sentAt UTC envoyée par l'API ASP.NET Core
 * Ajoute 'Z' si absent pour assurer la conversion correcte en heure locale.
 * @param {string} sentAt
 * @returns {Date}
 */
export function parseApiDate(sentAt) {
  if (!sentAt) return new Date();
  const iso = sentAt.endsWith('Z') ? sentAt : `${sentAt}Z`;
  return new Date(iso);
}

/**
 * Formate l'heure locale d'un message
 * @param {string} sentAt 
 * @returns {string} e.g. "14:05" ou "Hier 09:30"
 */
export function formatMessageTime(sentAt) {
  if (!sentAt) return '';
  const date = parseApiDate(sentAt);
  const now = new Date();

  const isToday =
    date.getDate() === now.getDate() &&
    date.getMonth() === now.getMonth() &&
    date.getFullYear() === now.getFullYear();

  const timeStr = date.toLocaleTimeString('fr-FR', {
    hour: '2-digit',
    minute: '2-digit',
  });

  if (isToday) {
    return timeStr;
  }

  const yesterday = new Date(now);
  yesterday.setDate(now.getDate() - 1);
  const isYesterday =
    date.getDate() === yesterday.getDate() &&
    date.getMonth() === yesterday.getMonth() &&
    date.getFullYear() === yesterday.getFullYear();

  if (isYesterday) {
    return `Hier, ${timeStr}`;
  }

  const dateStr = date.toLocaleDateString('fr-FR', {
    day: '2-digit',
    month: '2-digit',
  });
  return `${dateStr} à ${timeStr}`;
}

/**
 * Extrait un message d'erreur clair et lisible en français à partir de la réponse API
 * @param {any} error 
 * @param {string} defaultMessage 
 * @returns {string}
 */
export function getApiErrorMessage(error, defaultMessage = 'Une erreur est survenue.') {
  if (!error) return defaultMessage;

  // Erreur réseau / serveur éteint
  if (error.code === 'ERR_NETWORK' || error.message === 'Network Error') {
    return 'Impossible de contacter le serveur (https://localhost:7104). Vérifiez que le backend est démarré.';
  }

  const data = error.response?.data;
  if (!data) {
    return error.message || defaultMessage;
  }

  // Erreur personnalisée renvoyée avec { message: "..." }
  if (typeof data.message === 'string' && data.message.trim().length > 0) {
    return data.message;
  }

  // Erreurs de validation ASP.NET Core { errors: { Field: ["error"] } }
  if (data.errors && typeof data.errors === 'object') {
    const errorList = Object.values(data.errors).flat();
    if (errorList.length > 0) {
      return errorList.join(' ');
    }
  }

  if (typeof data.title === 'string' && data.title.trim().length > 0) {
    return data.title;
  }

  return defaultMessage;
}

/**
 * Génère des initiales à partir du nom d'utilisateur
 * @param {string} username 
 * @returns {string}
 */
export function getInitials(username) {
  if (!username) return '?';
  return username.slice(0, 2).toUpperCase();
}
