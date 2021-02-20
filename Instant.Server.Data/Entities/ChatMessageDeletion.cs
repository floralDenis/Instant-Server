using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Instant.Server.Data.Entities
{
    public class ChatMessageDeletion
    {
        [Required]
        [Key]
        [ForeignKey("ChatMessage")]
        public int ChatMessageId { get; set; }
        public ChatMessage ChatMessage { get; set; }
        public User Initiator { get; set; }
        public DateTime DateDeleted { get; set; }
    }
}