using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Collector.Models;
using RedditSharp;
using ShellProgressBar;

namespace Collector
{
    public static class CollectorCache
    {

        #region Fields

        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);
        private static Dictionary<string, User> _users = new Dictionary<string, User>();
        private static readonly List<ProgressBarOptions> SpawnsOptions = new List<ProgressBarOptions>();

        #endregion

        #region Public Methods

        public static async Task<User> GetUser(Reddit reddit, string name)
        {
            await Semaphore.WaitAsync();

            if (!_users.ContainsKey(name))
            {
                User user;
                try
                {
                    var userInfos = await reddit.GetUserAsync(name);
                    user = new User
                    {
                        UserId = userInfos.Id,
                        Name = userInfos.Name,
                        FullName = userInfos.FullName,
                        HideFromRobots = userInfos.HideFromRobots,
                        CommentKarma = userInfos.CommentKarma,
                        LinkKarma = userInfos.LinkKarma,
                        IsVerified = userInfos.IsVerified,
                        IsModerator = userInfos.IsModerator,
                        HasGold = userInfos.HasGold,
                        HasSubscribed = userInfos.HasSubscribed,
                        HasVerifiedEmail = userInfos.HasVerifiedEmail,
                        IsEmployee = userInfos.IsEmployee,
                        Created = userInfos.Created
                    };
                }
                catch
                {
                    user = null;
                }

                _users.Add(name, user);
            }

            Semaphore.Release();
            return _users[name];
        }

        public static void LoadUsers(DatabaseContext db)
        {
            _users = db.Users.ToDictionary(u => u.Name, u => u);
        }

        public static ProgressBarOptions NewSpawnOptions()
        {
            Semaphore.Wait();

            if (SpawnsOptions.Count == 6)
            {
                SpawnsOptions[0].CollapseWhenFinished = true;
                SpawnsOptions.RemoveAt(0);
            }

            var spawnOption = new ProgressBarOptions
            {
                ForegroundColor = ConsoleColor.Green,
                BackgroundColor = ConsoleColor.DarkGreen,
                ProgressCharacter = '─',
                CollapseWhenFinished = false
            };

            SpawnsOptions.Add(spawnOption);
            Semaphore.Release();
            return spawnOption;
        }

        #endregion

    }
}
