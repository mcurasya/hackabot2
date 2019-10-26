﻿using System;
using System.Collections.Generic;
using System.Linq;
using hackabot;
using Monad;
using StickerMemeDb.Model;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Commands
{
    public interface ICommand
    {
        Response Execute(Message message, Client.Client client, Account account,
            EitherStrict<ICommand, IEnumerable<IOneOfMany>> prevCommands);
    }

    public interface IOneOfMany : ICommand
    {
        bool Suitable(Message message, Account account);
    }
    public abstract class KeyboardButtonCommand : IOneOfMany
    {
        public abstract string        Name     { get; }
        public virtual bool Suitable(Message message, Account account) =>
            message.Text == Name;

        public abstract Response Execute(Message message, Client.Client client, Account account,
            EitherStrict<ICommand, IEnumerable<IOneOfMany>> prevCommands);
    }

    public abstract class InputCommand : ICommand
    {
        public abstract MessageType[] InputTypes { get; }

        public virtual bool Suitable(Message message, Account account) =>
            InputTypes.Contains(message.Type);

        public Response Execute(Message message, Client.Client client, Account account,
            EitherStrict<ICommand, IEnumerable<IOneOfMany>> prevCommands)
        {
            if(!Suitable(message, account))
                throw new BadInputException("Invalid input");
            return Run(message, client, account, prevCommands);
        }

        protected abstract Response Run(Message message, Client.Client client, Account account, EitherStrict<ICommand, IEnumerable<IOneOfMany>> prevCommands);
    }

    public abstract class StaticCommand : IOneOfMany
    {
        public abstract string Alias { get; }

        public virtual bool Suitable(Message message, Account account) =>
            message.Text == Alias;

        public abstract Response Execute(Message message, Client.Client client, Account account,
            EitherStrict<ICommand, IEnumerable<IOneOfMany>> prevCommands);
    }

    //todo this
    public class BadInputException : Exception
    {
        public Response ErrResponse;
        public BadInputException(string name)
        {
            
        }
        public BadInputException(Response errResponse)
        {
            ErrResponse = errResponse;
        }
    }
}