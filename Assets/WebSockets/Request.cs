using UnityEngine;
using System.Collections;
using System;

namespace WebSocket_Client
{
    public class Request : Message
    {

        public delegate void OnReplyCallback(Reply reply);

        public OnReplyCallback onReply { get; set; }
    
        public string CorrelationID { get; protected set; }
    
        public Request(string action, string data = null): base(action, data)
        {
            CorrelationID = Guid.NewGuid().ToString();
        }
    
        public Request(string action, byte[] data = null): base(action, data)
        {
            CorrelationID = Guid.NewGuid().ToString();
        }
    
        public virtual Request OnReply(OnReplyCallback onReply)
        {
            this.onReply = onReply;
            return this;
        }
    
        public void Send()
        {
            Client.Instance.Send(this.DataAsString,this);
        }
    
        public override string ToString()
        {
            return "CorrelationID: " + CorrelationID + "\n" + base.ToString();
        }
    }

}