using System;
using System.Collections.Generic;
using System.Linq;
using hackabot;
using hackabot.Db.Model;
using Telegram.Bot.Types;

namespace hackabot2.Db.Controllers

{
    public class TelegramController
    {
        private static bool Delete;
        private static bool First = true;
        public TelegramContext Context;

        public void Start()
        {
            Context = new TelegramContext();
            Context.Database.EnsureCreated();
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }

        #region ChatId

        public static Dictionary<long, Account> Accounts = new Dictionary<long, Account>();

        public Account FromId(int id)
        {
            var account = Accounts.Values.FirstOrDefault(a => a.Id == id);
            if (account != null)
            {
                return account;
            }

            account = Context.Accounts.Find(id);
            Accounts.Add(account.ChatId, account);

            return account;
        }

        public Account FromMessage(Message message)
        {

            if (Accounts.ContainsKey(message.Chat.Id))
                return Accounts[message.Chat.Id];
            var account = Context.Accounts.FirstOrDefault(a => a.ChatId == message.Chat.Id);

            if (account == null)
            {
                account = new Account();
                account.ChatId = message.Chat.Id;
                account.Controller = this;
                account.Username = message.Chat.Username;
            }
            if (!Accounts.ContainsKey(account.ChatId))
                Accounts.Add(account.ChatId, account);

            return account;
        }

        public Account FromQuery(CallbackQuery message)
        {
            var account = Context.Accounts.FirstOrDefault(a => a.ChatId == message.From.Id);
            if (account != null) return account;
            //TODO create new  account maybe? or do something idk  
            account = new Account
            {
                ChatId = message.From.Id
            };
            Context.Accounts.Add(account);
            SaveChanges();

            return account;
        }

        #endregion

        #region Boards

        public Board GetBoard(int id) => Context.Boards.Find(id);

        #endregion

        #region DBQueries

        public void AddTask(Task task)
        {
            Context.Tasks.Add(task);
            SaveChanges();
        }

        public void AddBoard(Board board)
        {
            Context.Boards.Add(board);
            SaveChanges();
        }

        public void AddWorkerToBoard(WorkerToBoard value)
        {
            Context.WorkerToBoards.Add(value);
            SaveChanges();
        }

        public void ChangePriority(Task task, Priorities priority)
        {
            Context.Tasks.Find(task.Id).Priority = priority;
            SaveChanges();
        }

        public void ChangeWorkerAccessLevel(WorkerToBoard worker, AccessLevel accessLevel)
        {
            Context.WorkerToBoards.Find(worker.Id).AccessLevel = accessLevel;
            SaveChanges();
        }

        public Board[] GetBoards(Account account)
        {

            var to = Context.WorkerToBoards.Where(a => a.Worker.Id == account.Id);
            return Context.Boards.Where(board => to.FirstOrDefault(a => a.Board.Id == board.Id) != null).ToArray();
        }

        public List<Task> GetTasks(Board board, Account user)
        {
            return Context.Tasks.Where(task => task.Board == board && task.AssignedTo == user).ToList();
        }

        public String GetStatAboutUserByBoard(Account user)
        {
            var userTasks = Context.Tasks.Where(task => task.AssignedTo == user).ToList();
            return $@"current user has {userTasks.Count(task => task.Status != TaskStatus.Done)} assigned tasks, {userTasks.Count(task => task.Status == TaskStatus.Done)} closed tasks, has closed {userTasks.Count(task => task.Status == TaskStatus.Done && task.FinishDate.Date == DateTime.Today)} tasks today.";
        }

        #endregion

    }
}