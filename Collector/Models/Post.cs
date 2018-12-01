using System;

namespace Collector.Models
{
    public class Post
    {

        public int Id { get; set; }
        public int AuthorId { get; set; }
        public int SubredditId { get; set; }
        public string PostId { get; set; }
        public string Title { get; set; }
        public string SelfText { get; set; }
        public int CommentCount { get; set; }
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        public bool Nsfw { get; set; }
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }
        public bool? IsArchived { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsRemoved { get; set; }
        public int? ReportCount { get; set; }
        public int Score { get; set; }
        public string Kind { get; set; }
        public DateTime Created { get; set; }

        public Subreddit Subreddit { get; set; }
        public User Author { get; set; }

    }
}
