using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Collector.Models;
using Collector.Properties;
using RedditSharp;
using ShellProgressBar;

namespace Collector
{
    internal class Program
    {

        #region Fields

        private static BotWebAgent _webAgent;
        private static Reddit _reddit;

        #endregion

        private static async Task Main(string[] args)
        {
            _webAgent = new BotWebAgent(Resources.Usernames.Split(',')[0], Resources.Passwords.Split(',')[0], Resources.ClientIDs.Split(',')[0], Resources.ClientSecrets.Split(',')[0], Resources.RedirectUri);
            _reddit = new Reddit(_webAgent, false);

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

                var srs = db.Subreddits.Skip(86).ToArray();

                using (var pb = new ProgressBar(srs.Length, $"Collecting posts from {srs.Length} subreddits", new ProgressBarOptions
                {
                    ForegroundColor = ConsoleColor.Yellow,
                    BackgroundColor = ConsoleColor.DarkYellow,
                    ProgressCharacter = '─',
                    ProgressBarOnBottom = true,
                    EnableTaskBarProgress = true
                }))
                {
                    // For each saved subreddit
                    foreach (var sr in srs)
                    {
                        // Get subreddit object
                        var subreddit = await _reddit.GetSubredditAsync($"/r/{sr.Name}");
                        using (var childPb = pb.Spawn(1000, $"Collecting posts from /r/{sr.Name}", CollectorCache.NewSpawnOptions()))
                        {
                            int collected = 0;

                            // For each post in the subreddit
                            await subreddit.GetPosts().ForEachAsync(p =>
                            {
                                // Get author user
                                var author = CollectorCache.GetUser(_reddit, p.AuthorName).Result;
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

                                // ReSharper disable AccessToDisposedClosure
                                db.Posts.Add(post);
                                childPb.Tick();

                                collected++;
                                if (collected % 100 == 0)
                                    db.SaveChanges();
                                // ReSharper restore AccessToDisposedClosure
                            });

                        }

                        await db.SaveChangesAsync();
                        pb.Tick();
                    }
                }
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
                    var subreddits = await _reddit.SearchSubreddits("game").Select(sr => new Subreddit
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

    }
}
