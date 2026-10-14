import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { fetchMessagesByGroupchatId } from '../thunks/messagesThunk';

interface MessagesState {
  messagesByGroupchatId: Record<string, MessageType[]>;
  loadingByGroupchatId: Record<string, boolean>;
  errorByGroupchatId: Record<string, ErrorResponse | null>;
}

const initialState: MessagesState = {
  messagesByGroupchatId: {},
  loadingByGroupchatId: {},
  errorByGroupchatId: {},
};

const messagesSlice = createSlice({
  name: 'messages',
  initialState,
  reducers: {
    addMessage: (
      state,
      action: PayloadAction<{ groupchat_id: string; message: MessageType }>,
    ) => {
      const { groupchat_id, message } = action.payload;
      state.messagesByGroupchatId[groupchat_id] ??= [];
      const messages = state.messagesByGroupchatId[groupchat_id];

      if (messages.some((existing) => existing.id === message.id)) {
        return;
      }

      const pendingMessageIndex = messages.findIndex(
        (existing) =>
          existing.status === 'sending' &&
          existing.user_id === message.user_id &&
          existing.content === message.content,
      );

      if (pendingMessageIndex !== -1) {
        messages[pendingMessageIndex] = { ...message, status: 'sent' };
        return;
      }

      messages.push(message);
    },
    removeMessage: (
      state,
      action: PayloadAction<{ groupchat_id: string; message_id: string }>,
    ) => {
      const { groupchat_id, message_id } = action.payload;
      state.messagesByGroupchatId[groupchat_id] =
        state.messagesByGroupchatId[groupchat_id]?.filter(
          (message) => message.id.toString() !== message_id,
        ) ?? [];
    },
    editMessage: (
      state,
      action: PayloadAction<{ groupchat_id: string; message: MessageType }>,
    ) => {
      const { groupchat_id, message } = action.payload;
      state.messagesByGroupchatId[groupchat_id] =
        state.messagesByGroupchatId[groupchat_id]?.map((msg) => {
          if (msg.id === message.id) {
            return message;
          }
          return msg;
        }) ?? [];
    },
    replaceMessage: (
      state,
      action: PayloadAction<{
        groupchat_id: string;
        message_id: number;
        message: MessageType;
      }>,
    ) => {
      const { groupchat_id, message_id, message } = action.payload;
      state.messagesByGroupchatId[groupchat_id] =
        state.messagesByGroupchatId[groupchat_id]?.map((msg) =>
          msg.id === message_id ? message : msg,
        ) ?? [];
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchMessagesByGroupchatId.pending, (state, action) => {
        state.loadingByGroupchatId[action.meta.arg] = true;
        state.errorByGroupchatId[action.meta.arg] = null;
      })
      .addCase(fetchMessagesByGroupchatId.fulfilled, (state, action) => {
        state.loadingByGroupchatId[action.meta.arg] = false;
        state.messagesByGroupchatId[action.meta.arg] = action.payload;
      })
      .addCase(fetchMessagesByGroupchatId.rejected, (state, action) => {
        state.loadingByGroupchatId[action.meta.arg] = false;
        state.errorByGroupchatId[action.meta.arg] = action.payload ?? null;
      });
  },
});

export const { addMessage, removeMessage, editMessage, replaceMessage } =
  messagesSlice.actions;
export default messagesSlice.reducer;
