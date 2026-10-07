type MessageType = {
  id: number;
  content: string;
  user_id: string;
  group_chat_id: number;
  created_at: string;
  modified_at: string;
  status?: 'sending' | 'sent' | 'error';
};

type MessageRequest = {
  content: string;
};
