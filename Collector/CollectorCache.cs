using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Collector.Models;
using RedditSharp;

namespace Collector
{
    public static class CollectorCache
    {

        #region Fields

        private static readonly SemaphoreSlim UsersSemaphore = new SemaphoreSlim(1, 1);

        #endregion

        #region Properties

        public static Dictionary<string, User> Users { get; private set; } = new Dictionary<string, User>();

        #endregion

        #region Public Methods

        public static async Task<User> GetUser(Reddit reddit, string name)
        {
            await UsersSemaphore.WaitAsync();

            if (!Users.ContainsKey(name))
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

                Users.Add(name, user);
            }

            UsersSemaphore.Release();
            return Users[name];
        }

        public static void LoadUsers(DatabaseContext db)
        {
            Users = db.Users.ToDictionary(u => u.Name, u => u);
        }

        #endregion

    }
}
