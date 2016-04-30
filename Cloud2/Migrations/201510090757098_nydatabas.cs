namespace Cloud2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nydatabas : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Media",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        fileurl = c.String(),
                        container = c.String(),
                        width = c.Int(),
                        height = c.Int(),
                        duration = c.Single(),
                        codec = c.String(),
                        bitrate = c.Single(),
                        channels = c.Int(),
                        samplingrate = c.Single(),
                        width1 = c.Int(),
                        height1 = c.Int(),
                        videocodec = c.String(),
                        videobitrate = c.Single(),
                        framerate = c.Single(),
                        audiocodec = c.String(),
                        audiobitrate = c.Single(),
                        channels1 = c.Int(),
                        samplingrate1 = c.Single(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        memory_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Memories", t => t.memory_ID)
                .Index(t => t.memory_ID);
            
            CreateTable(
                "dbo.Memories",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        title = c.String(),
                        description = c.String(),
                        place = c.String(),
                        time = c.Int(nullable: false),
                        position_longitude = c.Single(nullable: false),
                        position_latitude = c.Single(nullable: false),
                        vacation_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Vacations", t => t.vacation_ID)
                .Index(t => t.vacation_ID);
            
            CreateTable(
                "dbo.Vacations",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        title = c.String(),
                        description = c.String(),
                        place = c.String(),
                        start = c.Int(nullable: false),
                        end = c.Int(nullable: false),
                        user_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.user_ID)
                .Index(t => t.user_ID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        username = c.String(),
                        firstname = c.String(),
                        lastname = c.String(),
                        email = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserUsers",
                c => new
                    {
                        User_ID = c.Int(nullable: false),
                        User_ID1 = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_ID, t.User_ID1 })
                .ForeignKey("dbo.Users", t => t.User_ID)
                .ForeignKey("dbo.Users", t => t.User_ID1)
                .Index(t => t.User_ID)
                .Index(t => t.User_ID1);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Vacations", "user_ID", "dbo.Users");
            DropForeignKey("dbo.UserUsers", "User_ID1", "dbo.Users");
            DropForeignKey("dbo.UserUsers", "User_ID", "dbo.Users");
            DropForeignKey("dbo.Memories", "vacation_ID", "dbo.Vacations");
            DropForeignKey("dbo.Media", "memory_ID", "dbo.Memories");
            DropIndex("dbo.UserUsers", new[] { "User_ID1" });
            DropIndex("dbo.UserUsers", new[] { "User_ID" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Vacations", new[] { "user_ID" });
            DropIndex("dbo.Memories", new[] { "vacation_ID" });
            DropIndex("dbo.Media", new[] { "memory_ID" });
            DropTable("dbo.UserUsers");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Users");
            DropTable("dbo.Vacations");
            DropTable("dbo.Memories");
            DropTable("dbo.Media");
        }
    }
}
