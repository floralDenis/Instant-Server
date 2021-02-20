using System;
using System.Runtime.Serialization;

namespace Instant.Server.Communication.DataContracts
{
    [DataContract]
    public class SendMessageOptions
    {
        [DataMember]
        public int MessageId { get; set; }
        
        [DataMember]
        public int ChatId { get; set; }
        
        [DataMember]
        public string SenderLogin { get; set; }
        
        [DataMember]
        public string Text { get; set; }
        
        [DataMember]
        public DateTime DateSent { get; set; }
    }
}