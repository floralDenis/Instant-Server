using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Instant.Server.Data.Entities
{
    public class ChatDeletion
    {
        [Required]
        [Key]
        [ForeignKey("Chat")]
        public int ChatId { get; set; }
        public Chat Chat { get; set; }
        public User Initiator { get; set; }
        public DateTime DateDeleted { get; set; }
    }
}