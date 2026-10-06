export function UserEntry({
  className,
  user,
}: {
  className?: string;
  user: User;
}) {
  return (
    <label className={className}>
      <input type="checkbox" name="users" value={user.user_id} />
      <span>{user.username}</span>
    </label>
  );
}
