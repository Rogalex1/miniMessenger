'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import Link from 'next/link';
import { login } from '@/lib/auth.api';
import { useAuthStore } from '@/lib/authStore';
import { getApiErrorMessage } from '@/lib/utils';
import { AlertCircleIcon, MessageSquareIcon, SpinnerIcon } from '@/components/common/Icons';

export default function LoginPage() {
  const router = useRouter();
  const { user, ready, init, setAuth } = useAuthStore();

  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');

  // Initialisation de la session
  useEffect(() => {
    init();
  }, [init]);

  // Si l'utilisateur est déjà connecté, redirection vers /chat
  useEffect(() => {
    if (ready && user) {
      router.replace('/chat');
    }
  }, [ready, user, router]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setErrorMessage('');

    const trimmedUsername = username.trim();
    if (!trimmedUsername || !password) {
      setErrorMessage('Veuillez renseigner votre nom d\'utilisateur et votre mot de passe.');
      return;
    }

    setIsSubmitting(true);
    try {
      const response = await login({
        username: trimmedUsername,
        password: password,
      });

      // Succès : mise à jour de la session et redirection
      setAuth(response.data);
      router.push('/chat');
    } catch (err) {
      const msg = getApiErrorMessage(err, 'Identifiants invalides.');
      setErrorMessage(msg);
      setIsSubmitting(false);
    }
  };

  // Tant que l'état auth n'a pas été lu du localStorage, on évite un clignotement
  if (!ready || user) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-[#ffffff]">
        <SpinnerIcon className="w-6 h-6 animate-spin text-[#f97316]" />
      </div>
    );
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-[#f9fafb] px-4 py-12">
      <div className="w-full max-w-md bg-white rounded-2xl border border-[#e5e7eb] p-8 shadow-xs">
        {/* En-tête */}
        <div className="flex flex-col items-center mb-8">
          <div className="w-12 h-12 rounded-2xl bg-[#fff7ed] text-[#f97316] flex items-center justify-center mb-3">
            <MessageSquareIcon className="w-6 h-6" />
          </div>
          <h1 className="text-2xl font-bold text-[#1f2937]">Connexion</h1>
          <p className="text-sm text-[#6b7280] mt-1 text-center">
            Accédez à vos conversations instantanées
          </p>
        </div>

        {/* Message d'erreur */}
        {errorMessage && (
          <div className="mb-6 p-3.5 bg-red-50 border border-red-200 rounded-xl text-sm text-[#dc2626] flex items-start space-x-2.5">
            <AlertCircleIcon className="w-5 h-5 shrink-0 mt-0.5 text-[#dc2626]" />
            <div className="leading-snug">{errorMessage}</div>
          </div>
        )}

        {/* Formulaire */}
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-xs font-semibold text-[#4b5563] uppercase tracking-wider mb-1.5">
              Nom d'utilisateur
            </label>
            <input
              type="text"
              autoFocus
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              placeholder="ex: alex"
              disabled={isSubmitting}
              className="w-full px-3.5 py-2.5 text-sm bg-white border border-[#e5e7eb] rounded-xl focus:outline-none focus:border-[#f97316] focus:ring-1 focus:ring-[#f97316] transition-all disabled:opacity-50"
            />
          </div>

          <div>
            <label className="block text-xs font-semibold text-[#4b5563] uppercase tracking-wider mb-1.5">
              Mot de passe
            </label>
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="••••••••"
              disabled={isSubmitting}
              className="w-full px-3.5 py-2.5 text-sm bg-white border border-[#e5e7eb] rounded-xl focus:outline-none focus:border-[#f97316] focus:ring-1 focus:ring-[#f97316] transition-all disabled:opacity-50"
            />
          </div>

          <button
            type="submit"
            disabled={isSubmitting}
            className="w-full mt-2 py-3 bg-[#f97316] hover:bg-[#ea580c] disabled:bg-[#fdba74] text-white font-semibold rounded-xl text-sm flex items-center justify-center space-x-2 transition-colors cursor-pointer disabled:cursor-not-allowed shadow-xs"
          >
            {isSubmitting ? (
              <>
                <SpinnerIcon className="w-4 h-4 animate-spin" />
                <span>Connexion en cours...</span>
              </>
            ) : (
              <span>Se connecter</span>
            )}
          </button>
        </form>

        {/* Lien vers inscription */}
        <div className="mt-8 pt-6 border-t border-[#f3f4f6] text-center text-sm text-[#6b7280]">
          Pas encore de compte ?{' '}
          <Link
            href="/register"
            className="font-semibold text-[#c2410c] hover:text-[#ea580c] underline transition-colors"
          >
            Créer un compte
          </Link>
        </div>
      </div>
    </div>
  );
}
