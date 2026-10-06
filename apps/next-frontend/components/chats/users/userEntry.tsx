type UserEntryProps = {
  user: User;
  isSelected: boolean;
  onSelect: (userId: number) => void;
};

export function UserEntry({
  className,
  user,
  isSelected,
  onSelect,
}: UserEntryProps & { className?: string }) {
  return (
    <label className={className}>
      <input
        type="checkbox"
        name="users"
        value={user.user_id}
        checked={isSelected}
        onChange={() => onSelect(user.user_id)}
      />
      <span>{user.username}</span>
    </label>
  );
}
