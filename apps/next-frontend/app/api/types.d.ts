type Groupchat = {
  id: number;
  name: string;
  created_at?: string;
};

type CreateGroupchatRequest = {
  name: string;
  users: string[];
};

type ErrorResponse = {
  title: string;
  status: number;
  detail: string;
};
