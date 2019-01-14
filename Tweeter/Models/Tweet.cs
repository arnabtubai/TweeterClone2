namespace Tweeter.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Tweet")]
    public partial class Tweet
    {
        [Key]
        public int tweet_id { get; set; }

        [StringLength(25)]
        public string user_id { get; set; }

        [Required]
        [StringLength(140)]
        public string Message { get; set; }

        public DateTime Created { get; set; }

        public virtual Person Person { get; set; }
    }
}
