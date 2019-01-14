namespace Tweeter.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Person",
                c => new
                    {
                        user_id = c.String(nullable: false, maxLength: 25),
                        Password = c.String(nullable: false, maxLength: 25),
                        FullName = c.String(nullable: false, maxLength: 30),
                        Email = c.String(nullable: false, maxLength: 50),
                        Joined = c.DateTime(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.user_id);
            
            CreateTable(
                "dbo.Tweet",
                c => new
                    {
                        tweet_id = c.Int(nullable: false, identity: true),
                        user_id = c.String(maxLength: 25),
                        Message = c.String(nullable: false, maxLength: 140),
                        Created = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.tweet_id)
                .ForeignKey("dbo.Person", t => t.user_id)
                .Index(t => t.user_id);
            
            CreateTable(
                "dbo.Following",
                c => new
                    {
                        user_id = c.String(nullable: false, maxLength: 25),
                        following_id = c.String(nullable: false, maxLength: 25),
                    })
                .PrimaryKey(t => new { t.user_id, t.following_id })
                .ForeignKey("dbo.Person", t => t.user_id)
                .ForeignKey("dbo.Person", t => t.following_id)
                .Index(t => t.user_id)
                .Index(t => t.following_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tweet", "user_id", "dbo.Person");
            DropForeignKey("dbo.Following", "following_id", "dbo.Person");
            DropForeignKey("dbo.Following", "user_id", "dbo.Person"); 
            DropIndex("dbo.Following", new[] { "following_id" });
            DropIndex("dbo.Following", new[] { "user_id" });
            DropIndex("dbo.Tweet", new[] { "user_id" });
            DropTable("dbo.Following");
            DropTable("dbo.Tweet");
            DropTable("dbo.Person");
        }
    }
}
