'use client';

import { useState } from 'react';
import { useAuthStore } from '@/lib/authStore';
import { useChatStore } from '@/lib/chatStore';
import { getInitials } from '@/lib/utils';
import { AlertCircleIcon, LogOutIcon, SearchIcon, SpinnerIcon, UserIcon } from '@/components/common/Icons';

export default function ConversationList({ onSelectUser }) {
  const { user, logout } = useAuthStore();
  const { users, selectedUser, selectUser, isLoadingUsers, error, fetchUsers } = useChatStore();
  const [searchTerm, setSearchTerm] = useState('');

  const filteredUsers = users.filter((u) =>
    u.username.toLowerCase().includes(searchTerm.toLowerCase().trim())
  );

  const handleUserClick = (u) => {
    selectUser(u);
    if (onSelectUser) {
      onSelectUser(u);
    }
  };

  return (
    <div className="flex flex-col h-full bg-white border-r border-[#e5e7eb] select-none">
      {/* En-tête utilisateur connecté */}
      <div className="p-4 border-b border-[#e5e7eb] bg-[#f9fafb] flex items-center justify-between">
        <div className="flex items-center space-x-3 overflow-hidden">
          <div className="w-10 h-10 rounded-full bg-[#f97316] text-white flex items-center justify-center font-bold text-sm shrink-0 shadow-xs">
            {getInitials(user?.username)}
          </div>
          <div className="overflow-hidden">
            <p className="text-xs font-semibold text-[#6b7280] uppercase tracking-wider">Connecté en tant que</p>
            <p className="text-sm font-bold text-[#1f2937] truncate" title={user?.username}>
              {user?.username || 'Utilisateur'}
            </p>
          </div>
        </div>

        <button
          onClick={logout}
          title="Se déconnecter"
          className="p-2 text-[#6b7280] hover:text-[#dc2626] hover:bg-red-50 rounded-lg transition-colors cursor-pointer shrink-0"
        >
          <LogOutIcon className="w-5 h-5" />
        </button>
      </div>

      {/* Barre de recherche */}
      <div className="p-3 border-b border-[#e5e7eb]">
        <div className="relative">
          <SearchIcon className="w-4 h-4 text-[#9ca3af] absolute left-3 top-1/2 -translate-y-1/2 pointer-events-none" />
          <input
            type="text"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            placeholder="Rechercher un contact..."
            className="w-full pl-9 pr-3 py-2 text-sm bg-[#f9fafb] border border-[#e5e7eb] rounded-lg focus:outline-none focus:border-[#f97316] focus:ring-1 focus:ring-[#f97316] transition-all"
          />
        </div>
      </div>

      {/* Liste des contacts */}
      <div className="flex-1 overflow-y-auto divide-y divide-[#f3f4f6]">
        {isLoadingUsers && users.length === 0 ? (
          <div className="flex flex-col items-center justify-center py-12 text-[#9ca3af] space-y-2">
            <SpinnerIcon className="w-6 h-6 animate-spin text-[#f97316]" />
            <p className="text-sm">Chargement des contacts...</p>
          </div>
        ) : error && users.length === 0 ? (
          <div className="py-8 px-4 text-center text-[#dc2626] space-y-2">
            <AlertCircleIcon className="w-6 h-6 mx-auto text-[#dc2626]" />
            <p className="text-xs font-medium">{error}</p>
            <button
              onClick={() => fetchUsers()}
              className="mt-2 px-3 py-1.5 text-xs bg-[#f97316] hover:bg-[#ea580c] text-white font-medium rounded-lg transition-colors cursor-pointer"
            >
              Réessayer
            </button>
          </div>
        ) : filteredUsers.length === 0 ? (
          <div className="py-12 px-4 text-center text-[#9ca3af]">
            <UserIcon className="w-8 h-8 mx-auto mb-2 opacity-50" />
            <p className="text-sm font-medium">
              {searchTerm ? 'Aucun contact trouvé' : 'Aucun autre utilisateur'}
            </p>
            <p className="text-xs text-[#9ca3af] mt-1">
              {searchTerm ? 'Essayez un autre nom.' : 'Créez un deuxième compte pour tester.'}
            </p>
          </div>
        ) : (
          filteredUsers.map((u) => {
            const isSelected = selectedUser?.id === u.id;
            return (
              <button
                key={u.id}
                onClick={() => handleUserClick(u)}
                className={`w-full text-left px-4 py-3 flex items-center space-x-3 transition-colors cursor-pointer ${
                  isSelected
                    ? 'bg-[#fff7ed] border-l-4 border-[#f97316]'
                    : 'hover:bg-[#f9fafb]'
                }`}
              >
                <div
                  className={`w-10 h-10 rounded-full flex items-center justify-center font-bold text-sm shrink-0 transition-colors ${
                    isSelected
                      ? 'bg-[#f97316] text-white'
                      : 'bg-[#ffedd5] text-[#c2410c]'
                  }`}
                >
                  {getInitials(u.username)}
                </div>
                <div className="flex-1 min-w-0">
                  <p
                    className={`text-sm truncate ${
                      isSelected
                        ? 'font-bold text-[#c2410c]'
                        : 'font-semibold text-[#1f2937]'
                    }`}
                  >
                    {u.username}
                  </p>
                  <p className="text-xs text-[#9ca3af] truncate">
                    Cliquez pour discuter
                  </p>
                </div>
              </button>
            );
          })
        )}
      </div>
    </div>
  );
}
