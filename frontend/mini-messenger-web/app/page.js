'use client';

import { useEffect } from 'react';
import { useAuthStore } from '@/lib/authStore';
import { SpinnerIcon } from '@/components/common/Icons';

export default function HomePage() {
  const { user, ready, init } = useAuthStore();

  useEffect(() => {
    init();
  }, [init]);

  useEffect(() => {
    if (!ready) return;

    if (user) {
      window.location.href = '/chat';
    } else {
      window.location.href = '/login';
    }
  }, [ready, user]);

  // Sécurité : redirection de secours vers /login si le ready tarde
  useEffect(() => {
    const timer = setTimeout(() => {
      const token = typeof window !== 'undefined' ? localStorage.getItem('token') : null;
      if (token) {
        window.location.href = '/chat';
      } else {
        window.location.href = '/login';
      }
    }, 1000);

    return () => clearTimeout(timer);
  }, []);

  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-[#ffffff]">
      <div className="flex flex-col items-center space-y-4">
        <div className="w-12 h-12 rounded-2xl bg-[#fff7ed] text-[#f97316] flex items-center justify-center font-bold text-xl shadow-xs">
          M
        </div>
        <div className="flex items-center space-x-2 text-sm text-[#6b7280]">
          <SpinnerIcon className="w-4 h-4 animate-spin text-[#f97316]" />
          <span>Chargement de l'application...</span>
        </div>
      </div>
    </div>
  );
}
