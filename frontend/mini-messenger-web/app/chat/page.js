'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useAuthStore } from '@/lib/authStore';
import { useChatStore } from '@/lib/chatStore';
import ConversationList from '@/components/chat/ConversationList';
import MessageList from '@/components/chat/MessageList';
import MessageInput from '@/components/chat/MessageInput';
import { getInitials } from '@/lib/utils';
import { ArrowLeftIcon, MessageSquareIcon, SpinnerIcon } from '@/components/common/Icons';

export default function ChatPage() {
  const router = useRouter();
  const { user, ready, init } = useAuthStore();
  const {
    selectedUser,
    messages,
    isLoadingMessages,
    isSending,
    error,
    fetchUsers,
    fetchMessages,
    sendMessage,
    selectUser,
  } = useChatStore();

  // Contrôle de l'affichage sur mobile (soit liste, soit conversation)
  const [showConversationOnMobile, setShowConversationOnMobile] = useState(false);

  // 1. Initialisation de la session
  useEffect(() => {
    init();
  }, [init]);

  // 2. Protection de la route
  useEffect(() => {
    if (ready && !user) {
      router.replace('/login');
    }
  }, [ready, user, router]);

  // 3. Chargement de la liste des contacts au montage
  useEffect(() => {
    if (ready && user) {
      fetchUsers();
    }
  }, [ready, user, fetchUsers]);

  // 4. Polling provisoire toutes les 3.5 secondes pour la conversation active
  useEffect(() => {
    if (!selectedUser?.id) return;

    const intervalId = setInterval(() => {
      fetchMessages(selectedUser.id, true);
    }, 3500);

    return () => {
      clearInterval(intervalId);
    };
  }, [selectedUser?.id, fetchMessages]);

  const handleSelectContact = (u) => {
    setShowConversationOnMobile(true);
  };

  const handleBackToList = () => {
    setShowConversationOnMobile(false);
  };

  const handleSendMessage = async (content) => {
    return await sendMessage(content);
  };

  // Tant que la session n'est pas prête ou que l'utilisateur est redirigé
  if (!ready || !user) {
    return (
      <div className="min-h-screen flex flex-col items-center justify-center bg-white">
        <SpinnerIcon className="w-8 h-8 animate-spin text-[#f97316]" />
        <p className="mt-3 text-sm text-[#6b7280]">Vérification de la session...</p>
      </div>
    );
  }

  return (
    <div className="h-screen w-screen overflow-hidden flex bg-white text-[#1f2937]">
      {/* Colonne gauche : liste des conversations */}
      <div
        className={`w-full md:w-80 lg:w-96 h-full flex-col shrink-0 ${
          showConversationOnMobile ? 'hidden md:flex' : 'flex'
        }`}
      >
        <ConversationList onSelectUser={handleSelectContact} />
      </div>

      {/* Colonne droite : conversation active ou écran vide */}
      <div
        className={`flex-1 h-full flex-col bg-[#f9fafb] ${
          showConversationOnMobile ? 'flex' : 'hidden md:flex'
        }`}
      >
        {selectedUser ? (
          <>
            {/* En-tête de la conversation */}
            <div className="h-16 px-4 bg-white border-b border-[#e5e7eb] flex items-center justify-between shadow-2xs z-10">
              <div className="flex items-center space-x-3">
                {/* Bouton retour visible uniquement sur mobile */}
                <button
                  onClick={handleBackToList}
                  title="Retour aux contacts"
                  className="md:hidden p-2 -ml-2 text-[#6b7280] hover:text-[#1f2937] hover:bg-[#f3f4f6] rounded-lg transition-colors cursor-pointer"
                >
                  <ArrowLeftIcon className="w-5 h-5" />
                </button>

                <div className="relative">
                  <div className="w-10 h-10 rounded-full bg-[#f97316] text-white flex items-center justify-center font-bold text-sm shadow-xs">
                    {getInitials(selectedUser.username)}
                  </div>
                  <span className="absolute bottom-0 right-0 w-2.5 h-2.5 bg-emerald-500 border-2 border-white rounded-full" />
                </div>

                <div>
                  <h2 className="text-sm font-bold text-[#1f2937] leading-tight">
                    {selectedUser.username}
                  </h2>
                  <p className="text-[11px] text-[#6b7280]">En ligne</p>
                </div>
              </div>
            </div>

            {/* Corps : liste des messages */}
            <MessageList
              messages={messages}
              isLoading={isLoadingMessages}
              contactName={selectedUser.username}
            />

            {/* Pied : zone de saisie */}
            <MessageInput
              onSendMessage={handleSendMessage}
              isSending={isSending}
              error={error}
            />
          </>
        ) : (
          /* Aucun contact sélectionné (vue bureau par défaut) */
          <div className="flex-1 flex flex-col items-center justify-center p-8 text-center text-[#9ca3af]">
            <div className="w-16 h-16 rounded-3xl bg-[#fff7ed] text-[#f97316] flex items-center justify-center mb-4 shadow-xs">
              <MessageSquareIcon className="w-8 h-8" />
            </div>
            <h3 className="text-lg font-bold text-[#1f2937]">Vos messages</h3>
            <p className="text-sm text-[#6b7280] mt-1 max-w-sm">
              Sélectionnez un contact dans la liste à gauche pour ouvrir une conversation et échanger des messages.
            </p>
          </div>
        )}
      </div>
    </div>
  );
}
