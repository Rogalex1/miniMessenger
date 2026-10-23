'use client';

import { useState, useRef } from 'react';
import { SendIcon, SpinnerIcon, AlertCircleIcon } from '@/components/common/Icons';

export default function MessageInput({ onSendMessage, isSending, error }) {
  const [content, setContent] = useState('');
  const [localError, setLocalError] = useState('');
  const inputRef = useRef(null);

  const handleSubmit = async (e) => {
    e?.preventDefault();
    const trimmed = content.trim();

    if (!trimmed) return;

    if (trimmed.length > 2000) {
      setLocalError('Le message ne peut pas dépasser 2000 caractères.');
      return;
    }

    setLocalError('');
    try {
      await onSendMessage(trimmed);
      setContent('');
      setTimeout(() => {
        inputRef.current?.focus();
      }, 50);
    } catch {
      // L'erreur est gérée via le prop error ou affichée ici
    }
  };

  const handleKeyDown = (e) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      handleSubmit();
    }
  };

  const displayedError = localError || error;

  return (
    <div className="p-3 border-t border-[#e5e7eb] bg-white">
      {displayedError && (
        <div className="mb-2 px-3 py-2 bg-red-50 border border-red-200 rounded-lg text-xs text-[#dc2626] flex items-center space-x-2">
          <AlertCircleIcon className="w-4 h-4 shrink-0" />
          <span>{displayedError}</span>
        </div>
      )}

      <form onSubmit={handleSubmit} className="flex items-center space-x-2">
        <input
          ref={inputRef}
          type="text"
          value={content}
          onChange={(e) => {
            setContent(e.target.value);
            if (localError) setLocalError('');
          }}
          onKeyDown={handleKeyDown}
          placeholder="Écrivez un message... (Entrée pour envoyer)"
          disabled={isSending}
          maxLength={2000}
          className="flex-1 px-4 py-2.5 text-sm bg-[#f9fafb] border border-[#e5e7eb] rounded-xl focus:outline-none focus:border-[#f97316] focus:ring-1 focus:ring-[#f97316] disabled:opacity-50 transition-all placeholder:text-[#9ca3af]"
        />

        <button
          type="submit"
          disabled={isSending || !content.trim()}
          title="Envoyer le message"
          className="px-4 py-2.5 bg-[#f97316] hover:bg-[#ea580c] disabled:bg-[#fdba74] text-white font-semibold rounded-xl text-sm flex items-center space-x-1.5 transition-colors cursor-pointer disabled:cursor-not-allowed shadow-xs"
        >
          {isSending ? (
            <SpinnerIcon className="w-4 h-4 animate-spin" />
          ) : (
            <SendIcon className="w-4 h-4" />
          )}
          <span className="hidden sm:inline">Envoyer</span>
        </button>
      </form>
    </div>
  );
}
