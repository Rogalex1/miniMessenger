'use client';

import { useEffect, useRef } from 'react';
import MessageBubble from './MessageBubble';
import { useAuthStore } from '@/lib/authStore';
import { MessageSquareIcon, SpinnerIcon } from '@/components/common/Icons';

export default function MessageList({ messages, isLoading, contactName }) {
  const { user } = useAuthStore();
  const bottomRef = useRef(null);

  // Défilement automatique vers le bas au chargement et à l'arrivée de nouveaux messages
  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages]);

  if (isLoading) {
    return (
      <div className="flex-1 flex flex-col items-center justify-center p-8 text-[#9ca3af] space-y-3">
        <SpinnerIcon className="w-8 h-8 animate-spin text-[#f97316]" />
        <p className="text-sm font-medium">Chargement de la conversation...</p>
      </div>
    );
  }

  if (messages.length === 0) {
    return (
      <div className="flex-1 flex flex-col items-center justify-center p-8 text-center text-[#9ca3af]">
        <div className="w-14 h-14 rounded-full bg-[#fff7ed] flex items-center justify-center text-[#f97316] mb-3">
          <MessageSquareIcon className="w-7 h-7" />
        </div>
        <p className="text-base font-semibold text-[#374151]">Aucun message pour l'instant</p>
        <p className="text-sm text-[#6b7280] mt-1 max-w-xs">
          Envoyez un premier message à <span className="font-medium text-[#f97316]">{contactName}</span> pour démarrer la discussion.
        </p>
      </div>
    );
  }

  return (
    <div className="flex-1 overflow-y-auto p-4 space-y-1">
      {messages.map((msg) => {
        const isOwn = msg.senderId === user?.userId;
        return <MessageBubble key={msg.id} message={msg} isOwn={isOwn} />;
      })}
      <div ref={bottomRef} className="h-1" />
    </div>
  );
}
