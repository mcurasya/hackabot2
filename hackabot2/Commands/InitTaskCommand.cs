using System.Collections.Generic;
using hackabot;
using hackabot.Client;
using hackabot.Commands;
using hackabot.Db.Model;
using Monad;
using Telegram.Bot.Types;

namespace hackabot2.Commands
{
    public class InitTaskCommand : ICommand
    {
        public enum CommandStatus
        {
          name,
          description,
          priority,
          deadline
        }

        private CommandStatus _status = CommandStatus.name;
        public Response Execute(Message message, Client client, Account account, EitherStrict<ICommand, IEnumerable<IOneOfMany>> prevCommands)
        {
            //todo implementation
            if (_status == CommandStatus.name)
            {
                this._status = CommandStatus.description;
                return new WaitForTaskNameCommand().Execute(message,client,account,this);
            }
            if (_status == CommandStatus.description)
            {
                this._status = CommandStatus.priority;
                return new Response(this).TextMessage(account.ChatId, "And know please give description about your task");
            }
            if (_status == CommandStatus.priority)
            {
                this._status = CommandStatus.deadline;
                return new Response(this).TextMessage(account.ChatId, "How important this task?"); //todo add keyboard
            }

            if (_status == CommandStatus.deadline)
            {
                return new Response(null).TextMessage(account.ChatId, "Task added succesfully"); 
            }
            throw new System.NotImplementedException();
        }
    }
}