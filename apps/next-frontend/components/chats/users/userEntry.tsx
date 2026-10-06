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
  return <div>{user.username}</div>;
}
