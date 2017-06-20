using UnityEngine;
using System.Collections;

namespace WebSocket_Client
    {

public class Reply : InBoundMessage {

    public string CorrelationID { get; protected set; }
    
    public Reply(string correlationId, string action = null, string data = null): base(action, data)
    {
        CorrelationID = correlationId;
    }
    
    public Reply(string correlationId, string action = null, byte[] data = null): base(action, data)
    {
        CorrelationID = correlationId;
    }
    
//    public override void Dispatch()
//    {
//        Request request = null;
//        if (Client.Unreplied.TryGetValue(CorrelationID, out request))
//        {
//            if(Client.GetReplies)
//            {
//                lock (Client.Unreplied)
//                    Client.Unreplied.Remove(CorrelationID);
//                
//                if (!ReferenceEquals(request.onReply, null))
//                {
//                    //                        Loom.DispatchToMainThread(() => { 
//                    Networking.instance.ResponseProcessRequests.Enqueue(() => {
//                        request.onReply(request, this);
//                    });
//                    
//                    
//                    //                        });
//                }
//            }
//            
//        }
//        else
//            base.Dispatch();
//    }
    
    public override string ToString()
    {
        return "CorrelationID: " + CorrelationID + "\n" + base.ToString();
    }
}
}
