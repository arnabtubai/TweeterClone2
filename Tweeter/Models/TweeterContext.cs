namespace Tweeter.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class TweeterContext : DbContext
    {
        public TweeterContext()
            : base("name=TweeterContext")
        {
            
        }

        public virtual DbSet<Person> People { get; set; }
        public virtual DbSet<Tweet> Tweets { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
          

            modelBuilder.Entity<Person>()
                .HasMany(e => e.Following)
                .WithMany(e => e.Followers)
                .Map(m => m.ToTable("Following").MapLeftKey("user_id").MapRightKey("following_id"));
        }
    }
}
