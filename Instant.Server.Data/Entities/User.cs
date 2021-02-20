using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.Options;

namespace Instant.Server.Data.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Login { get; set; }
        public string Password { get; set; }
        public DateTime LastUpdate { get; set; }
        public ICollection<ChatPermission> ChatPermissions { get; set; }
        public ICollection<ChatMessage> ChatMessages { get; set; }
    }
}