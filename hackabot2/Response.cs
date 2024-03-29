﻿using System.Collections.Generic;
using hackabot.Db.Model;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace hackabot
{
    public class Response
    {
        public Response()
        {
            Responses = new List<ResponseMessage>();
        }
        public List<ResponseMessage> Responses { get; set; }

        #region Constructors
        public Response TextMessage(ChatId chat, string text, IReplyMarkup replyMarkup = null,
            int replyToMessageId = 0)
        {
            Responses.Add(new ResponseMessage(ResponseType.TextMessage)
            {
                ChatId = chat,
                    Text = text,
                    ReplyMarkup = replyMarkup,
                    ReplyToMessageId = replyToMessageId
            });
            return this;
        }

        public Response EditTextMessage(ChatId chatId, int editMessageId, string text,
            IReplyMarkup replyMarkup = null)
        {
            Responses.Add(new ResponseMessage(ResponseType.EditTextMesage)
            {
                ChatId = chatId,
                    EditMessageId = editMessageId,
                    Text = text,
                    ReplyMarkup = replyMarkup
            });
            return this;
        }

        public Response AnswerQueryMessage(string answerToMessageId, string text)
        {
            Responses.Add(new ResponseMessage(ResponseType.AnswerQuery)
            {
                AnswerToMessageId = answerToMessageId,
                    Text = text
            });
            return this;
        }

        public Response SendDocument(Account account,
            InputOnlineFile document,
            string caption = null,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null)
        {
            Responses.Add(new ResponseMessage(ResponseType.SendDocument)
            {
                ChatId = account,
                    Text = caption,
                    ReplyToMessageId = replyToMessageId,
                    ReplyMarkup = replyMarkup,
                    Document = document
            });
            return this;
        }

        public Response EditMessageMarkup(ChatId accountChatId, int messageMessageId,
            InlineKeyboardMarkup addMemeButton)
        {
            Responses.Add(new ResponseMessage(ResponseType.EditMessageMarkup) { ChatId = accountChatId, MessageId = messageMessageId, ReplyMarkup = addMemeButton });
            return this;
        }
        #endregion
    }

    public class ResponseMessage
    {
        public ResponseMessage(ResponseType type)
        {
            Type = type;
        }

        public ResponseMessage() { }

        public ChatId ChatId { get; set; }
        public string Text { get; set; }
        public int ReplyToMessageId { get; set; }
        public IReplyMarkup ReplyMarkup { get; set; }
        public int EditMessageId { get; set; }
        public string AnswerToMessageId { get; set; }
        public InputOnlineFile Document { get; set; }
        public ResponseType Type { get; }
        public IEnumerable<IAlbumInputMedia> Album { get; set; }
        public int MessageId { get; set; }
    }

    public enum ResponseType
    {
        TextMessage,
        EditTextMesage,
        AnswerQuery,
        SendDocument,
        SendPhoto,
        Album,
        EditMessageMarkup
    }
}