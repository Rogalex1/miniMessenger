'use client';

import { useState, useEffect } from 'react';
import Link from 'next/link';
import { register } from '@/lib/auth.api';
import { useAuthStore } from '@/lib/authStore';
import { getApiErrorMessage } from '@/lib/utils';
import { AlertCircleIcon, MessageSquareIcon, SpinnerIcon } from '@/components/common/Icons';

export default function RegisterPage() {
  const { user, ready, init, setAuth } = useAuthStore();

  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');

  // Initialisation de la session au montage
  useEffect(() => {
    init();
  }, [init]);

  // Si l'utilisateur est déjà connecté, redirection vers /chat
  useEffect(() => {
    if (ready && user) {
      window.location.href = '/chat';
    }
  }, [ready, user]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setErrorMessage('');

    const trimmedUsername = username.trim();

    // Validation côté client (critères d'acceptation de la spécification F2)
    if (!trimmedUsername || !password || !confirmPassword) {
      setErrorMessage('Tous les champs sont obligatoires.');
      return;
    }

    if (trimmedUsername.length < 3 || trimmedUsername.length > 50) {
      setErrorMessage("Le nom d'utilisateur doit contenir entre 3 et 50 caractères.");
      return;
    }

    if (password.length < 6) {
      setErrorMessage('Le mot de passe doit comporter au moins 6 caractères.');
      return;
    }

    if (password !== confirmPassword) {
      setErrorMessage('Les deux mots de passe ne correspondent pas.');
      return;
    }

    setIsSubmitting(true);
    try {
      const response = await register({
        username: trimmedUsername,
        password: password,
      });

      // Succès : enregistrer la session puis redirection vers /chat
      setAuth(response.data);
      window.location.href = '/chat';
    } catch (err) {
      const msg = getApiErrorMessage(
        err,
        "Erreur lors de l'inscription. Veuillez réessayer."
      );
      setErrorMessage(msg);
      setIsSubmitting(false);
    }
  };

  // Si déjà connecté et en cours de redirection
  if (ready && user) {
    return (
      <div className="min-h-screen flex flex-col items-center justify-center bg-[#ffffff]">
        <SpinnerIcon className="w-8 h-8 animate-spin text-[#f97316]" />
        <p className="mt-3 text-sm text-[#6b7280]">Redirection vers le chat...</p>
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
          <h1 className="text-2xl font-bold text-[#1f2937]">Inscription</h1>
          <p className="text-sm text-[#6b7280] mt-1 text-center">
            Créez votre compte pour commencer à chatter
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
              Nom d'utilisateur <span className="text-[#9ca3af] font-normal lowercase">(3 à 50 caractères)</span>
            </label>
            <input
              type="text"
              autoFocus
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              placeholder="ex: bob"
              disabled={isSubmitting}
              className="w-full px-3.5 py-2.5 text-sm bg-white border border-[#e5e7eb] rounded-xl focus:outline-none focus:border-[#f97316] focus:ring-1 focus:ring-[#f97316] transition-all disabled:opacity-50"
            />
          </div>

          <div>
            <label className="block text-xs font-semibold text-[#4b5563] uppercase tracking-wider mb-1.5">
              Mot de passe <span className="text-[#9ca3af] font-normal lowercase">(6 caractères min.)</span>
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

          <div>
            <label className="block text-xs font-semibold text-[#4b5563] uppercase tracking-wider mb-1.5">
              Confirmer le mot de passe
            </label>
            <input
              type="password"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
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
                <span>Création du compte...</span>
              </>
            ) : (
              <span>Créer mon compte</span>
            )}
          </button>
        </form>

        {/* Lien vers connexion */}
        <div className="mt-8 pt-6 border-t border-[#f3f4f6] text-center text-sm text-[#6b7280]">
          Vous possédez déjà un compte ?{' '}
          <Link
            href="/login"
            className="font-semibold text-[#c2410c] hover:text-[#ea580c] underline transition-colors"
          >
            Se connecter
          </Link>
        </div>
      </div>
    </div>
  );
}
