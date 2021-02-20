﻿namespace Instant.Server.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTextAttributeToChatMessage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChatMessages", "Text", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ChatMessages", "Text");
        }
    }
}
