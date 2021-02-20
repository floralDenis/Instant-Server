using System.Runtime.Serialization;
using Instant.Server.Communication.DataContracts.Enums;

namespace Instant.Server.Communication.DataContracts
{
    [DataContract]
    [KnownType(typeof(User))]
    public class OperationResult
    {
        [DataMember]
        public OperationResultTypes OperationResultType { get; set; }
        
        [DataMember]
        public string Message { get; set; }
        
        [DataMember]
        public object ExtraData { get; set; }

        public OperationResult()
        { }
        
        public OperationResult(
            OperationResultTypes operationResultType,
            string message = "",
            object extraData = null)
        {
            this.OperationResultType = operationResultType;
            this.Message = message;
            this.ExtraData = extraData;
        }
    }
}