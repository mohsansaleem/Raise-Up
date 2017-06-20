using UnityEngine;
using System.Collections;

namespace WebSocket_Client
{
    public class InBoundMessage : Message
    {

        public InBoundMessage(string action = null, string data = null): base(action, data)
        {
        }
    
        public InBoundMessage(string action = null, byte[] data = null): base(action, data)
        {
        }
    
        public virtual void Dispatch()
        {
//        Client.OnReceivedCallback handler = null;
//        if (Client.Handlers.TryGetValue (Action, out handler))
//        {
//            Networking.instance.ResponseProcessRequests.Enqueue(() => { 
//                handler(this); 
//            } );
//        }
        }
    }
}