using System;
namespace GatewayXNative.Models
{
    public class WorkerMessage
    {
        public string type { get; set; }
        public object payload { get; set; }
        public WorkerMessage()
        { 
        } 

        public WorkerMessage(string type, object payload)
        { 
            this.type = type;
            this.payload = payload;
        } 
    }
}
