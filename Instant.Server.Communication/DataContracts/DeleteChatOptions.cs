using System.Runtime.Serialization;

namespace Instant.Server.Communication.DataContracts
{
    [DataContract]
    public class DeleteChatOptions
    {
        [DataMember]
        public int ChatId { get; set; }
        
        [DataMember]
        public string InitiatorLogin { get; set; }
    }
}