export default async function GroupchatViewPage({
  params,
}: {
  params: { groupchat_id: string };
}) {
  const { groupchat_id } = await params;

  return (
    <div className="flex min-h-screen flex-col items-center justify-center bg-background">
      <h1 className="text-2xl font-bold">Groupchat ID: {groupchat_id}</h1>
    </div>
  );
}
