import { MessagingInterface } from '@/components/message/Groupchat-Messages';

export default async function GroupchatViewPage({
  params,
}: {
  params: { groupchat_id: string };
}) {
  const { groupchat_id } = await params;

  return (
    <div className="flex min-h-screen flex-col items-center justify-center bg-background">
      <MessagingInterface groupchatId={groupchat_id} />
    </div>
  );
}
