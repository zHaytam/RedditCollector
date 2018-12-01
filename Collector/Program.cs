using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Collector.Models;
using Collector.Properties;
using EFCore.BulkExtensions;
using RedditSharp;
using ShellProgressBar;

namespace Collector
{
    internal class Program
    {

        #region Fields

        private static List<BotWebAgent> _webAgents;
        private static List<Reddit> _reddits;

        #endregion

        private static void InitializeReddits()
        {
            string[] usernames = Resources.Usernames.Split(',');
            string[] passwords = Resources.Passwords.Split(',');
            string[] clientIds = Resources.ClientIDs.Split(',');
            string[] clientSecrets = Resources.ClientSecrets.Split(',');

            _webAgents = new List<BotWebAgent>(usernames.Length);
            _reddits = new List<Reddit>(usernames.Length);

            for (int i = 0; i < usernames.Length; i++)
            {
                _webAgents.Add(new BotWebAgent(usernames[i], passwords[i], clientIds[i], clientSecrets[i], Resources.RedirectUri));
                _reddits.Add(new Reddit(_webAgents[i], true));
            }
        }

        private static async Task Main(string[] args)
        {
            // Initialize reddits
            InitializeReddits();

            // Collect subreddits
            // await CollectSubredditsAsync();

            // Collect posts
            await CollectPostsAndAuthorsAsync();

            Console.ReadKey();
        }

        private static async Task CollectPostsAndAuthorsAsync()
        {
            using (var db = DatabaseContext.New)
            {
                // Load back users cache
                CollectorCache.LoadUsers(db);

                // Pick up where we left
                var srs = db.Subreddits.Skip(85).ToList();

                using (var pb = new ProgressBar(srs.Count, $"Collecting posts from {srs.Count} subreddits", new ProgressBarOptions
                {
                    ForegroundColor = ConsoleColor.Yellow,
                    BackgroundColor = ConsoleColor.DarkYellow,
                    ProgressCharacter = '─',
                    ProgressBarOnBottom = true,
                    EnableTaskBarProgress = true
                }))
                {
                    // ReSharper disable AccessToDisposedClosure
                    var tasks = SplitList(srs, _reddits.Count).Select((srsSublist, i) => CollectPostsAndAuthorsOfSubreddits(_reddits[i], srsSublist, db, pb)).ToArray();
                    // ReSharper restore AccessToDisposedClosure

                    await Task.WhenAll(tasks);
                }
            }
        }

        private static async Task CollectPostsAndAuthorsOfSubreddits(Reddit reddit, IEnumerable<Subreddit> subreddits, DatabaseContext db, ProgressBar pb)
        {
            foreach (var sr in subreddits)
            {
                // Get subreddit object
                var subreddit = await reddit.GetSubredditAsync($"/r/{sr.Name}");
                using (var childPb = pb.Spawn(1000, $"Collecting posts from /r/{sr.Name}", new ProgressBarOptions
                {
                    ForegroundColor = ConsoleColor.Green,
                    BackgroundColor = ConsoleColor.DarkGreen,
                    ProgressCharacter = '─'
                }))
                {
                    // ReSharper disable AccessToDisposedClosure
                    int collected = 0;

                    // For each post in the subreddit
                    await subreddit.GetPosts().ForEachAsync(p =>
                    {
                        // Get author user
                        var author = CollectorCache.GetUser(reddit, p.AuthorName).Result;
                        if (author == null)
                            return;

                        // Create post model with the author
                        var post = new Post
                        {
                            PostId = p.Id,
                            Author = author,
                            Title = p.Title,
                            SelfText = p.SelfText,
                            CommentCount = p.CommentCount,
                            Url = p.Url?.OriginalString,
                            ThumbnailUrl = p.Thumbnail?.OriginalString,
                            Nsfw = p.NSFW,
                            Upvotes = p.Upvotes,
                            Downvotes = p.Downvotes,
                            IsArchived = p.IsArchived,
                            IsApproved = p.IsApproved,
                            IsRemoved = p.IsRemoved,
                            ReportCount = p.ReportCount,
                            Score = p.Score,
                            Kind = p.Kind,
                            Created = p.Created,
                            Subreddit = sr
                        };

                        db.Posts.Add(post);
                        childPb.Tick();

                        collected++;
                        if (collected % 100 == 0)
                            db.SaveChanges();
                    });
                    // ReSharper restore AccessToDisposedClosure
                }

                await db.SaveChangesAsync();
                pb.Tick();
            }
        }

        private static async Task CollectSubredditsAsync()
        {
            using (var db = DatabaseContext.New)
            {
                using (var pb = new ProgressBar(2, "Collecting subreddits", new ProgressBarOptions
                {
                    ProgressCharacter = '─',
                    ProgressBarOnBottom = true
                }))
                {
                    var subreddits = await _reddits[0].SearchSubreddits("game").Select(sr => new Subreddit
                    {
                        Title = sr.Title,
                        Name = sr.DisplayName,
                        PublicDescription = sr.PublicDescription,
                        SubscribersCount = sr.Subscribers,
                        HeaderImageUrl = sr.HeaderImage,
                        Created = sr.Created
                    }).ToList();

                    pb.Tick($"Saving {subreddits.Count} subreddits...");
                    db.Subreddits.AddRange(subreddits);
                    await db.SaveChangesAsync();
                    pb.Tick($"{subreddits.Count} subreddits collected!");
                }
            }
        }

        public static IEnumerable<IEnumerable<T>> SplitList<T>(IEnumerable<T> list, int parts)
        {
            int i = 0;
            return from item in list
                   group item by i++ % parts into part
                   select part.AsEnumerable();
        }

    }
}
