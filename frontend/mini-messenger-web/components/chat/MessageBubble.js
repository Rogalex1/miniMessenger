import { formatMessageTime } from '@/lib/utils';

export default function MessageBubble({ message, isOwn }) {
  const timeFormatted = formatMessageTime(message.sentAt);

  return (
    <div className={`flex w-full ${isOwn ? 'justify-end' : 'justify-start'} my-1.5 px-1`}>
      <div
        className={`max-w-[78%] md:max-w-[65%] rounded-2xl px-4 py-2.5 shadow-xs transition-all ${
          isOwn
            ? 'bg-[#f97316] text-white rounded-br-xs'
            : 'bg-[#f3f4f6] text-[#1f2937] border border-[#e5e7eb] rounded-bl-xs'
        }`}
      >
        <p className="text-sm leading-relaxed whitespace-pre-wrap break-words">
          {message.content}
        </p>
        <div
          className={`text-[11px] mt-1 text-right select-none ${
            isOwn ? 'text-orange-100' : 'text-[#9ca3af]'
          }`}
        >
          {timeFormatted}
        </div>
      </div>
    </div>
  );
}
