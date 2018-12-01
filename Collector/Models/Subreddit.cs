using System;
using System.Collections.Generic;

namespace Collector.Models
{
    public class Subreddit
    {

        public int Id { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string PublicDescription { get; set; }
        public int? SubscribersCount { get; set; }
        public string HeaderImageUrl { get; set; }
        public DateTime? Created { get; set; }

        public ICollection<Post> Posts { get; set; }

    }
}
