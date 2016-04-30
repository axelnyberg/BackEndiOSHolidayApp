namespace Cloud2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Media", new[] { "memory_ID" });
            DropIndex("dbo.Memories", new[] { "vacation_ID" });
            DropIndex("dbo.Vacations", new[] { "user_ID" });
            DropIndex("dbo.UserUsers", new[] { "User_ID" });
            DropIndex("dbo.UserUsers", new[] { "User_ID1" });
            CreateIndex("dbo.Media", "memory_id");
            CreateIndex("dbo.Memories", "vacation_id");
            CreateIndex("dbo.Vacations", "user_id");
            CreateIndex("dbo.UserUsers", "User_id");
            CreateIndex("dbo.UserUsers", "User_id1");
        }
        
        public override void Down()
        {
            DropIndex("dbo.UserUsers", new[] { "User_id1" });
            DropIndex("dbo.UserUsers", new[] { "User_id" });
            DropIndex("dbo.Vacations", new[] { "user_id" });
            DropIndex("dbo.Memories", new[] { "vacation_id" });
            DropIndex("dbo.Media", new[] { "memory_id" });
            CreateIndex("dbo.UserUsers", "User_ID1");
            CreateIndex("dbo.UserUsers", "User_ID");
            CreateIndex("dbo.Vacations", "user_ID");
            CreateIndex("dbo.Memories", "vacation_ID");
            CreateIndex("dbo.Media", "memory_ID");
        }
    }
}
