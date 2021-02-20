using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Instant.Server.Domain.Enums;

namespace Instant.Server.Communication.DataContracts
{
    [DataContract]
    [KnownType(typeof(AddOrUpdateChatPermissionOptions))]
    public class CreateOrUpdateChatOptions
    {
        [DataMember]
        public int ChatId { get; set; }
        
        [DataMember]
        public ChatTypes ChatType { get; set; }
        
        [DataMember]
        public string Title { get; set; }
        
        [DataMember]
        public List<string> MembersLogins { get; set; }

        [DataMember]
        public string InitiatorLogin { get; set; }
        
        [DataMember]
        public DateTime LastUpdate { get; set; }
        
        [DataMember]
        public object ExtraData { get; set; }
    }
}