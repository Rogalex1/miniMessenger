import "./globals.css";

export const metadata = {
  title: "Mini Messenger",
  description: "Application de messagerie instantanée épurée et moderne",
};

export default function RootLayout({ children }) {
  return (
    <html lang="fr" className="h-full">
      <body className="h-full antialiased bg-white text-[#1f2937]">
        {children}
      </body>
    </html>
  );
}
