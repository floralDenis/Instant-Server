
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace Instant.Server.Data.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<Instant.Server.Data.InstantServerDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }
    } 
}