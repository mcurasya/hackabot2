using System;
using System.Collections.Generic;
using System.Linq;
using hackabot.Commands;
using hackabot.Db.Model;
using Monad;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace hackabot
{
    /*
      todo add command to all constructors
      todo set dafalt next commands as prev commands
     */
    public class QuerryResponse : Response
    {
        public QuerryResponse(EitherStrict<ICommand, IEnumerable<IOneOfMany>> nextPossible) : base(nextPossible)
        {
        }

        public QuerryResponse()
        {
            
        }
    }
    public class Response
    {
        protected Response()
        {
            Responses = new List<ResponseMessage>();
        }

        public Response(EitherStrict<ICommand, IEnumerable<IOneOfMany>> nextPossible)
        {
            Responses = new List<ResponseMessage>();
            this.nextPossible = nextPossible;
        }
        public List<ResponseMessage> Responses { get; set; }

        public EitherStrict<ICommand, IEnumerable<IOneOfMany>> nextPossible { get; }

        public EitherStrict<Response, IEnumerable<Response>> Eval(Account a, Message m, Client.Client c) =>
            Eval(a, m, c, this.nextPossible);
        public static EitherStrict<Response, IEnumerable<Response>> Eval(Account a, Message m, Client.Client c, EitherStrict<ICommand, IEnumerable<IOneOfMany>> nextPossible)
        {
            try
            {
                if (nextPossible.IsLeft)
                    return nextPossible.Left.Execute(m, c, a, nextPossible);
                var right = nextPossible.Right.ToList();
                right.AddRange(StaticCommands);
                var toRet = right.Where(t => t.Suitable(m, a)).Select(t => t.Execute(m, c, a, nextPossible));
                return EitherStrict.Right<Response, IEnumerable<Response>>(toRet);

            }
            catch (BadInputException e)
            {
                return e.ErrResponse;
            }
        }

        static Response()
        {
            var baseType = typeof(StaticCommand);
            var assembly = baseType.Assembly;
            StaticCommands = assembly
                .GetTypes()
                .Where(t => t.IsSubclassOf(baseType) && !t.IsAbstract)
                .Select(c => Activator.CreateInstance(c) as IOneOfMany)
                .Where(c => c != null).ToList();
        }

        public static List<IOneOfMany> StaticCommands; 

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