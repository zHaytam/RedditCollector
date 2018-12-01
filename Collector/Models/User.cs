using System;
using System.Collections.Generic;

namespace Collector.Models
{
    public class User
    {

        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public bool HideFromRobots { get; set; }
        public int CommentKarma { get; set; }
        public int LinkKarma { get; set; }
        public bool IsVerified { get; set; }
        public bool IsModerator { get; set; }
        public bool HasGold { get; set; }
        public bool HasSubscribed { get; set; }
        public bool? HasVerifiedEmail { get; set; }
        public bool IsEmployee { get; set; }
        public DateTime Created { get; set; }

        public ICollection<Post> Posts { get; set; }

    }
}
