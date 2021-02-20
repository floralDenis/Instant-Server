using System;
using System.Runtime.Serialization;

namespace Instant.Server.Communication.DataContracts
{
    [DataContract]
    public class AuthorizeUserOptions
    {
        [DataMember]
        public string Login { get; set; }
        
        [DataMember]
        public string Password { get; set; }
        
        [DataMember]
        public DateTime LastOnline { get; set; }
    }
}