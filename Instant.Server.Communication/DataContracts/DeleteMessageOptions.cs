using System.Runtime.Serialization;

namespace Instant.Server.Communication.DataContracts
{
    [DataContract]
    public class DeleteMessageOptions
    {
        [DataMember]
        public int MessageId { get; set; }
        
        [DataMember]
        public string InitiatorLogin { get; set; }
    }
}