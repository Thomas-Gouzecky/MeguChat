type Groupchat = {
  id: number;
  name: string;
  created_at?: string;
};

type ErrorResponse = {
  title: string;
  status: number;
  detail: string;
};
