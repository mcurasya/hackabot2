using System.Collections.Generic;
using System.Linq;
using hackabot.Db.Model;
using Telegram.Bot.Types;

namespace hackabot.Queries
{
    public abstract class Query
    {
        public abstract string Alias { get; }

        public Response Execute(CallbackQuery message, Account account) =>
            Run(message, account, UnpackParams(message.Data));

        protected abstract Response Run(CallbackQuery message, Account account, Dictionary<string, string> values);

        public virtual bool IsSuitable(CallbackQuery message, Account account)
        {
            return message.Data.StartsWith(Alias);
        }

        public static Dictionary<string, string> UnpackParams(string input)
        {
            if (!input.Contains(' ') || !input.Contains('='))
                return new Dictionary<string, string>();
            return input.Substring(input.IndexOf(' ') + 1)
                .Split('&')
                .Select(s => s.Split('='))
                .ToDictionary(r => r[0], r => r[1]);
        }

        public static string PackParams(string Alias, string Name, string Value) =>
            PackParams(Alias, (Name, Value));

        public static string PackParams(string Alias, params(string Name, string Value) [] input) =>
            $"{Alias} {string.Join('&'.ToString(), input.Select(i => $"{i.Name}={i.Value}"))}";
        //getstats accountid=23
        //get_board boardid=3
    }
}