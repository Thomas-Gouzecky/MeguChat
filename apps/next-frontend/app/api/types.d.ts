type Groupchat = {
  groupchat_id: number;
  name: string;
  created_at?: string;
};

type CreateGroupchatRequest = {
  name: string;
  users: string[];
};

type User = {
  user_id: string;
  username: string;
};

// this i think is a temporary type until we get the backend to return the ASP.NET Identity user instead of this member
// this will be done on a different endpoint too so i can get both. currently, i dont have an endpoint to get the ASP.NET Identity user
type GroupchatMember = {
  id: number;
  group_chat_id: number;
  user_id: string;
  joined_at: string;
  last_active_at: string;
  last_read_message_id: string | null;
};

type ErrorResponse = {
  title: string;
  status: number;
  detail: string;
};
