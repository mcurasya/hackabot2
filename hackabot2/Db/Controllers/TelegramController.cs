using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StickerMemeDb.Model;
using Telegram.Bot.Types;

namespace StickerMemeDb.Controllers

{
    public class TelegramController
    {
        private static bool            Delete;
        private static bool            First = true;
        public         TelegramContext Context;

        public void Start()
        {
            Context = new TelegramContext();
            if (First)
            {
                if (Delete)
                {
                    Context.Database.EnsureCreated();
                    var text = Context.Texts.ToList();
                    Context.Database.EnsureDeleted();
                    Context.Database.EnsureCreated();
                    Context.Texts.AddRange(text);
                    SaveChanges();
                    Delete = false;
                }
                else
                {
                    Context.Database.EnsureCreated();
                }

                First = false;
            }
            else
            {
                Context.Database.EnsureCreated();
            }
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }

#region Stats

        public void LikeMeme(int memeId, int accountId)
        {
            var like = Context.Likes.FirstOrDefault(l => l.AccountId == accountId && l.MemeId == memeId);
            var meme = Context.Memes.Find(memeId);
            if (like == null)
            {
                meme.Likes++;
                Context.Likes.Add(new Like {AccountId = accountId, MemeId = memeId});
            }
            else
            {
                meme.Likes--;
                Context.Likes.Remove(like);
            }

            SaveChanges();
        }

        public int CountLikes(int memeId)
        {
            return Context.Likes.Count(l => l.MemeId == memeId);
        }

#endregion

#region Admin

        public string GetChannelStats(int accountId)
        {
            //TODO referals and staff
            var channel = GetChannel(accountId);
            return
            $"{channel.Name} stats:\nMemes sold: {channel.MemesSold}\nMemes submited: {channel.MemesSubmited}\nMoney earned: {channel.MoneyEarned}.";
        }

        public Channel GetChannel(int accountId)
        {
            return Context.Channels.FirstOrDefault(c => c.AdminId == accountId);
        }

        public Channel GetChannel(string submitedBy)
        {
            return Context.Channels.FirstOrDefault(ch => ch.Name == submitedBy);
        }

#endregion

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
            var start = message.Text?.Length > "/start".Length && message.Text.StartsWith("/start");
            if (Accounts.ContainsKey(message.Chat.Id) && !start) return Accounts[message.Chat.Id];
            var account = Context.Accounts.FirstOrDefault(a => a.ChatId == message.Chat.Id);
            if (message.Text != null)
                if (start)
                {
                    var param              = message.Text.Substring(7);
                    var base64EncodedBytes = Convert.FromBase64String(param);
                    param = Encoding.UTF8.GetString(base64EncodedBytes);
                    var p = param.Split('*');

                    // p[0] == m
                    // p[1] = memeId
                    // p[2] = referal

                    //p[0] = type m = memetocart a = admin in = referal invite
                    //p[1] = channel link
                    //p[2] = account username
                    if (p[0] == "m")
                    {
                        if (account == null)
                            account = CreateCustomer(message, p);
                        AddMemeToCart(int.Parse(p[1]), account.Id);
                    }
                    else if (p[0] == "a")
                    {
                        if (account == null)
                            account         = CreateAdmin(message, p);
                        else account.Rights = AccountRights.Admin;
                        CreateChannel(account, p[1]);
                    }
                    else if (p[0] == "in")
                    {
                        //create user if null
                        //add referal
                    }
                }

            if (account == null) account = CreateCustomer(message);
            if (!Accounts.ContainsKey(account.ChatId))
                Accounts.Add(account.ChatId, account);
            //TODO
            // account.Language = message.From.LanguageCode;
            account.Language = "ru";

            return account;
        }

        public Account FromQuery(CallbackQuery message)
        {
            var account = Context.Accounts.FirstOrDefault(a => a.ChatId == message.From.Id);
            if (account != null) return account;
            //TODO create new  account maybe? or do something idk  
            account = new Account
            {
                ChatId   = message.From.Id,
                Language = message.From.LanguageCode,
                UserName = message.From.Username,
                Rights   = AccountRights.Customer,
                Status   = AccountStatus.Start
            };
            Context.Accounts.Add(account);
            SaveChanges();

            return account;
        }

        private Account CreateAdmin(Message message, string[] p)
        {
            var account = new Account
            {
                ChatId   = message.Chat.Id,
                UserName = message.From.Username,
                Status   = AccountStatus.Start,
                Rights   = AccountRights.Admin
            };
            Context.Accounts.Add(account);
            SaveChanges();
            CreateChannel(account, p[1]);
            return account;
        }

        private void CreateChannel(Account account, string url)
        {
            var ch = Context.Channels.FirstOrDefault(c => c.Url == url);
            if (ch == null)
            {
                var channel = new Channel
                {
                    AdminId = account.Id,
                    Url     = url
                };
                Context.Channels.Add(channel);
            }
            //watch it
            else
            {
                ch.AdminId = account.Id;
            }

            SaveChanges();
        }

        private Account CreateCustomer(Message message, string[] p = null)
        {
            var account = new Account
            {
                ChatId   = message.Chat.Id,
                UserName = message.Chat.Username,
                Status   = AccountStatus.Start
            };
            if (message.Chat.Username == null)
                account.UserName = message.Chat.FirstName + " " + message.Chat.LastName;
            Context.Accounts.Add(account);
            Context.SaveChanges();
            if (p != null)
                AddMemeToCart(int.Parse(p[1]), account.Id);
            return account;
        }

        public void AddDetails(string details, int accountId)
        {
            var account = FromId(accountId);
            if (account != null)
            {
                account.Details = details;
                SaveChanges();
            }
        }

#endregion

#region Meme

        public bool FileIdExist(string fileId)
        {
            return Context.Memes.Any(meme => meme.FileId == fileId);
        }

        public void AddMeme(Meme meme)
        {
            if (meme.FromChannel)
            {
                var channel = GetChannel(meme.SubmitedBy);
                if (channel != null)
                    channel.MemesSubmited++;
            }

            Context.Images.Add(meme.Image);
            Context.SaveChanges();
            meme.ImageId = meme.Image.Id;
            Context.Memes.Add(meme);
            Context.SaveChanges();
        }

        public Meme GetMeme(int memeId)
        {
            var meme = Context.Memes.Find(memeId);
            if (meme == null) return null;
            meme.Image = Context.Images.Find(meme.ImageId);
            return meme;
        }

        public Meme GetRandomMeme()
        {
            var rand  = new Random();
            var count = Context.Images.Count();
            if (count > 0)
            {
                var toSkip = rand.Next(0, count);
                var meme   = Context.Memes.Skip(toSkip).Take(1).First();
                meme.Image = Context.Images.Find(meme.ImageId);
                return meme;
            }

            return null;
        }

#endregion

#region Shopping Cart

        private ShoppingCart GetShoppingCart(int accountId)
        {
            return Context.Carts.FirstOrDefault(c => c.AccountId == accountId && !c.Closed);
        }

        public void AddMemeToCart(int memeId, int accountId)
        {
            var cart = GetShoppingCart(accountId);
            if (cart == null)
            {
                cart = new ShoppingCart
                {
                    AccountId = accountId,
                    Closed    = false
                };
                Context.Carts.Add(cart);
                SaveChanges();
            }

            Context.CartItems.Add(new ShoppingCartItem
            {
                CartId = cart.Id,
                MemeId = memeId
            });
            SaveChanges();
        }

        public List<ShoppingCartItem> GetCart(int accountId)
        {
            var cart = GetShoppingCart(accountId);
            if (cart == null) return new List<ShoppingCartItem>();
            return Context.CartItems.Where(i => i.CartId == cart.Id).ToList();
        }

        public double GetCartCost(int accountId)
        {
            var cart = GetShoppingCart(accountId);
            if (cart == null) return 0;

            var items = Context.CartItems.Where(i => i.CartId == cart.Id);
            var memes =
            items.Select(m => Context.Memes.FirstOrDefault(meme => meme.Id == m.MemeId));
            double price                   = 0;
            foreach (var m in memes) price += m?.Price ?? 0;
            return price;
        }

        public string CartToMessage(int accountId)
        {
            var cartItems     = GetCart(accountId);
            var stringBuilder = new StringBuilder();
            var i             = 0;
            foreach (var item in cartItems)
            {
                i++;
                var meme = Context.Memes.Find(item.MemeId);
                stringBuilder.Append(
                    $"{i}. {meme.DateAdded.ToShortDateString()}\nPrice: {meme.Price}₴\nShow image: /getmeme_{accountId}_{meme.Id}\n");
            }

            return stringBuilder.ToString();
        }

        public void MergeCarts(int toId, int fromId)
        {
            var from                          = Context.Carts.Find(fromId);
            var items                         = Context.CartItems.Where(i => i.CartId == fromId);
            foreach (var i in items) i.CartId = toId;
            Context.Carts.Remove(from);
            SaveChanges();
        }

#endregion

#region Orders

        public Order AddOrder(int accountId)
        {
            var cart = GetShoppingCart(accountId);
            if (cart == null) return null;
            var order = Context.Orders.FirstOrDefault(or => !or.Printed && or.AccountId == accountId);

            var items = GetCart(accountId);
            if (order == null)
            {
                order = new Order
                {
                    ShoppingCartId = cart.Id,
                    OnSheet        = false,
                    AccountId      = accountId,
                    Date           = DateTime.Now,
                    Payed          = false,
                    Printed        = false,
                    Shipped        = false,
                    Price          = items.Sum(i => GetMeme(i.MemeId).Price)
                };
                cart.Closed = true;
                Context.Orders.Add(order);
            }
            else
            {
                MergeCarts(order.ShoppingCartId, cart.Id);
            }

            foreach (var item in items) Context.Memes.Find(item.MemeId).Orders++;
            SaveChanges();
            return order;
        }

        public string OrderToMessage(Order order, Account account)
        {
            var builder = new StringBuilder();
            var cart    = Context.CartItems.Count(i => i.CartId == order.ShoppingCartId);
            builder.AppendLine($"{account.UserName} orders {cart} stikers");
            builder.AppendLine(
                $"Order unique code is {account.UserName} {new string(order.Id.ToString().Reverse().ToArray())}");
            return builder.ToString();
        }

        public double CountStickerSpace()
        {
            return Context.Orders.Sum(o =>
            Context.CartItems.Where(item => item.CartId == o.ShoppingCartId)
                   .Sum(it => Context.Memes.Where(m => m.Id == it.MemeId)
                                     .Sum(me => me.Height * me.Width)));
        }

        public Order ConfirmPayment(int orderId)
        {
            var order = Context.Orders.Find(orderId);
            order.Payed = true;
            var memes = Context.CartItems.Where(item => item.CartId == order.ShoppingCartId)
                               .Select(i => Context.Memes.FirstOrDefault(m => m.Id == i.MemeId));

            foreach (var meme in memes)
                if (meme.FromChannel)
                {
                    double revenue = 0;
                    switch (meme.Price)
                    {
                        case 3:
                            revenue = 1;
                            break;
                        case 4:
                            revenue = 1.5;
                            break;
                        case 5:
                            revenue = 2;
                            break;
                    }

                    var channel = GetChannel(meme.SubmitedBy) ??
                                  GetChannel(Context.Accounts.FirstOrDefault(a => a.UserName == meme.SubmitedBy).Id);
                    channel.MemesSold++;
                    channel.MoneyEarned += revenue;
                }

            SaveChanges();
            return order;
        }

#endregion

#region Localisation

        public string GetText(string key, string language)
        {
            var text = Context.Texts.Find(key, language);
            if (text == null)
            {
                text = new Text {Key = key, Value = key, Language = language};
                Context.Texts.Add(text);
                SaveChanges();
            }

            var langs = new List<string> {"ru", "en", "ua"};
            langs.Remove(language);
            foreach (var l in langs)
                if (Context.Texts.Find(key, l) == null)
                    Context.Texts.Add(new Text {Key = key, Value = key, Language = l});
            SaveChanges();

            return text.Value;
        }

        public string GetChannelLanguage(long chatId)
        {
            var channel = Context.Channels.FirstOrDefault(ch => ch.ChatId == chatId);
            return channel?.Language;
        }

#endregion
    }
}