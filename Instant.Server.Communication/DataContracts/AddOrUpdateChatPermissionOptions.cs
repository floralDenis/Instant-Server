using System;
using System.Runtime.Serialization;
using Instant.Server.Communication.DataContracts.Enums;

namespace Instant.Server.Communication.DataContracts
{
    [DataContract]
    public class AddOrUpdateChatPermissionOptions
    {
        [DataMember]
        public int ChatPermissionId { get; set; }
        
        [DataMember]
        public string ChatMemberLogin { get; set; }
        
        [DataMember]
        public int ChatId { get; set; }
        
        [DataMember]
        public ChatPermissionTypes PermissionType { get; set; }
        
        [DataMember]
        public string InitiatorLogin { get; set; }

        [DataMember]
        public DateTime Date { get; set; }
    }
}