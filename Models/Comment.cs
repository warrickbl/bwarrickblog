using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bwarrickblog.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int BlogPostId { get; set; }
        public int AuthorId { get; set; }
        public string Body { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string UpdateReason { get; set; }

        public virtual Post BlogPost { get; set; }
        public virtual ApplicationUser Author { get; set; }
    }
}