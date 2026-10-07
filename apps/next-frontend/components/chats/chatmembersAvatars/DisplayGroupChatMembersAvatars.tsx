import { AppDispatch, RootState } from '@/store/store';
import { fetchGroupchatMembers } from '@/store/thunks/groupchatMembersThunk';
import {
  Avatar,
  AvatarFallback,
  AvatarGroup,
  AvatarGroupCount,
  AvatarImage,
} from '@meguchat/ui/components/ui/avatar';
import { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';

export function DisplayGroupChatMembersAvatars({
  groupchat,
}: {
  groupchat: Groupchat;
}) {
  const dispatch = useDispatch<AppDispatch>();

  // CHANGE THIS TO USE THE MEMBERS FIELD INSTEAD OF MAKING AN API CALL TO GET THE MEMBERS
  const { membersByGroupchatId, loadingByGroupchatId, errorByGroupchatId } =
    useSelector((state: RootState) => state.groupchatMembers);

  useEffect(() => {
    void dispatch(fetchGroupchatMembers(groupchat.groupchat_id));
  }, [dispatch, groupchat.groupchat_id]);

  const members = membersByGroupchatId[groupchat.groupchat_id] ?? [];
  const isLoading = loadingByGroupchatId[groupchat.groupchat_id] ?? false;
  const error = errorByGroupchatId[groupchat.groupchat_id] ?? null;

  const firstThreeMembers = members.slice(0, 3);

  if (isLoading) {
    return <div>Loading group chat members...</div>;
  }

  if (error) {
    return (
      <div className="text-destructive">
        {error.title}: {error.detail}
      </div>
    );
  }

  return (
    <AvatarGroup className="*:data-[slot=avatar]:ring-card *:data-[slot=avatar-group-count]:ring-card">
      {firstThreeMembers.map((member) => (
        <Avatar key={member.user_id}>
          {/* Use the actual user image */}
          {/* <AvatarImage src="https://github.com/shadcn.png" /> */}
          <AvatarFallback>{member.user_id.charAt(0)}</AvatarFallback>
        </Avatar>
      ))}
      {members.length > 3 && (
        <AvatarGroupCount>
          +{members.length - firstThreeMembers.length}
        </AvatarGroupCount>
      )}
    </AvatarGroup>
  );
}
