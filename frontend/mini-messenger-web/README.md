# Mini-Messenger Web (Frontend Next.js)

Frontend de l'application **Mini Messenger**, réalisé avec **Next.js (App Router, JavaScript)**, **Tailwind CSS** et **Zustand**.

---

## 1. Périmètre réalisé

Conformément aux spécifications (`recapitulatif-mini-messenger.md`), seules les tâches **F2** et **F3** ont été réalisées. Le module Campagnes (F5) et le temps réel SignalR (F4) ne sont pas encore présents côté backend et sont strictement exclus du frontend.

### Tâche F2 : Authentification et protection des routes
- **Client HTTP (`lib/http.js`)** :
  - Instance Axios centralisée pointant sur `NEXT_PUBLIC_API_URL` (par défaut `https://localhost:7104`).
  - Intercepteur de requête : injection automatique du jeton Bearer (`Authorization: Bearer <token>`) depuis le `localStorage` lorsque le code s'exécute dans le navigateur.
  - Intercepteur de réponse : intercepte les retours `401 Unauthorized` sur les routes protégées pour effacer la session et rediriger automatiquement l'utilisateur vers `/login` (les routes de login/register sont exclues de cette redirection pour permettre l'affichage du message d'erreur d'identifiants).
- **Store d'authentification (`lib/authStore.js`)** :
  - Géré avec Zustand.
  - Propriétés : `user` (`{ userId, username }`), `token`, `ready`.
  - Actions : `init()` (synchronisation côté client avec le `localStorage`), `setAuth(data)`, `logout()`.
- **Formulaire et page de connexion (`app/login/page.js`)** :
  - Saisie nom d'utilisateur et mot de passe.
  - Validation client et désactivation du bouton pendant la requête.
  - Affichage direct du message d'erreur renvoyé par l'API (`Identifiants invalides.` ou indisponibilité du serveur).
  - Redirection vers `/chat` en cas de succès ou si déjà connecté.
- **Formulaire et page d'inscription (`app/register/page.js`)** :
  - Saisie du nom d'utilisateur (3 à 50 caractères), mot de passe (6 caractères minimum) et confirmation.
  - Validation stricte côté client avant tout envoi API.
  - Gestion et affichage des messages d'erreur API (`409` nom d'utilisateur déjà pris, `400` erreurs de validation de modèle).
  - Connexion automatique et redirection vers `/chat`.
- **Page d'accueil racine (`app/page.js`)** :
  - Redirection conditionnelle instantanée vers `/chat` si l'utilisateur possède une session valide, sinon vers `/login`.

### Tâche F3 : Écran de chat
- **Appels API (`lib/users.api.js`, `lib/messages.api.js`)** :
  - `getUsers()` : récupération de la liste des autres utilisateurs (`GET /api/users`).
  - `getConversation(userId)` : historique complet avec l'utilisateur (`GET /api/messages/{userId}`).
  - `sendMessage({ receiverId, content })` : envoi de message (`POST /api/messages`).
- **Store de chat (`lib/chatStore.js`)** :
  - Liste des contacts, utilisateur sélectionné, messages de la conversation active, statuts de chargement et d'envoi, gestion des erreurs.
- **Mise en page & Expérience utilisateur** :
  - **Interface à deux colonnes** :
    - *Colonne gauche* : en-tête avec profil connecté, bouton de déconnexion, champ de recherche de contact en temps réel, liste des contacts avec mise en valeur orange clair (`--primary-soft`) du contact sélectionné.
    - *Colonne droite* : en-tête du contact actif, historique de discussion, zone de saisie.
  - **Responsive mobile** : sur petit écran, alternance fluide entre la liste de contacts et la conversation grâce à un bouton retour "← Contacts".
  - **Bulles de messages (`MessageBubble.js`)** :
    - Messages envoyés : alignés à droite, fond orange vibrant (`#f97316`), texte blanc, heure discrète.
    - Messages reçus : alignés à gauche, fond gris clair très doux (`#f3f4f6`), texte foncé.
    - Défilement automatique vers le bas à l'ouverture d'une conversation et à chaque nouveau message envoyé ou reçu.
  - **Zone de saisie (`MessageInput.js`)** :
    - Envoi par clic sur le bouton ou par la touche **Entrée**.
    - Blocage du bouton d'envoi si le champ est vide ou pendant la requête.
    - Limite de 2000 caractères respectée.
  - **Rafraîchissement périodique (polling)** :
    - En l'absence provisoire de SignalR, la conversation active est rechargée automatiquement toutes les 3,5 secondes en arrière-plan sans scintillement ni réinitialisation de l'affichage.
    - L'intervalle est proprement nettoyé au changement de contact ou au démontage du composant.
  - **Gestion des états vides** :
    - Message dédié si aucun contact n'est inscrit.
    - Message dédié dans la conversation vide ("Aucun message pour l'instant, écrivez le premier").
    - Écran d'accueil neutre invitant à sélectionner une discussion.

---

## 2. Structure des fichiers

```
mini-messenger-web/
├── app/
│   ├── chat/
│   │   └── page.js           # Page principale du chat (protégée)
│   ├── login/
│   │   └── page.js           # Page de connexion
│   ├── register/
│   │   └── page.js           # Page d'inscription
│   ├── globals.css           # Thème CSS blanc & orange
│   ├── layout.js             # Layout racine avec métadonnées
│   └── page.js               # Redirection racine (/chat ou /login)
├── components/
│   ├── chat/
│   │   ├── ConversationList.js  # Liste des contacts + recherche + déconnexion
│   │   ├── MessageBubble.js     # Bulle individuelle (envoyée / reçue)
│   │   ├── MessageInput.js      # Zone de texte et bouton d'envoi
│   │   └── MessageList.js       # Historique et auto-scroll
│   └── common/
│       └── Icons.js             # Icônes SVG intégrées, épurées et légères
├── lib/
│   ├── auth.api.js           # Endpoints auth (login, register)
│   ├── authStore.js          # Store Zustand d'authentification
│   ├── chatStore.js          # Store Zustand du chat
│   ├── http.js               # Instance Axios + intercepteurs JWT & 401
│   ├── messages.api.js       # Endpoints messages (conversation, envoi)
│   ├── users.api.js          # Endpoints users (liste)
│   └── utils.js              # Parsing date UTC, formatage heure locale, erreurs API
├── .env                      # NEXT_PUBLIC_API_URL=https://localhost:7104
└── README.md
```

---

## 3. Choix de conception et hypothèses

1. **Charte graphique blanc et orange** :
   - Fond blanc pur (`#ffffff`), fond secondaire subtil (`#f9fafb`), bordures fines (`#e5e7eb`).
   - Orange principal (`#f97316`) et survol (`#ea580c`) strictement réservés aux éléments interactifs et dynamiques (boutons d'action, messages envoyés, badge actif).
   - Fond orange pastel doux (`#fff7ed`) pour le contact actif.
2. **Gestion des dates sans suffixe `Z`** :
   - Le backend ASP.NET Core renvoie les horodatages UTC au format ISO sans le suffixe `Z` (ex: `2026-10-23T09:12:45.123456`).
   - La fonction utilitaire `parseApiDate` ajoute automatiquement le `Z` manquant afin que le navigateur applique le bon décalage de fuseau horaire local.
3. **Format des erreurs de l'API** :
   - L'utilitaire `getApiErrorMessage` prend en compte les erreurs métier `{ "message": "..." }`, les erreurs de validation ASP.NET Core `{ "errors": { ... } }`, et les erreurs réseau afin de toujours afficher un texte clair et explicite en français à l'utilisateur.
4. **Composants d'icônes autonomes** :
   - Les icônes SVG ont été intégrées directement sous forme de composants React légers dans `components/common/Icons.js` afin d'éliminer toute dépendance externe lourde tout en garantissant un rendu net et réactif.

---

## 4. Recommandations d'évolution pour le backend

Lors du développement du frontend, les observations suivantes ont été relevées pour les futures itérations du backend :
- **Suffixe UTC `Z`** : Configurer le sérialiseur JSON de .NET (`System.Text.Json`) pour sérialiser systématiquement les objets `DateTime` avec le suffixe universel `Z` (`DateTimeKind.Utc`).
- **Pagination des messages** : Pour les conversations à fort volume, prévoir un paramètre de pagination par curseur ou par page (`GET /api/messages/{userId}?limit=50&beforeId=...`).
- **Statut de lecture** : Ajouter un champ `IsRead` / `ReadAt` pour afficher les indicateurs de lecture (double coche).
- **SignalR (Tâche F4)** : Dès que le hub SignalR `/hubs/chat` sera disponible sur le backend, il suffira de connecter le store Zustand pour recevoir les messages en temps réel et désactiver le polling d'arrière-plan.

---

## 5. Démarrage du projet

### 1. Démarrer le backend ASP.NET Core
Dans `backend/MiniMessenger.Api` :
```bash
dotnet run --launch-profile https
```
L'API écoute sur `https://localhost:7104`.

### 2. Démarrer le frontend Next.js
Dans `frontend/mini-messenger-web` :
```bash
npm run dev
```
Ouvrir votre navigateur à l'adresse [http://localhost:3000](http://localhost:3000).
